using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsOutSideBpmCallbackUrlConfRepository : RepositoryBase<OutSideBpmCallbackUrlConf>, IOutSideBpmCallbackUrlConfRepository
{
    public SsOutSideBpmCallbackUrlConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
