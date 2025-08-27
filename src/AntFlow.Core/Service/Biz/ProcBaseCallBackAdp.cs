using AntFlow.Core.Adaptor;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Vo;
using System.Text.Json;

namespace AntFlow.Core.Service.Business;

public class ProcBaseCallBackAdp : ICallbackAdaptor<ProcBaseCallBackVo, CallbackRespVo>
{
    public ProcBaseCallBackVo FormatRequest(BpmnConfVo bpmnConfVo)
    {
        return new ProcBaseCallBackVo(bpmnConfVo.FormData);
    }

    public CallbackRespVo FormatResponse(string resultJson)
    {
        return JsonSerializer.Deserialize<CallbackRespVo>(resultJson);
    }

    public CallbackRespVo GetNewRespObj()
    {
        return new CallbackRespVo();
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(
            CallbackTypeEnum.PROC_FINISH_CALL_BACK,
            CallbackTypeEnum.PROC_END_CALL_BACK,
            CallbackTypeEnum.PROC_COMMIT_CALL_BACK,
            CallbackTypeEnum.PROC_STARTED_CALL_BACK);
    }
}