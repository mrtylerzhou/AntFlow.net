using System.Text.Json;
using antflowcore.adaptor;
using antflowcore.constant.enus;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

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

