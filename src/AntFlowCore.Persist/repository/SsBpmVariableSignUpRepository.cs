using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableSignUpRepository : RepositoryBase<BpmVariableSignUp>, IBpmVariableSignUpRepository
{
    public SsBpmVariableSignUpRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey)
    {
        List<string> list = Db.Queryable<BpmVariableSignUp>()
            .InnerJoin<BpmVariable>((b, a) => a.Id == b.VariableId)
            .Where((b, a) => a.ProcessNum == processNumber && b.ElementId == taskDefinitionKey)
            .Select((b, a) => b.NodeId)
            .ToList();
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

        BpmVariable bpmVariable = Db.Queryable<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber && a.IsDel == 0)
            .First();
        if (bpmVariable == null)
        {
            return false;
        }

        long count = Db.Queryable<BpmVariableSignUp>()
            .Where(a => a.VariableId == bpmVariable.Id && a.ElementId == nodeId)
            .Count();
        return count > 0;
    }

    public bool IsMoreNode(String processNum, String elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Db.Queryable<BpmVariableMultiplayer>()
            .LeftJoin<BpmVariable>((a, b) => a.VariableId == b.Id)
            .LeftJoin<BpmVariableMultiplayerPersonnel>((a, b, c) => c.VariableMultiplayerId == a.Id)
            .Where((a, b, c) => a.ElementId == elementId && b.ProcessNum == processNum)
            .Select((a, b, c) => new
            {
                Multiplayer = a,
                UndertakeStatus = c.UndertakeStatus
            })
            .ToList()
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
