using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmProcessNameRepository : RepositoryBase<BpmProcessName>, IBpmProcessNameRepository
{
    public SsBpmProcessNameRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmProcessVo> GetAllProcessVo()
    {
        return Db.Queryable<BpmProcessName>()
            .LeftJoin<BpmProcessNameRelevancy>((a, b) => b.ProcessNameId == a.Id)
            .Select((a, b) => new BpmProcessVo
            {
                ProcessName = a.ProcessName,
                ProcessKey = b.ProcessKey
            })
            .ToList();
    }
}
