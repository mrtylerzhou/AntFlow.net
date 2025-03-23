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
            .Select<BpmnConfLfFormdata>()
            .From<BpmnConfLfFormdata, BpmnConf>(
                (a,b,c)=>
                    a.LeftJoin(x=>x.BpmnConfId==c.Id)
            ).Where(m => m.t3.EffectiveStatus == 1 && m.t3.FormCode == formCode)
            .First();
        return bpmnConfLfFormdata;
    }
    
}