using AntFlowCore.Abstraction.Orm.util;
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
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class SubmitProcessService: IProcessOperationAdaptor
{
    private readonly IFormFactory _formFactory;
    private readonly IBpmnConfCommonService _bpmnConfCommonService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly ILogger<SubmitProcessService> _logger;

    public SubmitProcessService(
        IFormFactory formFactory,
        IBpmnConfCommonService bpmnConfCommonService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        ILogger<SubmitProcessService> logger)
    {
        _formFactory = formFactory;
        _bpmnConfCommonService = bpmnConfCommonService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _logger = logger;
    }
    public void DoProcessButton(BusinessDataVo businessDataVo)
    {
        _logger.LogInformation($"Start submit process. param:{businessDataVo}");
        IFormOperationAdaptor<BusinessDataVo> formAdapter = _formFactory.GetFormAdaptor(businessDataVo);
        //记得参照示例,给businessDataVo赋必要值
        formAdapter.OnSubmitData(businessDataVo);
        String entryId = businessDataVo.EntityName + ":" + businessDataVo.BusinessId;
        // call the process's launch method to get launch parameters
        BpmnStartConditionsVo bpmnStartConditionsVo = formAdapter.PreviewSetCondition(businessDataVo);
        bpmnStartConditionsVo.BusinessDataVo = businessDataVo;
        bpmnStartConditionsVo.ApproversList = businessDataVo.ApproversList;

        string processNumber = businessDataVo.FormCode + "_" + businessDataVo.BusinessId;
        // migration (dynamic condition re-evaluation): keep the original process number so the
        // business number stays constant and bpmbusinessprocess is updated instead of re-created.
        if (businessDataVo.IsMigration == true)
        {
            processNumber = businessDataVo.ProcessNumber;
        }
        if (string.IsNullOrEmpty(businessDataVo.ProcessNumber))
        {
            businessDataVo.ProcessNumber = processNumber;
        }
        bpmnStartConditionsVo.ProcessNum = processNumber;
        bpmnStartConditionsVo.EntryId = entryId;
        bpmnStartConditionsVo.BusinessId = businessDataVo.BusinessId;
        bpmnStartConditionsVo.ApprovalEmpls = businessDataVo.ApprovalEmpls;
        bpmnStartConditionsVo.IsLowCodeFlow = businessDataVo.IsLowCodeFlow == 1;
        if (businessDataVo.IsMigration == true)
        {
            bpmnStartConditionsVo.IsMigration = businessDataVo.IsMigration;
        }
        else
        {
            if (!_bpmBusinessProcessService.CheckProcessData(entryId)) {
                throw new AFBizException("the process has already been submitted！");
            }
        }

        //process's name
        String processName = businessDataVo.FormCode;
        //apply user info
        String applyName = SecurityUtils.GetLogInEmpName();
        //save business and process information
        if (businessDataVo.IsMigration != true)
        {
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
            _bpmBusinessProcessService._repository.Add(bpmBusinessProcess);
            //the process number is predictable
            businessDataVo.ProcessNumber = businessDataVo.FormCode + "_" + businessDataVo.BusinessId;
        }
        _bpmnConfCommonService.StartProcess(businessDataVo.BpmnCode, bpmnStartConditionsVo);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_SUBMIT);
    }
}