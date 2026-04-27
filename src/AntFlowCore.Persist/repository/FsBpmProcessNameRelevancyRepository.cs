using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmProcessNameRelevancyRepository : RepositoryBase<BpmProcessNameRelevancy>, IBpmProcessNameRelevancyRepository
{
    public FsBpmProcessNameRelevancyRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<string> GetProcessKeysByProcessNameId(long id)
    {
        return _ormContext.FreeSql.GetRepository<BpmProcessNameRelevancy>()
            .Select.Where(a => a.ProcessNameId == id && a.IsDel == 0)
            .ToList(a => a.ProcessKey);
    }
}
