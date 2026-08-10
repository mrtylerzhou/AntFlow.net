using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor.processoperation;
using AntFlowCore.Bpmn.service.processor;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.processor.nextnode;

/// <summary>
/// 到达前设置(动态审批人)处理器.
/// <para>触发: 节点审批人为虚拟人 <see cref="AFSpecialAssigneeEnum.ARRIVAL_DYNAMIC_ASSIGNEE"/>("-5").
/// 流程发起时该虚拟人作为节点审批人透传到任务 assignee; 运行到该节点时 BpmnTaskListener 拉起本处理器,
/// 调用 <see cref="IFormOperationAdaptor{T}"/>.ProvideCurrentNodeAssignees 动态查询真实审批人,
/// 将虚拟人任务委托(setAssignee)给查到的真人, 达到动态目的.</para>
///
/// <para>处理规则:
/// <list type="bullet">
/// <item>assignee != -5: 直接返回(门控, 不影响其它节点).</item>
/// <item>assignee == -5 且 businessDataVo == null: 抛异常.
/// (.NET BpmnTaskListener 对 vo 有 DB fallback, 故 null 极罕见; 仅极端非用户链路触发, 回滚非用户事务.)</item>
/// <item>查到人(非空): 首个 setAssignee 委托(同步, 同 NextNodeForwardProcessor);
/// 其余人同步循环调 <see cref="AddAssigneeProcessService"/>.DoProcessButton 加签.</item>
/// <item>查不到人(空/null): 复用 AUTO_NODE_SKIP 跳过模式(setAssignee + ITaskService.Complete + 写 BpmVerifyInfo).</item>
/// </list></para>
///
/// <para>order=0: 先于 NextNodeForwardProcessor(委托, order=1) 执行, 使动态查出的人仍可叠加用户委托.
/// 与 NextNodeLabelsProcessor(0) 门控互斥(assignee 不同), 同级安全.</para>
///
/// <para><b>.NET 与 Java 的关键差异</b>: Java 版加签必须延迟到事务 afterCommit 后(避开 Activiti CommandContext 重入反模式);
/// .NET 版的 TaskService.Complete 是直接 DB 操作(无 CommandContext 缓冲, 见 ForwardToNodeService 同步执行 complete+TurnTransition),
/// 故本处理器<b>同步</b>完成 setAssignee + 加签, 无需 afterCommit/TransactionTemplate.</para>
/// 对应 Java NextNodeDynamicAssigneeProcessor.
/// </summary>
public class NextNodeDynamicAssigneeProcessor : INextNodeTaskProcessor
{
    /// <summary>bpm_flowrun_entrust.action_type: 0=委托(虚拟人委托给动态查到的真人, 与用户委托同语义)</summary>
    private const int ACTION_TYPE_ARRIVAL_DYNAMIC = 0;

    private readonly IFormFactory _formFactory;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly ILogger<NextNodeDynamicAssigneeProcessor> _logger;

    public NextNodeDynamicAssigneeProcessor(
        IFormFactory formFactory,
        IBpmVerifyInfoService bpmVerifyInfoService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        ILogger<NextNodeDynamicAssigneeProcessor> logger)
    {
        _formFactory = formFactory;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _logger = logger;
    }

    public int Order() => 0;

    public void PostProcess(BpmNextTaskDto dto)
    {
        BpmAfTask delegateTask = dto.DelegateTask;
        if (delegateTask == null)
        {
            return;
        }

        string assignee = delegateTask.Assignee;
        // 门控: 仅虚拟动态审批人节点生效
        if (!AFSpecialAssigneeEnum.ARRIVAL_DYNAMIC_ASSIGNEE.Id.Equals(assignee))
        {
            return;
        }

        BusinessDataVo businessDataVo = dto.BusinessDataVo;
        // vo==null 极罕见(.NET 有 DB fallback), 仅极端非用户链路; 抛异常回滚非用户事务
        if (businessDataVo == null)
        {
            throw new AFBizException("到达前设置(动态审批人)节点到达时 businessDataVo 为空, 无法动态查询审批人. "
                + "processNumber=" + dto.ProcessNumber + ", taskDefKey=" + dto.TaskDefKey
                + "(此场景多见于 timer/async/重试/迁移等非用户触发链路)");
        }

        // 获取运行时 FormOperationAdaptor(参照 SubmitProcessService.DoProcessButton)
        IFormOperationAdaptor<BusinessDataVo> formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
        {
            throw new AFBizException("到达前设置: 未找到 formCode=" + businessDataVo.FormCode + " 对应的 FormOperationAdaptor");
        }

        // 动态查询当前节点真实审批人
        List<BaseIdTranStruVo> assignees;
        try
        {
            assignees = formAdaptor.ProvideCurrentNodeAssignees(businessDataVo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "到达前设置: ProvideCurrentNodeAssignees 调用异常, processNumber={}, taskDefKey={}",
                dto.ProcessNumber, dto.TaskDefKey);
            throw new AFBizException("到达前设置: 动态查询审批人异常: " + e.Message);
        }

        // 查不到人: 复用 AUTO_NODE_SKIP 跳过模式
        if (assignees == null || assignees.Count == 0)
        {
            SkipCurrentNode(delegateTask, dto);
            return;
        }

        // 查到人: 首个 setAssignee 委托(同步)
        BaseIdTranStruVo first = assignees[0];
        if (first == null || string.IsNullOrEmpty(first.Id))
        {
            _logger.LogWarning("到达前设置: ProvideCurrentNodeAssignees 返回的首个审批人 id 为空, 跳过节点. processNumber={}",
                dto.ProcessNumber);
            SkipCurrentNode(delegateTask, dto);
            return;
        }

        string oldUserId = assignee; // -5
        string oldUserName = AFSpecialAssigneeEnum.ARRIVAL_DYNAMIC_ASSIGNEE.Desc;
        delegateTask.Assignee = first.Id;
        delegateTask.AssigneeName = first.Name;
        // 审计 bpm_flowrun_entrust(action_type=0 委托)
        _bpmFlowrunEntrustService.AddFlowrunEntrust(
            first.Id, first.Name, oldUserId, oldUserName,
            delegateTask.Id, 1, delegateTask.ProcInstId, dto.FormCode,
            delegateTask.TaskDefKey, ACTION_TYPE_ARRIVAL_DYNAMIC);
        _logger.LogInformation("到达前设置: 节点[{}] 虚拟人 {} -> 真人 {}({}), processNumber={}",
            delegateTask.TaskDefKey, oldUserId, first.Id, first.Name, dto.ProcessNumber);

        // 其余人: 同步循环调 AddAssigneeProcessService 加签
        // (.NET TaskService 为直接 DB 操作, 无 Activiti CommandContext 重入问题, 故同步即可, 无需 afterCommit)
        if (assignees.Count > 1)
        {
            AddRemainingAssignees(dto, assignees);
        }
    }

