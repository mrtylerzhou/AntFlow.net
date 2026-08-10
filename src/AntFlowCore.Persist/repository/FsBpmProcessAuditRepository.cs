using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmProcessAuditRepository : RepositoryBase<BpmProcessAudit>, IBpmProcessAuditRepository
{
    public FsBpmProcessAuditRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
