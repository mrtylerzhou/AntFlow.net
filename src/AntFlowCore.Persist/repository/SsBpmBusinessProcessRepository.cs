using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmBusinessProcessRepository : RepositoryBase<BpmBusinessProcess>, IBpmBusinessProcessRepository
{
    public SsBpmBusinessProcessRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void UpdateProcessDigest(string processNumber, string processDigest)
    {
        Db.Updateable<BpmBusinessProcess>()
            .SetColumns(a => a.ProcessDigest == processDigest)
            .Where(a => a.BusinessNumber == processNumber)
            .ExecuteCommand();
    }

    public void UpdateProcessState(long id, int processState)
    {
        Db.Updateable<BpmBusinessProcess>()
            .SetColumns(a => a.ProcessState == processState)
            .Where(a => a.Id == id)
            .ExecuteCommand();
    }

    public void UpdateProcInstId(long id, string procInstId)
    {
        Db.Updateable<BpmBusinessProcess>()
            .SetColumns(a => a.ProcInstId == procInstId)
            .Where(a => a.Id == id)
            .ExecuteCommand();
    }

    public void UpdateDto(BpmBusinessProcess bpmBusinessProcess)
    {
        Db.Updateable(bpmBusinessProcess)
            .Where(a => a.BusinessNumber == bpmBusinessProcess.BusinessNumber)
            .ExecuteCommand();
    }
}
