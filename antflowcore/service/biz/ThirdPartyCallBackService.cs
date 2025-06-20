using antflowcore.constant.enus;
using antflowcore.factory;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ThirdPartyCallBackService
{
    private readonly ThirdPartyCallbackFactory _thirdPartyCallbackFactory;
    private readonly ILogger<ThirdPartyCallBackService> _logger;

    public ThirdPartyCallBackService(ThirdPartyCallbackFactory thirdPartyCallbackFactory,
        ILogger<ThirdPartyCallBackService> logger)
    {
        _thirdPartyCallbackFactory = thirdPartyCallbackFactory;
        _logger = logger;
    }
    public void DoCallback(CallbackTypeEnum callbackTypeEnum, BpmnConfVo bpmnConfVo,
        String processNum, String businessId,String verifyUserName){
        _logger.LogInformation("准备执行外部工作流回调, processNumber:{} , callBackUrl:{} , 操作人：{}",CallbackTypeEnum.PROC_END_CALL_BACK.GetDesc() ,processNum, verifyUserName);
        _thirdPartyCallbackFactory.DoCallbackAsync<CallbackRespVo>(callbackTypeEnum,bpmnConfVo, processNum, businessId);
    }
}