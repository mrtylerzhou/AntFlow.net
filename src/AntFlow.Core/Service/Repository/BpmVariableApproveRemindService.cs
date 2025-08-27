using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableApproveRemindService : AFBaseCurdRepositoryService<BpmVariableApproveRemind>,
    IBpmVariableApproveRemindService
{
    public BpmVariableApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }
}