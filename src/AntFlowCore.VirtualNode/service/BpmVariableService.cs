using AntFlowCore.Base.entity;
using AntFlowCore.Base.dto;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableService: IBpmVariableService
{
    public BpmVariableService(IBpmVariableRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableRepository _repository { get; }

    public List<string> GetNodeIdsByeElementId(string processNumber, string elementId)
    {
        return _repository.GetNodeIdsByeElementId(processNumber, elementId);
    }

    public List<string> GetElementIdsdByNodeId(string processNumber, string nodeId)
    {
        return _repository.GetElementIdsdByNodeId(processNumber, nodeId);
    }

    public NodeElementDto GetNodeIdByElementId(string processNumber, string elementId)
    {
        return _repository.GetNodeIdByElementId(processNumber, elementId);
    }

    public NodeElementDto GetElementIdByNodeId(string processNumber, string nodeId)
    {
        return _repository.GetElementIdByNodeId(processNumber, nodeId);
    }

    public List<string> GetNodeIdByElementIds(string processNumber, List<string> elementIds)
    {
        return _repository.GetNodeIdByElementIds(processNumber, elementIds);
    }

    public BpmVariableMultiplayer GetCurrentMultiPlayerNode(string processNumber, string elementId, string nodeId)
    {
        return _repository.GetCurrentMultiPlayerNode(processNumber, elementId, nodeId);
    }

    public void InvalidNodeAssignees(List<string> assigneeIds, string processNumber, bool isSingle)
    {
        _repository.InvalidNodeAssignees(assigneeIds, processNumber, isSingle);
    }
}