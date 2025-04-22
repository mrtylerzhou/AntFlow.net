using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.util;

namespace antflowcore.service.repository;

public class BpmVariableSignUpService: AFBaseCurdRepositoryService<BpmVariableSignUp>
{
    public BpmVariableSignUpService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey)
    {
        List<string> list = Frsql.Select<BpmVariable,BpmVariableSignUp>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&b.ElementId==taskDefinitionKey)
            .ToList((a,b)=>b.NodeId);
        return list;
    }

    public bool CheckNodeIsSignUp(string processNumber, string nodeId)
    {
        if (String.IsNullOrEmpty(processNumber) || String.IsNullOrEmpty(nodeId)) {
            return false;
        }

        if (IsMoreNode(processNumber, nodeId))
        {
            return false;
        }

        BpmVariable bpmVariable = Frsql.Select<BpmVariable>()
            .Where(a=>a.ProcessNum==processNumber&&a.IsDel==0)
            .First();
        if (bpmVariable == null)
        {
            return false;
        }

        long count = this.baseRepo
            .Where(a=>a.VariableId==bpmVariable.Id&&a.ElementId==nodeId)
            .Count();
        return count > 0;
    }
    private BpmVariableMultiplayer MapToPersonnel(BpmVariableMultiplayer a, BpmVariable b, BpmVariableMultiplayerPersonnel c)

    {
        BpmVariableMultiplayer multiplayer = new BpmVariableMultiplayer();
        multiplayer = a;
        multiplayer.UnderTakeStatus = c.UndertakeStatus;
        return multiplayer;
    }
    private bool IsMoreNode(String processNum, String elementId) {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Frsql.Select<BpmVariableMultiplayer, BpmVariable, BpmVariableMultiplayerPersonnel>()
            .LeftJoin((a, b, c) => a.VariableId == b.Id)
            .LeftJoin((a, b, c) => c.VariableMultiplayerId == a.Id)
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
        List<BpmVariableMultiplayer> filteredPlayers = bpmVariableMultiplayers.Where(a=>a.UnderTakeStatus==null||a.UnderTakeStatus==0).ToList();
        return filteredPlayers!=null && filteredPlayers.Count>1&&filteredPlayers[0].SignType==2;
    }
}