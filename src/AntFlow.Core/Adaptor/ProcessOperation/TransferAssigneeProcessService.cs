using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.ProcessOperation;

public class TransferAssigneeProcessService : IProcessOperationAdaptor
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly AFTaskService _taskService;

    public TransferAssigneeProcessService(
        BpmBusinessProcessService bpmBusinessProcessService,
        AFTaskService taskService,
        TaskMgmtService taskMgmtService,
        BpmFlowrunEntrustService bpmFlowrunEntrustService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
        _taskMgmtService = taskMgmtService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
        List<BpmAfTask>? tasks = _taskService
            .baseRepo
            .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
            .ToList();

        List<BaseIdTranStruVo>? userInfos = vo.UserInfos;
        if (userInfos.Count != 2)
        {
            throw new AFBizException("转办人员配置错误,无法转办!");
        }

        string? originalUserId = userInfos[0].Id;
        string? originalUserName = userInfos[0].Name;
        string? transferToUserId = userInfos[1].Id;
        string? transferToUserName = userInfos[1].Name;

        List<string>? assignees = tasks.Select(task => task.Assignee).ToList();
        int originAssigneeIndex = assignees.IndexOf(originalUserId);

        if (originAssigneeIndex < 0)
        {
            throw new AFBizException("流程状态已变更,无当前办理人信息,转办失败!");
        }

        bool matched = false;

        foreach (BpmAfTask? task in tasks)
        {
            if (task.Assignee == originalUserId)
            {
                _bpmFlowrunEntrustService.AddFlowrunEntrust(transferToUserId, transferToUserName, originalUserId,
                    originalUserName, task.Id, 0, bpmBusinessProcess.ProcInstId, vo.ProcessKey);

                TaskMgmtVO? taskMgmtVO = new()
                {
                    ApplyUser = transferToUserId, ApplyUserName = transferToUserName, TaskId = task.Id
                };

                _taskMgmtService.UpdateTaskInst(taskMgmtVO);
                _taskMgmtService.UpdateTask(taskMgmtVO);

                matched = true;
            }
        }

        if (!matched)
        {
            throw new AFBizException("流程状态已变更,无当前办理人信息,转办失败!");
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_ZB);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_ZB);
    }
}