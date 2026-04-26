using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsDicMainRepository : RepositoryBase<DictMain>, IDicMainRepository
{
    public FsDicMainRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
