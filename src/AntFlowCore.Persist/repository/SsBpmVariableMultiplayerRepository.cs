using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableMultiplayerRepository : RepositoryBase<BpmVariableMultiplayer>, IBpmVariableMultiplayerRepository
{
    public SsBpmVariableMultiplayerRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVariableMultiplayer> QueryMultiplayersByProcessNumAndElementId(string processNum, string elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNum && b.ElementId == elementId)
            .Select((a, b) => b)
            .ToList();
        return bpmVariableMultiplayers;
    }
}
