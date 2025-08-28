using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface;

namespace AntFlow.Core.Service.Repository;

public class AFDeploymentService : AFBaseCurdRepositoryService<BpmAfDeployment>, IAFDeploymentService
{
    public AFDeploymentService(IFreeSql freeSql) : base(freeSql)
    {
    }
}