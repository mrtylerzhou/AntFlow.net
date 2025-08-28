using AntFlow.Core.Adaptor.ProcessOperation;
using AntFlow.Core.Bpmn.Listener;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Factory;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using static AntFlow.Core.Constant.Enums.CallbackTypeEnum;

namespace AntFlow.Core.Service.Business;

public class ButtonOperationService
{
    private readonly IAdaptorFactory _adaptorFactory;
    private readonly IFreeSql _freeSql;
    private readonly ITaskListener _taskListener;
    private readonly ThirdPartyCallBackService _thirdPartyCallBackService;

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
        processOperation.DoProcessButton(vo);

        if (vo.IsOutSideAccessProc == true)
        {
            string verifyUserName = SecurityUtils.GetLogInEmpName();
            if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_AGREE)
            {
                _thirdPartyCallBackService.DoCallback(PROC_COMMIT_CALL_BACK, vo.BpmnConfVo,
                    vo.ProcessNumber, vo.BusinessId, verifyUserName);
            }
            else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT)
            {
                _thirdPartyCallBackService.DoCallback(PROC_STARTED_CALL_BACK, vo.BpmnConfVo,
                    vo.ProcessNumber, vo.BusinessId, verifyUserName);
            }
            else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE)
            {
                _thirdPartyCallBackService.DoCallback(PROC_END_CALL_BACK, vo.BpmnConfVo,
                    vo.ProcessNumber, vo.BusinessId, verifyUserName);
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
            if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_AGREE)
            {
                foreach (BpmAfTask bpmAfTask in bpmAfTasks)
                {
                    bpmAfTask.ProcessNumber = vo.ProcessNumber;
                    _taskListener.Notify(bpmAfTask, ITaskListener.EVENTNAME_COMPLETE);
                }
            }
            else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT)
            {
                _taskListener.Notify(new BpmAfTask { ProcessNumber = vo.ProcessNumber },
                    ITaskListener.EVENTNAME_CREATE);
            }
            else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_RESUBMIT)
            {
                _taskListener.Notify(new BpmAfTask { ProcessNumber = vo.ProcessNumber },
                    ITaskListener.EVENTNAME_RE_SUBMIT);
            }
            else if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE)
            {
                _taskListener.Notify(new BpmAfTask { ProcessNumber = vo.ProcessNumber },
                    ITaskListener.EVENTNAME_COMPLETE);
            }
        }

        return vo;
    }
}