using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsLFMainRepository : RepositoryBase<LFMain>, ILFMainRepository
{
    public SsLFMainRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
