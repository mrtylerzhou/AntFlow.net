using antflowcore.entity;
using antflowcore.service.interf;

namespace antflowcore.service.repository;

public class AFExecutionService: AFBaseCurdRepositoryService<BpmAfExecution>,IAFExecutionService
{
    public AFExecutionService(IFreeSql freeSql) : base(freeSql)
    {
        
    }
}