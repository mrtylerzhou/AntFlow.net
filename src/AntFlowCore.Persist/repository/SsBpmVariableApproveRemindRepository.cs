using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableApproveRemindRepository : RepositoryBase<BpmVariableApproveRemind>, IBpmVariableApproveRemindRepository
{
    public SsBpmVariableApproveRemindRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
