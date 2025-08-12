using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmVariableMultiplayerService: AFBaseCurdRepositoryService<BpmVariableMultiplayer>,IBpmVariableMultiplayerService
{
    public BpmVariableMultiplayerService(IFreeSql freeSql) : base(freeSql)
    {
    }
}