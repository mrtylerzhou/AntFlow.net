using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmDynamicConditionChoosenRepository : RepositoryBase<BpmDynamicConditionChoosen>, IBpmDynamicConditionChoosenRepository
{
    public FsBpmDynamicConditionChoosenRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
