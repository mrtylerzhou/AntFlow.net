using AntFlowCore.Bpmn.service;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Common.util.Extension;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class FastForwardProcessService: IProcessOperationAdaptor
{
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IAFTaskService _afTaskInstService;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly TaskService _taskService;

    public FastForwardProcessService(IBpmBusinessProcessService bpmBusinessProcessService,
        IAFTaskService afTaskInstService,
        IBpmVerifyInfoService bpmVerifyInfoService,
        IBpmVariableService bpmVariableService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        TaskService taskService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _afTaskInstService = afTaskInstService;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _bpmVariableService = bpmVariableService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _taskService = taskService;
    }
    public void DoProcessButton(BusinessDataVo vo)
    {
        String processNumber = vo.ProcessNumber;
        String taskDefKey = vo.TaskDefKey;
        if(string.IsNullOrEmpty(processNumber)){
            throw new AFBizException(BusinessError.PARAMS_IS_NULL,"请输入流程编号");
        }
        if(string.IsNullOrEmpty(taskDefKey)){
            throw new AFBizException(BusinessError.PARAMS_IS_NULL,"请输入要跳转到的节点");
        }
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if(bpmBusinessProcess==null){
            throw  new AFBizException($"未能根据流程编号:{processNumber}找到流程信息");
        }
        String procInstId=bpmBusinessProcess.ProcInstId;
        
        List<BpmAfTask> taskList =_afTaskInstService.baseRepo
            .Where(a => a.ProcInstId == procInstId).ToList();
        if (taskList.IsEmpty()) {
            throw new AFBizException(BusinessError.STATUS_ERROR,"未获取到当前流程信息!,流程编号:" + bpmBusinessProcess.ProcessinessKey);
        }
        foreach (BpmAfTask task in taskList)
        {
            if (ProcessNodeEnum.Compare(taskDefKey,task.TaskDefKey)<0) {
                throw new AFBizException(BusinessError.STATUS_ERROR,"流程推进只能向前!");
            }
        }
        CompleteTaskRecursively(taskList,procInstId,taskDefKey,processNumber,vo.ApprovalComment,bpmBusinessProcess.ProcessinessKey);
    }
    private void CompleteTaskRecursively( List<BpmAfTask> taskList,String processInstanceId,String forwardToNodeElementId,String processNumber,String verifyComment,String processKey){
        if(taskList.IsEmpty()){
            return;
        }
      
        foreach (BpmAfTask task in taskList) {
            if (ProcessNodeEnum.Compare(task.TaskDefKey,forwardToNodeElementId)>0) {//如果已经到当前节点后面了,就直接return掉了
                return;
            }
            String actual=SecurityUtils.GetLogInEmpId();
            String actualName=SecurityUtils.GetLogInEmpName();
            _taskService.Complete(task);
            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = task.Name,
                TaskId = task.Id,
                RunInfoId = task.ProcInstId,
                   VerifyUserId = actual,
                   VerifyUserName = $"管理员-{actualName}",
                   TaskDefKey = task.TaskDefKey,
                   VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                   VerifyDesc = $"管理员跳过,原因:{verifyComment??""}",
                   ProcessCode = processNumber,
            };
            List<string> nodeIdsByeElementId = _bpmVariableService.GetNodeIdsByeElementId(processNumber,task.TaskDefKey);
            if (nodeIdsByeElementId.IsEmpty())
            {
                throw new AFBizException(BusinessError.STATUS_ERROR, "未能根据节点ElementId找到nodeId");
            }

            string nodeId = nodeIdsByeElementId[0];
            _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
            _bpmFlowrunEntrustService.AddFlowrunEntrust(actual,actualName,task.Assignee,task.AssigneeName,task.TaskDefKey,0,
                processInstanceId,processKey,nodeId,1);
        }
        
        List<BpmAfTask> tasks =_afTaskInstService.baseRepo
            .Where(a => a.ProcInstId == processInstanceId).ToList();
        CompleteTaskRecursively(tasks,processInstanceId,forwardToNodeElementId,processNumber,verifyComment,processKey);
    }
    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_PROCESS_MOVE_AHEAD);
    }
}