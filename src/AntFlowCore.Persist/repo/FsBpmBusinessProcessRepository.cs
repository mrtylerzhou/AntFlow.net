using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmBusinessProcessRepository: RepositoryBase<BpmBusinessProcess>, IBpmBusinessProcessRepository
{
    public FsBpmBusinessProcessRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
