using System.Runtime.InteropServices.JavaScript;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Constants;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.formoperation;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.factory;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using AntFlowCore.Extensions.service;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
/// Third-party process submit implementation
/// </summary>
public class OutSideAccessSubmitProcessService : IProcessOperationAdaptor
{
    private readonly IBpmnConfCommonService _bpmnConfCommonService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IUserService _employeeService;

    private readonly IOutSideBpmAccessBusinessService _outSideBpmAccessBusinessService;
    private readonly IOutSideBpmConditionsTemplateService _outSideBpmConditionsTemplateService;
    private readonly IFormFactory _formFactory;
    private readonly ILogger<OutSideAccessSubmitProcessService> _logger;

    public OutSideAccessSubmitProcessService(
        IBpmnConfCommonService bpmnConfCommonService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IUserService employeeService,
        IOutSideBpmAccessBusinessService outSideBpmAccessBusinessService,
        IOutSideBpmConditionsTemplateService outSideBpmConditionsTemplateService,
        IFormFactory formFactory,
        ILogger<OutSideAccessSubmitProcessService> logger)
    {
        _bpmnConfCommonService = bpmnConfCommonService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _employeeService = employeeService;
        _outSideBpmAccessBusinessService = outSideBpmAccessBusinessService;
        _outSideBpmConditionsTemplateService = outSideBpmConditionsTemplateService;
        _formFactory = formFactory;
        _logger = logger;
    }

    public void DoProcessButton(BusinessDataVo businessDataVo)
    {
        // Generate process number by rule
        string processNum = $"{businessDataVo.FormCode}_{businessDataVo.BusinessId}";

        // Check whether the process is already started
        if (!_bpmBusinessProcessService.CheckProcessData(processNum))
        {
            throw new AFBizException("流程已发起！");
        }
        String originalBusinessId=businessDataVo.BusinessId;
        if (businessDataVo.IsLowCodeFlow.HasValue && businessDataVo.IsLowCodeFlow == 1)
        {
            IFormOperationAdaptor<BusinessDataVo> formOperationAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
            formOperationAdaptor.OnSubmitData(businessDataVo);
        }
       
        // Query outside access business info
        OutSideBpmAccessBusiness outSideBpmAccessBusiness = _outSideBpmAccessBusinessService.baseRepo
            .Where(a => a.Id == Convert.ToInt64(originalBusinessId)).First();

        // New start conditions vo
        var bpmnStartConditionsVo = new BpmnStartConditionsVo();
        string templateMark = outSideBpmAccessBusiness.TemplateMark;

        if (!string.IsNullOrEmpty(templateMark))
        {
            // Query template mark
            OutSideBpmConditionsTemplate outSideBpmConditionsTemplate = _outSideBpmConditionsTemplateService.baseRepo.Where(x =>
                x.IsDel == 0 &&
                x.BusinessPartyId == outSideBpmAccessBusiness.BusinessPartyId &&
                x.TemplateMark == templateMark).First();

            if (outSideBpmConditionsTemplate == null)
            {
                throw new AFBizException($"条件模板[{templateMark}]已经失效，无法发起流程");
            }

            bpmnStartConditionsVo.TemplateMarkId = (int)outSideBpmConditionsTemplate.Id;
        }

        bpmnStartConditionsVo.OutSideType = businessDataVo.OutSideType;
        bpmnStartConditionsVo.BusinessId = businessDataVo.BusinessId;
        bpmnStartConditionsVo.StartUserId = businessDataVo.StartUserId;

        // Set approvers list
        bpmnStartConditionsVo.ApproversList = businessDataVo.ApproversList ?? new Dictionary<string, List<BaseIdTranStruVo>>();

        // Set process number
        bpmnStartConditionsVo.ProcessNum = processNum;

        // Set entry id
        bpmnStartConditionsVo.EntryId = processNum;
        bpmnStartConditionsVo.EmbedNodes = businessDataVo.EmbedNodes;
        bpmnStartConditionsVo.OutSideLevelNodes = businessDataVo.OutSideLevelNodes;
        bpmnStartConditionsVo.IsOutSideAccessProc = true;

        // Set process title
        string processTitlePrefix;
        processTitlePrefix = businessDataVo.SubmitUser;

        BpmBusinessProcess bpmBusinessProcess = new BpmBusinessProcess
        {
            BusinessId = businessDataVo.BusinessId,
            ProcessinessKey = businessDataVo.FormCode,
            BusinessNumber = processNum,
            CreateUser = businessDataVo.StartUserId,
            UserName = businessDataVo.SubmitUser,
            CreateTime = DateTime.Now,
            ProcessState = (int)ProcessStateEnum.HANDLING_STATE,
            EntryId = processNum,
            Description = $"{processTitlePrefix}-{businessDataVo.BpmnName}",
            Version = businessDataVo.BpmnCode,
            IsOutSideProcess = 1,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };

        // Save business process info
        _bpmBusinessProcessService.AddBusinessProcess(bpmBusinessProcess);
        businessDataVo.ProcessNumber = processNum;

        // Start process
        _bpmnConfCommonService.StartProcess(businessDataVo.BpmnCode, bpmnStartConditionsVo);

        // Fill info
        _outSideBpmAccessBusinessService.UpdateById(new OutSideBpmAccessBusiness
        {
            Id = long.Parse(originalBusinessId),
            ProcessNumber = processNum,
            BpmnConfId = businessDataVo.BpmnConfVo?.Id ?? 0
        });
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_SUBMIT);
    }
}