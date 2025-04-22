using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableApproveRemindService : AFBaseCurdRepositoryService<BpmVariableApproveRemind>
{
    public BpmVariableApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }
}