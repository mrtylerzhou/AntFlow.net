using antflowcore.bpmn.service;
using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.repository;
using AntFlowCore.Vo;
using Microsoft.VisualBasic;

namespace antflowcore.adaptor.processoperation;

public class TaskRecoverProcessSerivce: IProcessOperationAdaptor
{
    private readonly BpmBusinessProcessService _businessProcessService;
    private readonly RuntimeService _runtimeService;

    public TaskRecoverProcessSerivce(BpmBusinessProcessService businessProcessService,
        RuntimeService runtimeService)
    {
        _businessProcessService = businessProcessService;
        _runtimeService = runtimeService;
    }
    public void DoProcessButton(BusinessDataVo vo)
    {
        String processNumber=vo.ProcessNumber;
        String taskDefKey = vo.TaskDefKey;
        if(string.IsNullOrEmpty(processNumber)){
            throw new AFBizException(BusinessError.PARAMS_IS_NULL,"请输入流程编号");
        }
        if(String.IsNullOrEmpty(taskDefKey)){
            throw new AFBizException(BusinessError.PARAMS_IS_NULL,"请输入流程taskDefKey");
        }
        BpmBusinessProcess bpmBusinessProcess = _businessProcessService.GetBpmBusinessProcess(processNumber);
        if(bpmBusinessProcess==null){
            throw  new AFBizException($"未能根据流程编号:{processNumber}找到流程信息");
        }
       
        _runtimeService.InsertTasks(bpmBusinessProcess,taskDefKey);
        bpmBusinessProcess.UpdateTime=DateTime.Now;
        bpmBusinessProcess.ProcessState=(int)ProcessStateEnum.HANDLE_STATE;
        _businessProcessService.Update(bpmBusinessProcess);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_RECOVER_TO_HIS);
    }
}