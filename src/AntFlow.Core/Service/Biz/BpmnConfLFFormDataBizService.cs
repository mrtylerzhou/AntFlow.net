using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;

namespace AntFlow.Core.Service.Business;

public class BpmnConfLFFormDataBizService
{
    private readonly BpmnConfLfFormdataService _bpmnConfLfFormdataService;

    public BpmnConfLFFormDataBizService(BpmnConfLfFormdataService bpmnConfLfFormdataService)
    {
        _bpmnConfLfFormdataService = bpmnConfLfFormdataService;
    }

    public BpmnConfLfFormdata GetLFFormDataByFormCode(string formCode)
    {
        BpmnConfLfFormdata bpmnConfLfFormdata = _bpmnConfLfFormdataService
            .Frsql
            .Select<BpmnConfLfFormdata, BpmnConf>()
            .InnerJoin((a, b) => a.BpmnConfId == b.Id && b.EffectiveStatus == 1)
            .Where(m => m.t2.FormCode == formCode)
            .First();
        return bpmnConfLfFormdata;
    }
}