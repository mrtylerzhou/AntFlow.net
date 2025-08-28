using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnViewPageButtonService : AFBaseCurdRepositoryService<BpmnViewPageButton>, IBpmnViewPageButtonService
{
    public BpmnViewPageButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void DeleteByConfId(long confId)
    {
        baseRepo.Delete(a => a.ConfId == confId);
    }
}