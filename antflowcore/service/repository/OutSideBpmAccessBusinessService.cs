using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class OutSideBpmAccessBusinessService: AFBaseCurdRepositoryService<OutSideBpmAccessBusiness>
{
    public OutSideBpmAccessBusinessService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void UpdateById(OutSideBpmAccessBusiness outSideBpmAccessBusiness)
    {
        this.baseRepo.Update(outSideBpmAccessBusiness);
    }
}