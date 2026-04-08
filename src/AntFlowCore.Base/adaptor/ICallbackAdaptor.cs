using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.adaptor;

public interface ICallbackAdaptor<out TReq, out TResp>: IAdaptorService
    where TReq : CallbackReqVo
    where TResp : CallbackRespVo, new()
{
    TReq FormatRequest(BpmnConfVo conf);
    TResp FormatResponse(string json);

    TResp GetNewRespObj()
    {
        return new TResp();
    }
}
