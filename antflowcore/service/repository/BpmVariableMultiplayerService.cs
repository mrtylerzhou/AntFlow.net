using AntFlowCore.Entities;
using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableMultiplayerService: AFBaseCurdRepositoryService<BpmVariableMultiplayer>
{
    public BpmVariableMultiplayerService(IFreeSql freeSql) : base(freeSql)
    {
    }
}