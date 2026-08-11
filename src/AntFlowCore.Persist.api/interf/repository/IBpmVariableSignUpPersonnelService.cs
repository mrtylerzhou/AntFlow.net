using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpPersonnelService
{
    void InsertSignUpPersonnel(string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers);

    /// <summary>
    /// 自动加批场景重载: 回路(加批后回到审批人) personnel 名称用传入的 assigneeName, 而非登录名(自动场景无登录用户).
    /// </summary>
    void InsertSignUpPersonnel(string processNumber, string taskTaskDefinitionKey, string assignee, string assigneeName, List<BaseIdTranStruVo> signUpUsers);

    /// <summary>
    /// 幂等检查: 该节点(elementId)的 signUp personnel 是否已非空(已加批过). 用于条件自动加批防止回到审批人后重复触发.
    /// </summary>
    bool HasSignUpPersonnel(string processNumber, string elementId);

    List<KeyValuePair<string, string>> GetSignUpNodeAssigneeMap(string procInstId, string elementId);
}
