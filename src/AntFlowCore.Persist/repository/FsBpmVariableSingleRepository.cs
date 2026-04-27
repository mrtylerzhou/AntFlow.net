using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableSingleRepository : RepositoryBase<BpmVariableSingle>, IBpmVariableSingleRepository
{
    public FsBpmVariableSingleRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void UpdateAssignee(long id, string assignee, string assigneeName, string remark)
    {
        _ormContext.FreeSql.Update<BpmVariableSingle>()
            .Set(a => a.Assignee, assignee)
            .Set(a => a.AssigneeName, assigneeName)
            .Set(a => a.Remark, remark)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
    }
}
