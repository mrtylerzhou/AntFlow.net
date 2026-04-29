using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmProcessNodeSubmitRepository : RepositoryBase<BpmProcessNodeSubmit>, IBpmProcessNodeSubmitRepository
{
    public SsBpmProcessNodeSubmitRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmProcessNodeSubmit? FindLatestByProcessInstanceId(string processInstanceId)
    {
        return Db.Queryable<BpmProcessNodeSubmit>()
            .Where(a => a.ProcessInstanceId.Equals(processInstanceId))
            .OrderByDescending(a => a.CreateTime)
            .First();
    }

    public void DeleteByProcessInstanceId(string processInstanceId)
    {
        Db.Deleteable<BpmProcessNodeSubmit>()
            .Where(a => a.ProcessInstanceId == processInstanceId)
            .ExecuteCommand();
    }
}
