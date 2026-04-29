using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableSingleRepository : RepositoryBase<BpmVariableSingle>, IBpmVariableSingleRepository
{
    public SsBpmVariableSingleRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void UpdateAssignee(long id, string assignee, string assigneeName, string remark)
    {
        Db.Updateable<BpmVariableSingle>()
            .SetColumns(a => a.Assignee == assignee)
            .SetColumns(a => a.AssigneeName == assigneeName)
            .SetColumns(a => a.Remark == remark)
            .Where(a => a.Id == id)
            .ExecuteCommand();
    }
}
