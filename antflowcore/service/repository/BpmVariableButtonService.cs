using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableButtonService: AFBaseCurdRepositoryService<BpmVariableButton>
{
    public BpmVariableButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }
}