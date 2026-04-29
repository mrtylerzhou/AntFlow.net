using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsLFMainFieldRepository : RepositoryBase<LFMainField>, ILFMainFieldRepository
{
    public SsLFMainFieldRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
