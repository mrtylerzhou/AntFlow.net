using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableSignUpService : IBpmVariableSignUpService
{
    public BpmVariableSignUpService(IBpmVariableSignUpRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableSignUpRepository _repository { get; }

    public List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey)
    {
        return _repository.GetSignUpPrevNodeIdsByeElementId(processNumber, taskDefinitionKey);
    }

    public bool CheckNodeIsSignUp(string processNumber, string nodeId)
    {
        return _repository.CheckNodeIsSignUp(processNumber, nodeId);
    }

    public bool IsMoreNode(string processNum, string elementId)
    {
        return _repository.IsMoreNode(processNum, elementId);
    }
}
