using antflowcore.entity;

namespace antflowcore.service.repository;

public class AFDeploymentService : AFBaseCurdRepositoryService<BpmAfDeployment>
{
    public AFDeploymentService(IFreeSql freeSql) : base(freeSql)
    {
    }
}