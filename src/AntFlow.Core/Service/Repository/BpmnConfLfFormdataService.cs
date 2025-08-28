using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnConfLfFormdataService : AFBaseCurdRepositoryService<BpmnConfLfFormdata>, IBpmnConfLfFormdataService
{
    public BpmnConfLfFormdataService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmnConfLfFormdata> ListByConfId(long confId)
    {
        List<BpmnConfLfFormdata> lfFormdatas = baseRepo.Select.Where(a => a.BpmnConfId == confId).ToList();
        return lfFormdatas;
    }
}