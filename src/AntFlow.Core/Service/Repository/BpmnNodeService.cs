using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeService : AFBaseCurdRepositoryService<BpmnNode>, IBpmnNodeService
{
    public BpmnNodeService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property)
    {
        List<BpmnNode> bpmnNodes = Frsql.Select<BpmnConf, BpmnNode>()
            .InnerJoin((a, b) => a.Id == b.ConfId)
            .Where((a, b) => a.FormCode == formCode && a.EffectiveStatus == 1 && b.NodeProperty == property)
            .OrderByDescending((a, b) => a.CreateTime)
            .ToList((a, b) => b);
        return bpmnNodes;
    }
}