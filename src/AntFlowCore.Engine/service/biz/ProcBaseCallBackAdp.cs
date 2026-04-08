using System.Text.Json;
using AntFlowCore.Bpmn.adaptor;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Engine.service.biz;

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

