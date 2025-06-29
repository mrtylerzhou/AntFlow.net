using antflowcore.entity;

namespace antflowcore.service.repository;

public class BpmVariableApproveRemindService: AFBaseCurdRepositoryService<BpmVariableApproveRemind>
{
    public BpmVariableApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }
}