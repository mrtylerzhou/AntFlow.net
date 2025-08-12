using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeService : AFBaseCurdRepositoryService<BpmnNode>,IBpmnNodeService
{
    public BpmnNodeService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property)
    {
        List<BpmnNode> bpmnNodes = this.Frsql.Select<BpmnConf, BpmnNode>()
            .InnerJoin((a, b) => a.Id == b.ConfId)
            .Where((a, b) => a.FormCode == formCode && a.EffectiveStatus == 1 && b.NodeProperty == property)
            .OrderByDescending((a, b) => a.CreateTime)
            .ToList((a,b)=>b);
        return bpmnNodes;
    }
}