using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IThirdPartyCallBackService
{
    void DoCallback(CallbackTypeEnum callbackTypeEnum, BpmnConfVo bpmnConfVo, string processNum, string businessId, string verifyUserName);
}
