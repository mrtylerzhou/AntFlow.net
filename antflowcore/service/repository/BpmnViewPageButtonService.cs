using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnViewPageButtonService: AFBaseCurdRepositoryService<BpmnViewPageButton>,IBpmnViewPageButtonService
{
    public BpmnViewPageButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public void DeleteByConfId(long confId)
    {
        this.baseRepo.Delete(a => a.ConfId == confId);
    }
}