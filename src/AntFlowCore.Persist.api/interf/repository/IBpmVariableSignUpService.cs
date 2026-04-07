using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpService : IBaseRepositoryService<BpmVariableSignUp>
{
    List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey);
    bool CheckNodeIsSignUp(string processNumber, string nodeId);
    bool IsMoreNode(string processNum, string elementId);
}
