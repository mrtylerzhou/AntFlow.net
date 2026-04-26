using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsLFMainRepository : RepositoryBase<LFMain>, ILFMainRepository
{
    public FsLFMainRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
