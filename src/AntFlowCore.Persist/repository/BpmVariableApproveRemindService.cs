using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmVariableApproveRemindService: AFBaseCurdRepositoryService<BpmVariableApproveRemind>,IBpmVariableApproveRemindService
{
    public BpmVariableApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }
}