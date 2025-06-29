using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableMultiplayerService: AFBaseCurdRepositoryService<BpmVariableMultiplayer>
{
    public BpmVariableMultiplayerService(IFreeSql freeSql) : base(freeSql)
    {
    }
}