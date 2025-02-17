using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableViewPageButtonService: AFBaseCurdRepositoryService<BpmVariableViewPageButton>
{
    public BpmVariableViewPageButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }
}