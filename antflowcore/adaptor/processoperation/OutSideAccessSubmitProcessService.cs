using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.processoperation;

/// <summary>
/// Third-party process submit implementation
/// </summary>
public class OutSideAccessSubmitProcessService : IProcessOperationAdaptor
{
    private readonly BpmnConfCommonService _bpmnConfCommonService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly UserService _employeeService;

    private readonly OutSideBpmAccessBusinessService _outSideBpmAccessBusinessService;
    private readonly OutSideBpmConditionsTemplateService _outSideBpmConditionsTemplateService;
    private readonly FormFactory _formFactory;
    private readonly ILogger<OutSideAccessSubmitProcessService> _logger;

    public OutSideAccessSubmitProcessService(
        BpmnConfCommonService bpmnConfCommonService,
        BpmBusinessProcessService bpmBusinessProcessService,
        UserService employeeService,
        OutSideBpmAccessBusinessService outSideBpmAccessBusinessService,
        OutSideBpmConditionsTemplateService outSideBpmConditionsTemplateService,
        FormFactory formFactory,
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