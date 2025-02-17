using antflowcore.adaptor;
using antflowcore.constant;
using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.bpmn.listener;

public class BpmnExecutionListener: IExecutionListener
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly FormFactory _formFactory;
    private readonly ILogger<BpmnExecutionListener> _logger;

    public BpmnExecutionListener(
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmnConfService bpmnConfService,
        FormFactory formFactory,
        
        ILogger<BpmnExecutionListener> logger)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmnConfService = bpmnConfService;
        _formFactory = formFactory;
        _logger = logger;
    }
    public void Notify(BpmAfExecution execution,string eventName)
    {
        string procInstId = execution.ProcInstId;
        List<BpmBusinessProcess> bpmBusinessProcesses = _bpmBusinessProcessService
            .baseRepo
            .Where(a => a.ProcInstId == procInstId)
            .ToList();
        if (bpmBusinessProcesses==null||bpmBusinessProcesses.Count == 0)
        {
            throw new AFBizException($"Can not get bpm business process by procInstId:{procInstId}");
        }
        if (bpmBusinessProcesses.Count > 1)
        {
            throw new AFBizException($"Get more than one bpm business process by procInstId:{procInstId}");
        }

        BpmBusinessProcess bpmBusinessProcess = bpmBusinessProcesses[0];
        string businessId = bpmBusinessProcess.BusinessId;
        string startUser = bpmBusinessProcess.CreateUser;
        string processNumber = bpmBusinessProcess.BusinessNumber;
        string formCode = bpmBusinessProcess.ProcessinessKey;
        List<BpmnConf> bpmnConfs = _bpmnConfService.baseRepo
            .Where(a => a.BpmnCode == bpmBusinessProcess.Version).ToList();
        if (bpmnConfs == null || bpmnConfs.Count == 0)
        {
            throw new Exception($"No bpmn confs found by bpmncode:{bpmBusinessProcess.Version}");
        }
        if (bpmnConfs.Count > 1)
        {
            throw new AFBizException($"get more than one bpmn conf by bpmnCode:{bpmBusinessProcess.Version}");
        }

        BpmnConf bpmnConf = bpmnConfs[0];
        _logger.LogInformation($"execute {processNumber} process finished event Listener!");
        //to indicate it is not an outside process
        bool isOutside = false;
        if (bpmnConf.IsOutSideProcess == 1)//todo outside callback
        {
            isOutside = true;
        }
        else
        {
            BusinessDataVo businessDataVo=new BusinessDataVo();
            businessDataVo.BusinessId=businessId;
            businessDataVo.FormCode=formCode;
            if(bpmnConf.IsLowCodeFlow==1){
                businessDataVo.IsLowCodeFlow=1;
                BpmnConfVo confVo=new BpmnConfVo();
                GlobalConstant.Mapper.Map(bpmnConf,confVo);
                businessDataVo.BpmnConfVo=confVo;
            }

            IFormOperationAdaptor<BusinessDataVo> formOperationAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
            formOperationAdaptor.OnFinishData(businessDataVo);
        }

        _bpmBusinessProcessService
            .Frsql
            .Update<BpmBusinessProcess>()
            .Set(a => a.ProcessState, (int)ProcessStateEnum.HANDLE_STATE)
            .Where(a => a.Id == bpmBusinessProcess.Id)
            .ExecuteAffrows();
        //todo notification
    }
}