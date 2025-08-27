using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Factory;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Business;

public class ThirdPartyCallBackService
{
    private readonly ILogger<ThirdPartyCallBackService> _logger;
    private readonly ThirdPartyCallbackFactory _thirdPartyCallbackFactory;

    public ThirdPartyCallBackService(ThirdPartyCallbackFactory thirdPartyCallbackFactory,
        ILogger<ThirdPartyCallBackService> logger)
    {
        _thirdPartyCallbackFactory = thirdPartyCallbackFactory;
        _logger = logger;
    }

    public void DoCallback(CallbackTypeEnum callbackTypeEnum, BpmnConfVo bpmnConfVo,
        string processNum, string businessId, string verifyUserName)
    {
        _logger.LogInformation("开始执行第三方回调, processNumber:{} , callBackUrl:{} , 审批人:{}",
            CallbackTypeEnum.PROC_END_CALL_BACK.GetDesc(), processNum, verifyUserName);
        _thirdPartyCallbackFactory.DoCallbackAsync<CallbackRespVo>(callbackTypeEnum, bpmnConfVo, processNum,
            businessId);
    }
}