using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableViewPageButtonRepository : RepositoryBase<BpmVariableViewPageButton>, IBpmVariableViewPageButtonRepository
{
    public SsBpmVariableViewPageButtonRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum)
    {
        List<BpmVariableViewPageButton> bpmVariableViewPageButtons = Db.Queryable<BpmVariableViewPageButton>()
            .LeftJoin<BpmVariable>((a, b) => a.VariableId == b.Id)
            .Where((a, b) => b.ProcessNum == processNum)
            .Select((a, b) => a)
            .ToList();
        return bpmVariableViewPageButtons;
    }
}
