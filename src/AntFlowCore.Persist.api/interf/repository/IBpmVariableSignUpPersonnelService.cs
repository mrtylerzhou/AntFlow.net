using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpPersonnelService
{
    void InsertSignUpPersonnel(string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers);

    List<KeyValuePair<string, string>> GetSignUpNodeAssigneeMap(string procInstId, string elementId);
}
