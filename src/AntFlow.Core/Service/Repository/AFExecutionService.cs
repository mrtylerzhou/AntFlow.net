using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface;

namespace AntFlow.Core.Service.Repository;

public class AFExecutionService : AFBaseCurdRepositoryService<BpmAfExecution>, IAFExecutionService
{
    public AFExecutionService(IFreeSql freeSql) : base(freeSql)
    {
    }
}