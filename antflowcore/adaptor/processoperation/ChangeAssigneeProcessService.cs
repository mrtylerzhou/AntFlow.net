using antflowcore.constant.enums;
using antflowcore.service.biz;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class ChangeAssigneeProcessService : IProcessOperationAdaptor
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly AFTaskService _taskService;
    private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly TaskMgmtService _taskMgmtService;

    public ChangeAssigneeProcessService(
        BpmBusinessProcessService bpmBusinessProcessService,
        AFTaskService taskService,
        BpmFlowrunEntrustService bpmFlowrunEntrustService,
        TaskMgmtService taskMgmtService
        )
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _taskMgmtService = taskMgmtService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
        var taskList = _taskService.baseRepo
            .Where(t => t.ProcInstId == bpmBusinessProcess.ProcInstId)
            .ToList();

        var userInfos = vo.UserInfos;
        var userIds = userInfos.Select(ui => ui.Id).ToList();

        for (int i = 0; i < taskList.Count; i++)
        {
            var task = taskList[i];
            var user = userInfos[i].Id;
            var userName = userInfos[i].Name;
            var assignee = task.Assignee;
            var assigneeName = task.Description;

            if (!userIds.Contains(assignee))
            {
                _bpmFlowrunEntrustService.AddFlowrunEntrust(user, userName, assignee, assigneeName, task.Id, 0,
                    bpmBusinessProcess.ProcInstId, vo.ProcessKey);
            }

            var taskMgmtVo = new TaskMgmtVO
            {
                ApplyUser = user,
                ApplyUserName = userName,
                TaskId = task.Id
            };

            _taskMgmtService.UpdateTaskInst(taskMgmtVo);
            _taskMgmtService.UpdateTask(taskMgmtVo);
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_CHANGE_ASSIGNEE);
    }
}