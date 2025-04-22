using antflowcore.constant.enums;
using antflowcore.vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ThirdPartyCallBackService
{
    private readonly ILogger<ThirdPartyCallBackService> _logger;

    public ThirdPartyCallBackService(ILogger<ThirdPartyCallBackService> logger)
    {
        _logger = logger;
    }

    public void DoCallback(String url, CallbackTypeEnum callbackTypeEnum, BpmnConfVo bpmnConfVo,
        String processNum, String businessId, String verifyUserName)
    {
        _logger.LogInformation("准备执行外部工作流回调：{} , processNumber:{} , callBackUrl:{} , 操作人：{}", CallbackTypeEnum.PROC_END_CALL_BACK.Desc, processNum, url, verifyUserName);
        //回调通知业务方

        //todo 回调待实现
    }
}