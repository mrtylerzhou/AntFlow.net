using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnViewPageButtonService: AFBaseCurdRepositoryService<BpmnViewPageButton>
{
    public BpmnViewPageButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public void DeleteByConfId(long confId)
    {
        this.baseRepo.Delete(a => a.ConfId == confId);
    }
}