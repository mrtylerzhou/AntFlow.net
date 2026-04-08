using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class SubmitProcessService: IProcessOperationAdaptor
{
    private readonly IFormFactory _formFactory;
    private readonly IBpmnConfCommonService _bpmnConfCommonService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmProcessNameService _bpmProcessNameService;
    private readonly ILogger<SubmitProcessService> _logger;

    public SubmitProcessService(
        IFormFactory formFactory,
        IBpmnConfCommonService bpmnConfCommonService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmProcessNameService bpmProcessNameService,
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
        bpmnStartConditionsVo.ApprovalEmpls=businessDataVo.ApprovalEmpls;
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