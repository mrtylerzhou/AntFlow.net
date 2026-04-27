using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableMultiplayerRepository : RepositoryBase<BpmVariableMultiplayer>, IBpmVariableMultiplayerRepository
{
    public FsBpmVariableMultiplayerRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVariableMultiplayer> QueryMultiplayersByProcessNumAndElementId(string processNum, string elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = _ormContext.FreeSql.Select<BpmVariable, BpmVariableMultiplayer>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNum && b.ElementId == elementId)
            .ToList<BpmVariableMultiplayer>();
        return bpmVariableMultiplayers;
    }
}
