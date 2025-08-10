using antflowcore.entity;
using antflowcore.service.interf;

namespace antflowcore.service.repository;

public class AFDeploymentService: AFBaseCurdRepositoryService<BpmAfDeployment>,IAFDeploymentService
{
    public AFDeploymentService(IFreeSql freeSql) : base(freeSql)
    {
    }
}