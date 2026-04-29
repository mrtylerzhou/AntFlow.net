using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsDicMainRepository : RepositoryBase<DictMain>, IDicMainRepository
{
    public SsDicMainRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
