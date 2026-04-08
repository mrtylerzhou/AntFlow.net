using AntFlowCore.Core.entity;
using AntFlowCore.Core.interf;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpPersonnelService : IBaseRepositoryService<BpmVariableSignUpPersonnel>
{
    void InsertSignUpPersonnel(ITaskService taskService, string taskId, string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers);
}
