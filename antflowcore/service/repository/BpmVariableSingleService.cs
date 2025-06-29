using AntFlowCore.Entities;
using antflowcore.entity;

namespace antflowcore.service.repository;

public class BpmVariableSingleService: AFBaseCurdRepositoryService<BpmVariableSingle>
{
    public BpmVariableSingleService(IFreeSql freeSql) : base(freeSql)
    {
    }
}