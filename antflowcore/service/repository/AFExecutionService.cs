using antflowcore.entity;

namespace antflowcore.service.repository;

public class AFExecutionService : AFBaseCurdRepositoryService<BpmAfExecution>
{
    public AFExecutionService(IFreeSql freeSql) : base(freeSql)
    {
    }
}