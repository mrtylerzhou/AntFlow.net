using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service.processor;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.processor.nextnode;

/// <summary>
/// 用户自动审批设置 运行时处理器. 对应 Java NextNodeAutoApproveProcessor.
/// Order=2: 在委托(order=1)之后执行, 评估实际处理人的自动审批配置.
/// 命中条件: enabled=1 ∧ 归属人==当前assignee ∧ config.bpmnCode==活跃bpmnCode ∧ 节点范围命中 ∧ (无条件 或 条件评估为true).
/// Fail-safe: 任何异常仅log, 不阻断流程.
/// </summary>
public class NextNodeAutoApproveProcessor : INextNodeTaskProcessor
{
    private readonly IBpmUserAutoApproveService _userAutoApproveService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly AutoNodeConditionEvaluator _conditionEvaluator;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly IFormFactory _formFactory;
    private readonly ILogger<NextNodeAutoApproveProcessor> _logger;

    public NextNodeAutoApproveProcessor(
        IBpmUserAutoApproveService userAutoApproveService,
        IBpmnConfService bpmnConfService,
        AutoNodeConditionEvaluator conditionEvaluator,
        IBpmVerifyInfoService bpmVerifyInfoService,
        IFormFactory formFactory,
        ILogger<NextNodeAutoApproveProcessor> logger)
    {
        _userAutoApproveService = userAutoApproveService;
        _bpmnConfService = bpmnConfService;
        _conditionEvaluator = conditionEvaluator;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _formFactory = formFactory;
        _logger = logger;
    }

    public int Order() => 2;

    public void PostProcess(BpmNextTaskDto dto)
    {
        try
        {
            DoProcess(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "自动审批处理异常, processNumber={PN}", dto.ProcessNumber);
        }
    }

    private void DoProcess(BpmNextTaskDto dto)
    {
        BpmAfTask delegateTask = dto.DelegateTask;
        BusinessDataVo vo = dto.BusinessDataVo;
        string formCode = dto.FormCode;
        if (delegateTask == null || vo == null || string.IsNullOrEmpty(formCode))
        {
            return;
        }
        string assignee = delegateTask.Assignee;
        if (string.IsNullOrEmpty(assignee))
        {
            return;
        }
        string activeBpmnCode = vo.BpmnCode;
        if (string.IsNullOrEmpty(activeBpmnCode))
        {
            activeBpmnCode = dto.BpmnCode;
        }
        if (string.IsNullOrEmpty(activeBpmnCode))
        {
            activeBpmnCode = _bpmnConfService._repository
                .FirstOrDefault(c => c.FormCode == formCode && c.EffectiveStatus == 1)?.BpmnCode;
        }
        if (string.IsNullOrEmpty(activeBpmnCode))
        {
            return;
        }
        List<BpmUserAutoApprove> configs = _userAutoApproveService.ListForRuntime(assignee, formCode, activeBpmnCode);
        if (configs.Count == 0)
        {
            return;
        }
        string taskDefKey = dto.TaskDefKey;
        foreach (BpmUserAutoApprove config in configs)
        {
            HitResult hit = MatchAndEvaluate(config, dto, vo, taskDefKey);
            if (hit == null)
            {
                continue;
            }
            CompleteAsAutoApprove(delegateTask, assignee, config, hit, dto);
            break;
        }
    }

    /// <summary>
    /// 命中结果: HasCondition/ConditionResult 用于审批意见文案
    /// </summary>
    private class HitResult
    {
        public bool HasCondition { get; set; }
        public bool? ConditionResult { get; set; }
    }

    /// <summary>
    /// 返回命中结果; 未命中返回 null.
    /// </summary>
    private HitResult MatchAndEvaluate(BpmUserAutoApprove config, BpmNextTaskDto dto, BusinessDataVo vo, string taskDefKey)
    {
        //节点范围
        if (!string.IsNullOrEmpty(config.NodeScopeJson))
        {
            List<UserAutoApproveVo.NodeScopeItem> scope =
                JsonConfUtil.ParseObject<List<UserAutoApproveVo.NodeScopeItem>>(config.NodeScopeJson);
            bool inScope = scope != null && scope.Any(i => i.ElementId == taskDefKey);
            if (!inScope)
            {
                return null;
            }
        }
        //无条件 → 直接命中
        BpmnNodeAutoNodeConfJson cond = string.IsNullOrEmpty(config.ConditionJson)
            ? null
            : JsonConfUtil.ParseObject<BpmnNodeAutoNodeConfJson>(config.ConditionJson);
        if (cond == null || cond.ConditionList == null || cond.ConditionList.Count == 0)
        {
            return new HitResult { HasCondition = false };
        }
        //有条件: 仅LF可评估
        if (vo.IsLowCodeFlow != 1)
        {
            return null;
        }
        vo.ProcessNumber = dto.ProcessNumber;
        vo.TaskDefKey = taskDefKey;
        vo.FormCode = dto.FormCode;
        if (vo.LfFields == null || vo.LfFields.Count == 0)
        {
            try
            {
                var formAdaptor = _formFactory.GetFormAdaptor(vo);
                if (formAdaptor == null)
                {
                    return null;
                }
                formAdaptor.OnQueryData(vo);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "自动审批拉取表单数据失败, processNumber={PN}", dto.ProcessNumber);
                return null;
            }
        }
        if (vo.LfFields == null || vo.LfFields.Count == 0)
        {
            return null;
        }
        bool? result = _conditionEvaluator.EvaluateConf(cond, vo.LfFields);
        if (result != true)
        {
            return null;
        }
        return new HitResult { HasCondition = true, ConditionResult = result };
    }

    private void CompleteAsAutoApprove(BpmAfTask delegateTask, string assignee, BpmUserAutoApprove config,
        HitResult hit, BpmNextTaskDto dto)
    {
        string assigneeName = delegateTask.AssigneeName ?? "";
        try
        {
            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService?.Complete(delegateTask);
        }
        catch (Exception e)
        {
            //任务可能已被前序处理器complete, 静默跳过
            _logger.LogWarning(e, "自动审批complete失败(任务可能已完成), processNumber={PN}", dto.ProcessNumber);
            return;
        }
        string desc;
        if (!string.IsNullOrEmpty(config.DefaultComment))
        {
            desc = StringConstants.AF_AUTO_APPROVE_PREFIX + config.DefaultComment;
        }
        else if (hit.HasCondition)
        {
            desc = StringConstants.AF_AUTO_APPROVE_PREFIX
                   + string.Format(StringConstants.AF_AUTO_APPROVE_COMMENT, hit.ConditionResult);
        }
        else
        {
            desc = StringConstants.AF_AUTO_APPROVE_PREFIX + StringConstants.AF_AUTO_APPROVE_UNCONDITIONAL_COMMENT;
        }
        BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = delegateTask.Name,
            TaskId = delegateTask.Id,
            RunInfoId = delegateTask.ProcInstId,
            VerifyUserId = assignee,
            VerifyUserName = assigneeName,
            TaskDefKey = delegateTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = desc,
            ProcessCode = dto.ProcessNumber,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
        _logger.LogInformation("自动审批命中: processNumber={PN}, assignee={A}, taskDefKey={T}",
            dto.ProcessNumber, assignee, dto.TaskDefKey);
    }
}
