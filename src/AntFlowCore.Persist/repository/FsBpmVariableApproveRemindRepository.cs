using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableApproveRemindRepository : RepositoryBase<BpmVariableApproveRemind>, IBpmVariableApproveRemindRepository
{
    public FsBpmVariableApproveRemindRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
