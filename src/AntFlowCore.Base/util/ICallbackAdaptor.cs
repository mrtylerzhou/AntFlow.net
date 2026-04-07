using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor;

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
