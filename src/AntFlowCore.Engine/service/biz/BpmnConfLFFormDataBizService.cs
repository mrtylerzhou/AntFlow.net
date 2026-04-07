using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

public class BpmnConfLFFormDataBizService : IBpmnConfLFFormDataBizService
{
    private readonly IBpmnConfLfFormdataService _bpmnConfLfFormdataService;

    public BpmnConfLFFormDataBizService(IBpmnConfLfFormdataService bpmnConfLfFormdataService)
    {
        _bpmnConfLfFormdataService = bpmnConfLfFormdataService;
    }
    public BpmnConfLfFormdata GetLFFormDataByFormCode(String formCode)
    {
        BpmnConfLfFormdata bpmnConfLfFormdata = _bpmnConfLfFormdataService
            .Frsql
            .Select<BpmnConfLfFormdata,BpmnConf>()
            .InnerJoin((a,b)=>a.BpmnConfId ==b.Id&&b.EffectiveStatus==1)
            .Where(m => m.t2.FormCode == formCode)
            .First();
        return bpmnConfLfFormdata;
    }
    
}