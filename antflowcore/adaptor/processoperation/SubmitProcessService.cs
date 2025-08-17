using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.processoperation;

public class SubmitProcessService: IProcessOperationAdaptor
{
    private readonly FormFactory _formFactory;
    private readonly BpmnConfCommonService _bpmnConfCommonService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmProcessNameService _bpmProcessNameService;
    private readonly ILogger<SubmitProcessService> _logger;

    public SubmitProcessService(
        FormFactory formFactory,
        BpmnConfCommonService bpmnConfCommonService,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmProcessNameService bpmProcessNameService,
        ILogger<SubmitProcessService> logger)
    {
        _formFactory = formFactory;
        _bpmnConfCommonService = bpmnConfCommonService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmProcessNameService = bpmProcessNameService;
        _logger = logger;
    }
    public void DoProcessButton(BusinessDataVo businessDataVo)
    {
        _logger.LogInformation($"Start submit process. param:{businessDataVo}");
        IFormOperationAdaptor<BusinessDataVo> formAdapter = _formFactory.GetFormAdaptor(businessDataVo);
        //记得参照示例,给businessDataVo赋必要值
        formAdapter.OnSubmitData(businessDataVo);
        String entryId = businessDataVo.EntityName + ":" + businessDataVo.BusinessId;
        BpmnStartConditionsVo bpmnStartConditionsVo = formAdapter.PreviewSetCondition(businessDataVo);
        bpmnStartConditionsVo.ApproversList = businessDataVo.ApproversList;
        bpmnStartConditionsVo.ProcessNum=(businessDataVo.FormCode + "_" + businessDataVo.BusinessId);
        bpmnStartConditionsVo.EntryId=entryId;
        bpmnStartConditionsVo.BusinessId=businessDataVo.BusinessId;
        if (!_bpmBusinessProcessService.CheckProcessData(entryId)) {
            throw new AFBizException("the process has already been submitted！");
        }
        //process's name
        String processName = _bpmProcessNameService.GetBpmProcessName(businessDataVo.FormCode)?.ProcessName;
        //apply user info
        String applyName = SecurityUtils.GetLogInEmpName();
        string processNumber = businessDataVo.FormCode+"_"+businessDataVo.BusinessId;
        //save business and process information
        BpmBusinessProcess bpmBusinessProcess = new BpmBusinessProcess
        {
            BusinessId = businessDataVo.BusinessId,
            ProcessinessKey = businessDataVo.FormCode,
            BusinessNumber = processNumber,
            IsLowCodeFlow = businessDataVo.IsLowCodeFlow??0,
            CreateUser = businessDataVo.StartUserId,
            UserName = businessDataVo.StartUserName,
            CreateTime = DateTime.Now,
            ProcessState = (int)ProcessStateEnum.HANDLING_STATE,
            EntryId = entryId,
            Description = applyName+"-"+processName,
            DataSourceId = businessDataVo.DataSourceId,
            ProcessDigest = businessDataVo.ProcessDigest,
            Version = businessDataVo.BpmnCode,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmBusinessProcessService.baseRepo.Insert(bpmBusinessProcess);
        businessDataVo.ProcessNumber = processNumber;
        _bpmnConfCommonService.StartProcess(businessDataVo.BpmnCode, bpmnStartConditionsVo);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_SUBMIT);
    }
}