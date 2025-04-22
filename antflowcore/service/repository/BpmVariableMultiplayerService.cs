using antflowcore.entity;

namespace antflowcore.service.repository;

public class BpmVariableMultiplayerService : AFBaseCurdRepositoryService<BpmVariableMultiplayer>
{
    public BpmVariableMultiplayerService(IFreeSql freeSql) : base(freeSql)
    {
    }
}