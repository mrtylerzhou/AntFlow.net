using AntFlowCore.Base.entity;
using AntFlowCore.Base.dto;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableService : IAntFlowRepositoryMix<BpmVariable, IBpmVariableRepository>
{
    List<string> GetNodeIdsByeElementId(string processNumber, string elementId);
    List<string> GetElementIdsdByNodeId(string processNumber, string nodeId);
    NodeElementDto GetNodeIdByElementId(string processNumber, string elementId);
    NodeElementDto GetElementIdByNodeId(string processNumber, string nodeId);
    List<string> GetNodeIdByElementIds(string processNumber, List<string> elementIds);
    BpmVariableMultiplayer GetCurrentMultiPlayerNode(string processNumber, string elementId, string nodeId);
    void InvalidNodeAssignees(List<string> assigneeIds, string processNumber, bool isSingle);
}
