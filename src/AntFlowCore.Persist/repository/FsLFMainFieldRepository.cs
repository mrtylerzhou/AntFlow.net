using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsLFMainFieldRepository : RepositoryBase<LFMainField>, ILFMainFieldRepository
{
    public FsLFMainFieldRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
