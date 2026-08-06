using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service.processor;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.processor.nextnode;

/// <summary>
/// 下一节点委托处理器:根据用户委托配置替换任务审批人,并写 BpmFlowrunEntrust 审计记录.
/// 对应 Java NextNodeForwardProcessor. Order=1,在 NextNodeLabelsProcessor(order=0) 之后执行.
/// </summary>
public class NextNodeForwardProcessor : INextNodeTaskProcessor
{
    private readonly IUserEntrustService _userEntrustService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly ILogger<NextNodeForwardProcessor> _logger;

    public NextNodeForwardProcessor(
        IUserEntrustService userEntrustService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        ILogger<NextNodeForwardProcessor> logger)
    {
        _userEntrustService = userEntrustService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
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
}
