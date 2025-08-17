using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmVariableApproveRemindService: AFBaseCurdRepositoryService<BpmVariableApproveRemind>,IBpmVariableApproveRemindService
{
    public BpmVariableApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }
}