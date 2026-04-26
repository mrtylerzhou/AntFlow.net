using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmProcessNameRepository : RepositoryBase<BpmProcessName>, IBpmProcessNameRepository
{
    public FsBpmProcessNameRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmProcessVo> GetAllProcessVo()
    {
        return _ormContext.FreeSql
            .Select<BpmProcessName, BpmProcessNameRelevancy>()
            .LeftJoin((a, b) => b.ProcessNameId == a.Id)
            .ToList<BpmProcessVo>((b, s) => new BpmProcessVo()
            {
                ProcessName = b.ProcessName,
                ProcessKey = s.ProcessKey
            });
    }
}
