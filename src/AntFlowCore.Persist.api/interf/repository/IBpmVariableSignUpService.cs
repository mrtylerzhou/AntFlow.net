using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpService : IAntFlowRepositoryMix<BpmVariableSignUp, IBpmVariableSignUpRepository>
{
    List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey);
    bool CheckNodeIsSignUp(string processNumber, string nodeId);
    bool IsMoreNode(string processNum, string elementId);
}
