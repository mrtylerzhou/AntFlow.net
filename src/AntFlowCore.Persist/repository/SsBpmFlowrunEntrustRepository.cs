using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmFlowrunEntrustRepository : RepositoryBase<BpmFlowrunEntrust>, IBpmFlowrunEntrustRepository
{
    public SsBpmFlowrunEntrustRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmFlowrunEntrust> GetEntrustsByProcessNumber(string processNumber)
    {
        return Db.Queryable<BpmFlowrunEntrust>()
            .InnerJoin<BpmBusinessProcess>((a, b) => a.RunInfoId == b.ProcInstId)
            .Where((a, b) => b.BusinessNumber == processNumber)
            .ToList();
    }
}
