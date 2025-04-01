using antflowcore.constant.enus;
using AntFlowCore.Constants;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ProcessApprovalService
{
    private readonly FormFactory _formFactory;
    private readonly ButtonOperationService _buttonOperationService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmVariableSignUpService _bpmVariableSignUpService;
    private readonly ProcessConstantsService _processConstantsService;
    private readonly ConfigFlowButtonContantService _configFlowButtonContantService;
    private readonly BpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IFreeSql _freeSql;
    private readonly ILogger _logger;

    public ProcessApprovalService(
        FormFactory formFactory,
        ButtonOperationService buttonOperationService,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmVariableSignUpService bpmVariableSignUpService,
        ProcessConstantsService processConstantsService,
        ConfigFlowButtonContantService configFlowButtonContantService,
        BpmVariableMultiplayerService bpmVariableMultiplayerService,
        IFreeSql freeSql,
        ILogger<ProcessApprovalService> logger
        )
    {
        _formFactory = formFactory;
        _buttonOperationService = buttonOperationService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmVariableSignUpService = bpmVariableSignUpService;
        _processConstantsService = processConstantsService;
        _configFlowButtonContantService = configFlowButtonContantService;
        _bpmVariableMultiplayerService = bpmVariableMultiplayerService;
        _freeSql = freeSql;
        _logger = logger;
    }
    public BusinessDataVo ButtonsOperation(String parameters, String formCode) {
       _logger.LogInformation($"params:{parameters},formCode:{formCode}");
       //deserialize parameters that passed in
       BusinessDataVo vo = _formFactory.DataFormConversion(parameters, formCode);
       //To determine the operation Type
       ProcessOperationEnum? poEnum = ProcessOperationEnumExtensions.GetEnumByCode(vo.OperationType);
       if (poEnum == null)
       {
           throw new AFBizException("unknown operation type,please Contact the Administrator");
       }
       formCode=vo.FormCode;
       ThreadLocalContainer.Set(StringConstants.FORM_CODE,formCode);
       //set the operation Flag
       if (poEnum==ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE || poEnum==ProcessOperationEnum.BUTTON_TYPE_STOP)
       {
           vo.Flag = false;
       } else if (poEnum==ProcessOperationEnum.BUTTON_TYPE_ABANDON)
       {
           vo.Flag = true;
       }
       //set start user Info
       if (string.IsNullOrEmpty(vo.StartUserId)) {
           vo.StartUserId=SecurityUtils.GetLogInEmpId();
           vo.StartUserName = SecurityUtils.GetLogInEmpName();
       }

       BusinessDataVo dataVo = null;
       _freeSql.Ado.Transaction(() =>
       {
            dataVo = _buttonOperationService.ButtonsOperationTransactional(vo);
       });

       return dataVo;

    }
    public BusinessDataVo GetBusinessInfo(string parameters, string formCode)
    {
        var vo = _formFactory.DataFormConversion(parameters, formCode);
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);

        if (bpmBusinessProcess == null)
        {
            throw new AFBizException($"processNumber {vo.ProcessNumber}, its data does not exist!");
        }

        vo.BusinessId = bpmBusinessProcess.BusinessId;

        BusinessDataVo businessDataVo;
        if (vo.IsOutSideAccessProc==null||!vo.IsOutSideAccessProc.Value || vo.IsLowCodeFlow == 1)
        {
            var formAdaptor = _formFactory.GetFormAdaptor(vo);
            formAdaptor.OnQueryData(vo);
            businessDataVo = vo;
        }
        else
        {
            businessDataVo = vo;
        }

        // 设置业务 ID
        businessDataVo.BusinessId = bpmBusinessProcess.BusinessId;

        // 设置其他重要信息
        businessDataVo.FormCode = vo.FormCode;
        businessDataVo.ProcessNumber = vo.ProcessNumber;

        // 校验流程权限，并从业务表中获取信息
        businessDataVo.ProcessRecordInfo = _processConstantsService.ProcessInfo(bpmBusinessProcess);
        businessDataVo.ProcessKey = bpmBusinessProcess.BusinessNumber;
        businessDataVo.ProcessState = bpmBusinessProcess.ProcessState != (int)ProcessStateEnum.END_STATE && bpmBusinessProcess.ProcessState != (int)ProcessStateEnum.REJECT_STATE;

        bool flag = businessDataVo.ProcessRecordInfo.StartUserId == SecurityUtils.GetLogInEmpIdStr();

        bool isJurisdiction = false; // TODO: 目前未实现

        // 设置操作按钮
        businessDataVo.ProcessRecordInfo.PcButtons = _configFlowButtonContantService.GetButtons(
            bpmBusinessProcess.BusinessNumber, 
            businessDataVo.ProcessRecordInfo.NodeId, 
            isJurisdiction, 
            flag
        );

        // 检查当前节点是否为报名节点，并设置属性
        string nodeId = businessDataVo.ProcessRecordInfo.NodeId;
        bool nodeIsSignUp = _bpmVariableSignUpService.CheckNodeIsSignUp(vo.ProcessNumber, nodeId);
        businessDataVo.IsSignUpNode = nodeIsSignUp;

        // 如果是报名节点，则添加“选择审核人”按钮
        if (nodeIsSignUp)
        {
            AddApproverButton(businessDataVo);
        }

        return businessDataVo;
    }
    private void AddApproverButton(BusinessDataVo businessDataVo)
    {
        // Set the approver button
        ProcessActionButtonVo addApproverButton = new ProcessActionButtonVo
        {
            ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_JP,
            Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_JP)
        };

        // Set add approver button on the PC
        var pcButtons = businessDataVo.ProcessRecordInfo.PcButtons;
        if (!pcButtons.TryGetValue(ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT), out var pcProcButtons))
        {
            pcProcButtons = new List<ProcessActionButtonVo>();
            pcButtons[ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT)] = pcProcButtons;
        }

        if (!pcProcButtons.Any(a => ConfigFlowButtonSortEnum.BUTTON_TYPE_JP.Code.Equals(a.ButtonType)))
        {
            pcProcButtons.Add(addApproverButton);
        }
        
    }
}

