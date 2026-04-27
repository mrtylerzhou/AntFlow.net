using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableSignUpRepository : RepositoryBase<BpmVariableSignUp>, IBpmVariableSignUpRepository
{
    public FsBpmVariableSignUpRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey)
    {
        List<string> list = _ormContext.FreeSql.Select<BpmVariable, BpmVariableSignUp>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefinitionKey)
            .ToList((a, b) => b.NodeId);
        return list;
    }

    public bool CheckNodeIsSignUp(string processNumber, string nodeId)
    {
        if (String.IsNullOrEmpty(processNumber) || String.IsNullOrEmpty(nodeId))
        {
            return false;
        }

        if (IsMoreNode(processNumber, nodeId))
        {
            return false;
        }

        BpmVariable bpmVariable = _ormContext.FreeSql.Select<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber && a.IsDel == 0)
            .First();
        if (bpmVariable == null)
        {
            return false;
        }

        long count = _ormContext.FreeSql.Select<BpmVariableSignUp>()
            .Where(a => a.VariableId == bpmVariable.Id && a.ElementId == nodeId)
            .Count();
        return count > 0;
    }

    public bool IsMoreNode(String processNum, String elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = _ormContext.FreeSql.Select<BpmVariableMultiplayer, BpmVariable, BpmVariableMultiplayerPersonnel>()
            .LeftJoin((a, b, c) => a.VariableId == b.Id)
            .LeftJoin((a, b, c) => c.VariableMultiplayerId == a.Id)
            .Where(a => a.t1.ElementId == elementId && a.t2.ProcessNum == processNum)
            .ToList((a, b, c) => new
            {
                Multiplayer = a,
                UndertakeStatus = c.UndertakeStatus
            })
            .Select(x =>
            {
                x.Multiplayer.UnderTakeStatus = x.UndertakeStatus;
                return x.Multiplayer;
            })
            .ToList();
        List<BpmVariableMultiplayer> filteredPlayers = bpmVariableMultiplayers.Where(a => a.UnderTakeStatus == null || a.UnderTakeStatus == 0).ToList();
        return filteredPlayers != null && filteredPlayers.Count > 1 && filteredPlayers[0].SignType == 2;
    }
}
