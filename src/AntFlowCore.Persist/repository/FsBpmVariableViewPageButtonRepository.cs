using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableViewPageButtonRepository : RepositoryBase<BpmVariableViewPageButton>, IBpmVariableViewPageButtonRepository
{
    public FsBpmVariableViewPageButtonRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum)
    {
        List<BpmVariableViewPageButton> bpmVariableViewPageButtons = _ormContext.FreeSql.Select<BpmVariableViewPageButton, BpmVariable>()
            .LeftJoin((a, b) => a.VariableId == b.Id)
            .Where((a, b) => b.ProcessNum == processNum)
            .ToList();
        return bpmVariableViewPageButtons;
    }
}
