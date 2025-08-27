using AntFlow.Core.Adaptor;
using AntFlow.Core.Constant.Enums;
using System.Text.Json;

namespace AntFlow.Core.Vo;

public class ProcSubmitCallbackAdp : ICallbackAdaptor<ProcSubmitCallbackReqVo, CallbackRespVo>
{
    public ProcSubmitCallbackReqVo FormatRequest(BpmnConfVo bpmnConfVo)
    {
        return new ProcSubmitCallbackReqVo(bpmnConfVo.FormData);
    }


    public CallbackRespVo FormatResponse(string resultJson)
    {
        return JsonSerializer.Deserialize<CallbackRespVo>(resultJson);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(CallbackTypeEnum.PROC_SUBMIT_CALL_BACK);
    }
}