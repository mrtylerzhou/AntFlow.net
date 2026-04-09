using System.Text.Json;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.vo;

public class ProcSubmitCallbackAdp : ICallbackAdaptor<ProcSubmitCallbackReqVo, CallbackRespVo> {

   
    public ProcSubmitCallbackReqVo FormatRequest(BpmnConfVo bpmnConfVo) {

        return new  ProcSubmitCallbackReqVo(bpmnConfVo.FormData);

    }

   
    public CallbackRespVo FormatResponse(String resultJson) {
        return JsonSerializer.Deserialize<CallbackRespVo>(resultJson);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(CallbackTypeEnum.PROC_SUBMIT_CALL_BACK);
    }
}
