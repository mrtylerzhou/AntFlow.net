using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class ChangeAssigneeProcessService: IProcessOperationAdaptor
{
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IAFTaskService _taskService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly ITaskMgmtService _taskMgmtService;

    public ChangeAssigneeProcessService(
        IBpmBusinessProcessService bpmBusinessProcessService,
        IAFTaskService taskService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        ITaskMgmtService taskMgmtService
        )
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _taskMgmtService = taskMgmtService;
    }
    public void DoProcessButton(BusinessDataVo vo)
    {
        BpmBusinessProcess bpmBusinessProcess =_bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
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
                _bpmFlowrunEntrustService.AddFlowrunEntrust(user, userName, assignee, assigneeName, task.TaskDefKey, 0,
                    bpmBusinessProcess.ProcInstId, vo.ProcessKey,vo.NodeId,1);
            }

            TaskMgmtVO taskMgmtVo = new TaskMgmtVO
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