using AntFlowCore.Entity;

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
}