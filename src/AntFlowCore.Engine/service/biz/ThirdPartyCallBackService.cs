using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.Engine.factory;
using AntFlowCore.Engine.factory;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

public class ThirdPartyCallBackService : IThirdPartyCallBackService
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