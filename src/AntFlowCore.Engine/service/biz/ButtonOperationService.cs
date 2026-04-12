using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor;
using AntFlowCore.Bpmn.listener;

namespace AntFlowCore.Engine.service.biz;

public class ButtonOperationService : IButtonOperationService
{
    private readonly IFreeSql _freeSql;
    private readonly ITaskListener _taskListener;
    private readonly IThirdPartyCallBackService _thirdPartyCallBackService;
    private readonly IAdaptorFactory _adaptorFactory;

    public ButtonOperationService(
        IFreeSql freeSql,
        ITaskListener taskListener,
        ThirdPartyCallBackService thirdPartyCallBackService,
        IAdaptorFactory adaptorFactory)
    {
        _freeSql = freeSql;
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
                List<BpmAfTask> bpmAfTasks = _freeSql
                    .Select<BpmAfTask>()
                    .From<BpmBusinessProcess>((a, b) =>
                        a.InnerJoin(x => x.ProcInstId == b.ProcInstId)
                    )
                    .Where((a, b) => b.BusinessNumber == vo.ProcessNumber)
                    .ToList();
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
                foreach (BpmAfTask bpmAfTask in bpmAfTasks)
                {
                    bpmAfTask.ProcessNumber = vo.ProcessNumber;
                    _taskListener.Notify(bpmAfTask,eventName);
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