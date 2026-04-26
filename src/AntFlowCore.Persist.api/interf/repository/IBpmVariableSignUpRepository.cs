using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpRepository : IBaseRepository<BpmVariableSignUp>
{
    List<string>? GetSignUpPrevNodeIdsByeElementId(string processNumber, string taskDefinitionKey);
    bool CheckNodeIsSignUp(string processNumber, string nodeId);
    bool IsMoreNode(string processNum, string elementId);
}
