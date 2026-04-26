using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmAfTaskInstRepository: RepositoryBase<BpmAfTaskInst>, IBpmAfTaskInstRepository
{
    public FsBpmAfTaskInstRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }
}
