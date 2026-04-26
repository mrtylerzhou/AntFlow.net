using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsOutSideBpmCallbackUrlConfRepository : RepositoryBase<OutSideBpmCallbackUrlConf>, IOutSideBpmCallbackUrlConfRepository
{
    public FsOutSideBpmCallbackUrlConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
