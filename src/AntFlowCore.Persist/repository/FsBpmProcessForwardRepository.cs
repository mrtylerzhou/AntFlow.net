using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmProcessForwardRepository : RepositoryBase<BpmProcessForward>, IBpmProcessForwardRepository
{
    public FsBpmProcessForwardRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
