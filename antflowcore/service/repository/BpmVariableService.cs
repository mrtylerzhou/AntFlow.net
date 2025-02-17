using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableService: AFBaseCurdRepositoryService<BpmVariable>
{
    public BpmVariableService(IFreeSql freeSql) : base(freeSql)
    {
    }
}