    /// <summary>
    /// 查不到人时跳过当前虚拟人节点: setAssignee(AUTO_NODE_SKIP) + ITaskService.Complete + 写 BpmVerifyInfo.
    /// 复用 NextNodeLabelsProcessor 的自动跳过模式.
    /// </summary>
    private void SkipCurrentNode(BpmAfTask delegateTask, BpmNextTaskDto dto)
    {
        string skipUserId = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id;
        string skipUserName = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc;
        delegateTask.Assignee = skipUserId;
        delegateTask.AssigneeName = skipUserName;

        ITaskService taskService = ServiceProviderUtils.GetService<ITaskService>();
        taskService?.Complete(delegateTask);

        BpmVerifyInfo verifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = delegateTask.Name,
            TaskId = delegateTask.Id,
            RunInfoId = delegateTask.ProcInstId,
            VerifyUserId = skipUserId,
            VerifyUserName = skipUserName,
            TaskDefKey = delegateTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = "到达前设置: 动态查询审批人为空, 自动跳过",
            ProcessCode = dto.ProcessNumber,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        try
        {
            _bpmVerifyInfoService.AddVerifyInfo(verifyInfo);
        }
        catch (Exception e)
        {
            // 审计写入失败不应阻断流程流转(任务已 complete)
            _logger.LogError(e, "到达前设置: 跳过节点写 BpmVerifyInfo 失败, processNumber={}, taskDefKey={}",
                dto.ProcessNumber, dto.TaskDefKey);
        }
        _logger.LogInformation("到达前设置: 节点[{}] 动态查询审批人为空, 自动跳过, processNumber={}",
            delegateTask.TaskDefKey, dto.ProcessNumber);
    }

    /// <summary>
    /// 同步循环调 AddAssigneeProcessService 为其余人加签(每人一次, 因 DoProcessButton 限制每次 1 人).
    /// <para>.NET 无 Activiti CommandContext 缓冲, TaskService 为直接 DB 操作, 故同步执行即可
    /// (Java 版需 afterCommit 延迟, .NET 版不需要, 见 ForwardToNodeService 同步 complete+TurnTransition 先例).</para>
    /// 加签失败仅告警, 不影响首人(首人已 setAssignee 生效).
    /// </summary>
    private void AddRemainingAssignees(BpmNextTaskDto dto, List<BaseIdTranStruVo> assignees)
    {
        AddAssigneeProcessService addAssignee = ServiceProviderUtils.GetService<AddAssigneeProcessService>();
        if (addAssignee == null)
        {
            _logger.LogError("到达前设置: 未获取到 AddAssigneeProcessService, 跳过加签(首人已生效). processNumber={}, taskDefKey={}",
                dto.ProcessNumber, dto.TaskDefKey);
            return;
        }

        for (int i = 1; i < assignees.Count; i++)
        {
            BaseIdTranStruVo person = assignees[i];
            if (person == null || string.IsNullOrEmpty(person.Id))
            {
                continue;
            }

            BusinessDataVo vo = new BusinessDataVo
            {
                FormCode = dto.FormCode,
                ProcessNumber = dto.ProcessNumber,
                TaskDefKey = dto.TaskDefKey,
                OperationType = (int)ProcessOperationEnum.BUTTON_TYPE_ADD_ASSIGNEE,
                UserInfos = new List<BaseIdTranStruVo> { person },
            };
            try
            {
                addAssignee.DoProcessButton(vo);
                _logger.LogInformation("到达前设置: 加签成功, processNumber={}, taskDefKey={}, 加签人={}({})",
                    dto.ProcessNumber, dto.TaskDefKey, person.Id, person.Name);
            }
            catch (Exception e)
            {
                // 单人加签失败不影响其余人/首人; 首人已 setAssignee 可正常审批
                _logger.LogError(e, "到达前设置: 加签失败, 跳过此人. processNumber={}, taskDefKey={}, 加签人={}({})",
                    dto.ProcessNumber, dto.TaskDefKey, person.Id, person.Name);
            }
        }
    }
}
