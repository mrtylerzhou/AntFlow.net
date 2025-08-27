using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableSignUpService : AFBaseCurdRepositoryService<BpmVariableSignUp>, IBpmVariableSignUpService
{
    public BpmVariableSignUpService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey)
    {
        List<string> list = Frsql.Select<BpmVariable, BpmVariableSignUp>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefinitionKey)
            .ToList((a, b) => b.NodeId);
        return list;
    }

    public bool CheckNodeIsSignUp(string processNumber, string nodeId)
    {
        if (string.IsNullOrEmpty(processNumber) || string.IsNullOrEmpty(nodeId))
        {
            return false;
        }

        if (IsMoreNode(processNumber, nodeId))
        {
            return false;
        }

        BpmVariable bpmVariable = Frsql.Select<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber && a.IsDel == 0)
            .First();
        if (bpmVariable == null)
        {
            return false;
        }

        long count = baseRepo
            .Where(a => a.VariableId == bpmVariable.Id && a.ElementId == nodeId)
            .Count();
        return count > 0;
    }

    private BpmVariableMultiplayer MapToPersonnel(BpmVariableMultiplayer a, BpmVariable b,
        BpmVariableMultiplayerPersonnel c)

    {
        BpmVariableMultiplayer multiplayer = new();
        multiplayer = a;
        multiplayer.UnderTakeStatus = c.UndertakeStatus;
        return multiplayer;
    }

    public bool IsMoreNode(string processNum, string elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Frsql
            .Select<BpmVariableMultiplayer, BpmVariable, BpmVariableMultiplayerPersonnel>()
            .LeftJoin((a, b, c) => a.VariableId == b.Id)
            .LeftJoin((a, b, c) => c.VariableMultiplayerId == a.Id)
            .Where(a => a.t1.ElementId == elementId && a.t2.ProcessNum == processNum)
            .ToList((a, b, c) => new { Multiplayer = a, c.UndertakeStatus })
            .Select(x =>
            {
                x.Multiplayer.UnderTakeStatus = x.UndertakeStatus;
                return x.Multiplayer;
            })
            .ToList();
        List<BpmVariableMultiplayer> filteredPlayers = bpmVariableMultiplayers
            .Where(a => a.UnderTakeStatus == null || a.UnderTakeStatus == 0).ToList();
        return filteredPlayers != null && filteredPlayers.Count > 1 && filteredPlayers[0].SignType == 2;
    }
}