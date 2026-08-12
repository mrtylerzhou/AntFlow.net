using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service.processor;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Abstraction.service.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.processor.nextnode;

/// <summary>
/// 下一节点委托处理器:根据用户委托配置替换任务审批人,并写 BpmFlowrunEntrust 审计记录.
/// 对应 Java NextNodeForwardProcessor. Order=1,在 NextNodeLabelsProcessor(order=0) 之后执行.
///
/// 门禁(新增,优先级低于全局委托):全局委托未命中时,若流程配置了"审批人非办公状态自动转办"(AUTO_DELEGATE_OFF_DUTY=512),
/// 则调用 IUserService.CheckEmployeeEffective 判断审批人是否不可用,不可用且时间窗口命中且返回了转办目标人(DelegateUser)时,
/// 将任务转办给该目标人,并同样写 BpmFlowrunEntrust 审计记录。
/// </summary>
public class NextNodeForwardProcessor : INextNodeTaskProcessor
{
    private readonly IUserEntrustService _userEntrustService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IUserService _userService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly ILogger<NextNodeForwardProcessor> _logger;

    public NextNodeForwardProcessor(
        IUserEntrustService userEntrustService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IUserService userService,
        IBpmnConfService bpmnConfService,
        ILogger<NextNodeForwardProcessor> logger)
    {
        _userEntrustService = userEntrustService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _userService = userService;
        _bpmnConfService = bpmnConfService;
        _logger = logger;
    }

    public int Order() => 1;

    public void PostProcess(BpmNextTaskDto dto)
    {
        BpmAfTask delegateTask = dto.DelegateTask;
        string formCode = dto.FormCode;

        string oldUserId = delegateTask.Assignee;
        string oldUserName = delegateTask.AssigneeName ?? "";

        BaseIdTranStruVo entrustEmployee = _userEntrustService.GetEntrustEmployee(oldUserId, oldUserName, formCode);
        string userId = entrustEmployee.Id;
        string userName = entrustEmployee.Name;

        // 全局委托未命中(返回本人)时,进入"审批人非办公状态自动转办"门禁:
        // 流程配置了 AUTO_DELEGATE_OFF_DUTY 且审批人不可用(时间窗口命中)且返回了转办目标人,则转办给该目标人
        if (oldUserId != null && oldUserId.Equals(userId))
        {
            BaseIdTranStruVo offDutyDelegate = TryOffDutyDelegate(dto, oldUserId);
            if (offDutyDelegate != null && !string.IsNullOrEmpty(offDutyDelegate.Id))
            {
                userId = offDutyDelegate.Id;
                userName = offDutyDelegate.Name;
            }
        }

        // if userId is not null and valid then set user task delegate
        if (!string.IsNullOrEmpty(userId))
        {
            delegateTask.Assignee = userId;
            delegateTask.AssigneeName = userName;
        }

        // 如果委托生效,则在我的委托列表中加一条数据
        if (!oldUserId.Equals(userId))
        {
            _bpmFlowrunEntrustService.AddFlowrunEntrust(
                userId, userName, oldUserId, oldUserName,
                delegateTask.Id, 1, delegateTask.ProcInstId, formCode,
                delegateTask.TaskDefKey, 1);
            _logger.LogInformation("委托生效,委托前:{},委托后:{}", oldUserId, userId);
        }
    }

    /// <summary>
    /// 审批人非办公状态自动转办门禁:
    /// 1. 流程未配置 AUTO_DELEGATE_OFF_DUTY → 不转办
    /// 2. 审批人处于办公状态(可用)→ 不转办
    /// 3. 不可用但时间窗口未命中 → 不转办
    /// 4. 不可用且时间窗口命中 → 返回接口给定的转办目标人(DelegateUser)
    /// </summary>
    private BaseIdTranStruVo TryOffDutyDelegate(BpmNextTaskDto dto, string oldUserId)
    {
        if (string.IsNullOrEmpty(oldUserId))
        {
            return null;
        }
        // 1. 流程配置门禁
        BpmnConf bpmnConf = GetBpmnConf(dto);
        if (bpmnConf == null || !BpmnConfFlagsEnum.HasFlag(bpmnConf.ExtraFlags, BpmnConfFlagsEnum.AUTO_DELEGATE_OFF_DUTY))
        {
            return null;
        }
        // 2. 审批人可用性(办公状态)
        UserAvailableVo availableVo = _userService.CheckEmployeeEffective(oldUserId);
        if (availableVo == null || availableVo.Available != false)
        {
            return null;
        }
        // 3. 不可用时间窗口判断
        if (!IsUnavailableTimeWindowHit(availableVo))
        {
            return null;
        }
        // 4. 返回转办目标人
        return availableVo.DelegateUser;
    }

    /// <summary>
    /// 不可用时间窗口四象限判断(当前时间):
    /// - 无开始无结束 → 永久不可用,直接生效
    /// - 只有开始时间,开始早于当前 → 生效
    /// - 只有结束时间,结束晚于当前 → 生效
    /// - 同时有开始和结束,当前在区间内 → 生效
    /// </summary>
    private static bool IsUnavailableTimeWindowHit(UserAvailableVo availableVo)
    {
        DateTime now = DateTime.Now;
        DateTime? begin = availableVo.UnavailableBeginTime;
        DateTime? end = availableVo.UnavailableEndTime;
        if (begin == null && end == null)
        {
            return true;
        }
        if (begin != null && end == null)
        {
            return now >= begin.Value;
        }
        if (begin == null && end != null)
        {
            return now <= end.Value;
        }
        return now >= begin.Value && now <= end.Value;
    }

    /// <summary>
    /// 按流程 bpmnCode 查询流程配置
    /// </summary>
    private BpmnConf GetBpmnConf(BpmNextTaskDto dto)
    {
        if (string.IsNullOrEmpty(dto.BpmnCode))
        {
            return null;
        }
        return _bpmnConfService._repository.Find(a => a.BpmnCode.Equals(dto.BpmnCode)).FirstOrDefault();
    }
}
