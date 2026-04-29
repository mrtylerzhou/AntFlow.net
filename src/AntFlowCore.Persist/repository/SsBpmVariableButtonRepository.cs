using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableButtonRepository : RepositoryBase<BpmVariableButton>, IBpmVariableButtonRepository
{
    public SsBpmVariableButtonRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, List<string> elementIds)
    {
        List<BpmVariableButton> bpmVariableButtons = Db.Queryable<BpmVariableButton, BpmVariable>(
                (a, b) => new JoinQueryInfos(JoinType.Left, a.VariableId == b.Id))
            .Where((a, b) => b.ProcessNum == processNum && elementIds.Contains(a.ElementId))
            .Select((a, b) => a)
            .ToList();
        return bpmVariableButtons;
    }
}
