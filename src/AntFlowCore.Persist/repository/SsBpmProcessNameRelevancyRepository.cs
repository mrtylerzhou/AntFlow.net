using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmProcessNameRelevancyRepository : RepositoryBase<BpmProcessNameRelevancy>, IBpmProcessNameRelevancyRepository
{
    public SsBpmProcessNameRelevancyRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<string> GetProcessKeysByProcessNameId(long id)
    {
        return Db.Queryable<BpmProcessNameRelevancy>()
            .Where(a => a.ProcessNameId == id && a.IsDel == 0)
            .Select(a => a.ProcessKey)
            .ToList();
    }
}
