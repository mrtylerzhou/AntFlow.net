using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmBusinessProcessRepository: RepositoryBase<BpmBusinessProcess>, IBpmBusinessProcessRepository
{
    public FsBpmBusinessProcessRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void UpdateProcessDigest(string processNumber, string processDigest)
    {
        _ormContext.FreeSql.Update<BpmBusinessProcess>()
            .Set(a => a.ProcessDigest, processDigest)
            .Where(a => a.BusinessNumber.Equals(processNumber))
            .ExecuteAffrows();
    }

    public void UpdateProcessState(long id, int processState)
    {
        _ormContext.FreeSql.Update<BpmBusinessProcess>()
            .Set(a => a.ProcessState, processState)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
    }

    public void UpdateProcInstId(long id, string procInstId)
    {
        _ormContext.FreeSql.Update<BpmBusinessProcess>()
            .Set(a => a.ProcInstId, procInstId)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
    }
}
