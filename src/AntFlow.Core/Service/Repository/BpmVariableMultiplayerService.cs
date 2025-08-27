using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableMultiplayerService : AFBaseCurdRepositoryService<BpmVariableMultiplayer>,
    IBpmVariableMultiplayerService
{
    public BpmVariableMultiplayerService(IFreeSql freeSql) : base(freeSql)
    {
    }
}