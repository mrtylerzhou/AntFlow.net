using AntFlowCore.Entities;
using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableService: AFBaseCurdRepositoryService<BpmVariable>
{
    public BpmVariableService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<string> GetNodeIdsByeElementId(string processNumber, string elementId)
    {
        List<string> nodeIds = Frsql.Select<BpmVariable, BpmVariableSingle>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == elementId)
            .WithTempQuery((a, b) => b.NodeId)

            .UnionAll(
                Frsql.Select<BpmVariable, BpmVariableMultiplayer>()
                    .InnerJoin((a, b) => a.Id == b.VariableId)
                    .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == elementId)
                    .WithTempQuery((a, b) => b.NodeId)
            )
            .ToList();
        return nodeIds;
    }

    public List<string> GetElementIdsdByNodeId(string processNumber, string nodeId)
    {
        List<string> elementIds = Frsql.Select<BpmVariable, BpmVariableSingle>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .WithTempQuery((a, b) => b.ElementId)
            .UnionAll(
                Frsql.Select<BpmVariable, BpmVariableMultiplayer>()
                    .InnerJoin((a, b) => a.Id == b.VariableId)
                    .Where((a, b) => a.ProcessNum == processNumber && b.NodeId == nodeId)
                    .WithTempQuery((a, b) => b.ElementId)
            )
            .ToList();
        return elementIds;
    }
}