using AntFlowCore.Bpmn.service;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.util;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class TaskRecoverProcessSerivce: IProcessOperationAdaptor
{
    private readonly IBpmBusinessProcessService _businessProcessService;
    private readonly RuntimeService _runtimeService;

    public TaskRecoverProcessSerivce(IBpmBusinessProcessService businessProcessService,
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