using System.Text.Json;
using AntFlowCore.Bpmn.adaptor;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Vo;

namespace AntFlowCore.Core.vo;

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
