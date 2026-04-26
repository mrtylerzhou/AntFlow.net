using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmFlowrunEntrustRepository : RepositoryBase<BpmFlowrunEntrust>, IBpmFlowrunEntrustRepository
{
    public FsBpmFlowrunEntrustRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmFlowrunEntrust> GetEntrustsByProcessNumber(string processNumber)
    {
        return _ormContext.FreeSql
            .Select<BpmFlowrunEntrust, BpmBusinessProcess>()
            .InnerJoin((a, b) => a.RunInfoId == b.ProcInstId)
            .Where((a, b) => b.BusinessNumber == processNumber)
            .ToList<BpmFlowrunEntrust>();
    }
}
