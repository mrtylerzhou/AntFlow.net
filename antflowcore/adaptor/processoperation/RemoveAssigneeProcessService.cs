using antflowcore.bpmn.service;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class RemoveAssigneeProcessService: IProcessOperationAdaptor
{
    private readonly AFTaskService _aFtaskService;
    private readonly TaskService _taskService;
    private readonly BpmFlowrunEntrustService _flowrunEntrustService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;

    public RemoveAssigneeProcessService(AFTaskService aFtaskService,
        TaskService taskService,
        BpmFlowrunEntrustService flowrunEntrustService,
        TaskMgmtService taskMgmtService,
        BpmBusinessProcessService bpmBusinessProcessService)
    {
        _aFtaskService = aFtaskService;
        _taskService = taskService;
        _flowrunEntrustService = flowrunEntrustService;
        _taskMgmtService = taskMgmtService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
    }
    public void DoProcessButton(BusinessDataVo vo)
    {
        String processNumber = vo.ProcessNumber;
        String taskDefKey = vo.TaskDefKey;
        List<BaseIdTranStruVo> userInfos = vo.UserInfos;
        if(userInfos==null){
            throw new AFBizException("请选择要减签的人员");
        }
        if(userInfos.Count>1){
            throw new AFBizException("减签只能选择一个人");
        }
        if(string.IsNullOrEmpty(processNumber)){
            throw new AFBizException("请指定流程编号");
        }
        if(string.IsNullOrEmpty(taskDefKey)){
            throw new AFBizException("请指定审任务节点");
        }
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException("未能根据流程编号找到流程信息!");
        }
        String processInstanceId = bpmBusinessProcess.ProcInstId;
        List<BpmAfTask> bpmAfTasks = _aFtaskService.baseRepo
            .Where(a=>a.ProcInstId==processInstanceId&&a.TaskDefKey==taskDefKey)
            .ToList<BpmAfTask>();
        if (bpmAfTasks.IsEmpty())
        {
            throw new AFBizException($"未能根据当前taskdefkey:{taskDefKey}找到任务");
        }

        if (bpmAfTasks.Count == 1)
        {
            _taskService.Complete(bpmAfTasks[0]);
            return;
        }

        string userToRemove = userInfos[0].Id;
        string userToRemoveName = userInfos[0].Name;
        List<BpmAfTask> currentAssigneeTasks = bpmAfTasks.Where(a=>a.Assignee==userToRemove).ToList();
        if (currentAssigneeTasks.IsEmpty())
        {
            throw new AFBizException($"未能找到当前用户{userToRemove}的审批任务");
        }

        BpmAfTask currentAssigneeTask = currentAssigneeTasks[0];
        string executionId = currentAssigneeTask.ExecutionId;
        string taskId = currentAssigneeTask.Id;
        _taskMgmtService.DeleteExecutionById(executionId);
        _taskMgmtService.DeletTask(taskId);
        _flowrunEntrustService.AddFlowrunEntrust("0","管理员减签",userToRemove,userToRemoveName,taskDefKey,0,
            processInstanceId,bpmBusinessProcess.ProcessinessKey,vo.NodeId,3);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_REMOVE_ASSIGNEE);
    }
}