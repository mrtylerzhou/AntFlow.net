using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor;
using AntFlowCore.Bpmn.listener;
using AntFlowCore.Engine.factory;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

public class ButtonOperationService : IButtonOperationService
{
    private readonly IAFTaskService _taskService;
    private readonly ITaskListener _taskListener;
    private readonly IThirdPartyCallBackService _thirdPartyCallBackService;
    private readonly IAdaptorFactory _adaptorFactory;

    public ButtonOperationService(
        IAFTaskService taskService,
        ITaskListener taskListener,
        ThirdPartyCallBackService thirdPartyCallBackService,
        IAdaptorFactory adaptorFactory)
    {
        _taskService = taskService;
        _taskListener = taskListener;
        _thirdPartyCallBackService = thirdPartyCallBackService;
        _adaptorFactory = adaptorFactory;
    }

    public BusinessDataVo ButtonsOperationTransactional(BusinessDataVo vo)
    {

        //Do button operations
        IProcessOperationAdaptor processOperation = _adaptorFactory.GetProcessOperation(vo);
        try
        {
            // 将下一节点审批人写入 ThreadLocalContainer, 供 AFTaskService.InsertTasks 读取并替换虚拟审批人 -4
            // (上一节点指定审批人功能). 调用前设置, 内部使用后会被清空.
            if (vo.NextNodeApprovers != null && vo.NextNodeApprovers.Count > 0)
            {
                ThreadLocalContainer.Set(StringConstants.NEXT_NODE_APPROVER, vo.NextNodeApprovers);
            }
            processOperation.DoProcessButton(vo);
           
            if (vo.IsOutSideAccessProc == true)
            {
                String verifyUserName = SecurityUtils.GetLogInEmpName();
                if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_AGREE)
                {
                    _thirdPartyCallBackService.DoCallback( CallbackTypeEnum.PROC_COMMIT_CALL_BACK, vo.BpmnConfVo,
                        vo.ProcessNumber, vo.BusinessId,verifyUserName);
                }else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT)
                {
                    _thirdPartyCallBackService.DoCallback( CallbackTypeEnum.PROC_STARTED_CALL_BACK, vo.BpmnConfVo,
                        vo.ProcessNumber, vo.BusinessId,verifyUserName); 
                }else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE)
                {
                    _thirdPartyCallBackService.DoCallback( CallbackTypeEnum.PROC_END_CALL_BACK, vo.BpmnConfVo,
                        vo.ProcessNumber, vo.BusinessId,verifyUserName);
                } 
            }
            else
            {
                List<BpmAfTask> bpmAfTasks = _taskService._repository
                    .FindTasksByProcessNumber(vo.ProcessNumber);
                string eventName = "";
                if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_AGREE)
                {
                    eventName = ITaskListener.EVENTNAME_COMPLETE;
                }else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT)
                {
                    eventName = ITaskListener.EVENTNAME_CREATE;
                }else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_RESUBMIT)
                {
                    eventName = ITaskListener.EVENTNAME_RE_SUBMIT;
                }else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE)
                {
                    eventName= ITaskListener.EVENTNAME_DELETE;
                }
              
                ThreadLocalContainer.Set(StringConstants.AF_RUNTIME_BUISINESS_INFO, vo);
                try
                {
                    foreach (BpmAfTask bpmAfTask in bpmAfTasks)
                    {
                        bpmAfTask.ProcessNumber = vo.ProcessNumber;
                        _taskListener.Notify(bpmAfTask,eventName);
                    }
                }
                finally
                {
                    ThreadLocalContainer.Remove(StringConstants.AF_RUNTIME_BUISINESS_INFO);
                }
            }
           
           
        }
        catch (Exception e)
        {
            
            throw;
        }

        return vo;
    }
}