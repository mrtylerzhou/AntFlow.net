using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmProcessNodeSubmitRepository : RepositoryBase<BpmProcessNodeSubmit>, IBpmProcessNodeSubmitRepository
{
    public FsBpmProcessNodeSubmitRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmProcessNodeSubmit? FindLatestByProcessInstanceId(string processInstanceId)
    {
        return _ormContext.FreeSql.GetRepository<BpmProcessNodeSubmit>()
            .Select.Where(a => a.ProcessInstanceId.Equals(processInstanceId))
            .OrderByDescending(a => a.CreateTime)
            .First();
    }

    public void DeleteByProcessInstanceId(string processInstanceId)
    {
        _ormContext.FreeSql.GetRepository<BpmProcessNodeSubmit>()
            .Delete(a => a.ProcessInstanceId == processInstanceId);
    }
}
