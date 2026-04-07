using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmVariableApproveRemindService: AFBaseCurdRepositoryService<BpmVariableApproveRemind>,IBpmVariableApproveRemindService
{
    public BpmVariableApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }
}