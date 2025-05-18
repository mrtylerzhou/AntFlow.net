using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.vo;

namespace antflowcore.service.biz;

public class BpmnConfLFFormDataBizService
{
    private readonly BpmnConfLfFormdataService _bpmnConfLfFormdataService;

    public BpmnConfLFFormDataBizService(BpmnConfLfFormdataService bpmnConfLfFormdataService)
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