using AntFlowCore.Constants;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.util;
using AntFlowCore.Util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ProcessApprovalService
{
    private readonly FormFactory _formFactory;
    private readonly ButtonOperationService _buttonOperationService;
    private readonly IFreeSql _freeSql;
    private readonly ILogger _logger;

    public ProcessApprovalService(
        FormFactory formFactory,
        ButtonOperationService buttonOperationService,
        IFreeSql freeSql,
        ILogger<ProcessApprovalService> logger
        )
    {
        _formFactory = formFactory;
        _buttonOperationService = buttonOperationService;
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
}

