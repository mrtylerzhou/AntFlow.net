using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableButtonRepository : RepositoryBase<BpmVariableButton>, IBpmVariableButtonRepository
{
    public FsBpmVariableButtonRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, List<string> elementIds)
    {
        List<BpmVariableButton> bpmVariableButtons = _ormContext.FreeSql.Select<BpmVariableButton, BpmVariable>()
            .LeftJoin((a, b) => a.VariableId == b.Id)
            .Where((a, b) => b.ProcessNum == processNum && elementIds.Contains(a.ElementId))
            .ToList();
        return bpmVariableButtons;
    }
}
