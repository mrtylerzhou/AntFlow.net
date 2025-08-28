using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableSingleService : AFBaseCurdRepositoryService<BpmVariableSingle>, IBpmVariableSingleService
{
    public BpmVariableSingleService(IFreeSql freeSql) : base(freeSql)
    {
    }
}