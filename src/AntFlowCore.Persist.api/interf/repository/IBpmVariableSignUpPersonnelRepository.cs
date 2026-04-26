using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpPersonnelRepository : IBaseRepository<BpmVariableSignUpPersonnel>
{
    void InsertSignUpPersonnel(ITaskService taskService, string taskId, string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers);
    List<KeyValuePair<string, string>> GetSignUpNodeAssigneeMap(string procInstId, string elementId);
}
