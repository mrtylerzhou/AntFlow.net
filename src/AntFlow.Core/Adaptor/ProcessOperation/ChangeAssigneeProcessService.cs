using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.ProcessOperation;

public class ChangeAssigneeProcessService : IProcessOperationAdaptor
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly AFTaskService _taskService;

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
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
        List<BpmAfTask> taskList = _taskService.baseRepo
            .Where(t => t.ProcInstId == bpmBusinessProcess.ProcInstId)
            .ToList();

        List<BaseIdTranStruVo> userInfos = vo.UserInfos;
        List<string> userIds = userInfos.Select(ui => ui.Id).ToList();

        for (int i = 0; i < taskList.Count; i++)
        {
            BpmAfTask task = taskList[i];
            string user = userInfos[i].Id;
            string userName = userInfos[i].Name;
            string assignee = task.Assignee;
            string assigneeName = task.Description;

            if (!userIds.Contains(assignee))
            {
                _bpmFlowrunEntrustService.AddFlowrunEntrust(user, userName, assignee, assigneeName, task.Id, 0,
                    bpmBusinessProcess.ProcInstId, vo.ProcessKey);
            }

            TaskMgmtVO taskMgmtVo = new() { ApplyUser = user, ApplyUserName = userName, TaskId = task.Id };

            _taskMgmtService.UpdateTaskInst(taskMgmtVo);
            _taskMgmtService.UpdateTask(taskMgmtVo);
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_CHANGE_ASSIGNEE);
    }
}