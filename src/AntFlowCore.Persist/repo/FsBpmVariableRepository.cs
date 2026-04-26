using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmVariableRepository : RepositoryBase<BpmVariable>, IBpmVariableRepository
{
    public FsBpmVariableRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmVariable FindByProcessNum(string processNumber)
    {
        return _ormContext.FreeSql.Select<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber)
            .First();
    }
}
