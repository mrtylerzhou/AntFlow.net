using AntFlowCore.Base.entity;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpPersonnelService : IAntFlowRepositoryMix<BpmVariableSignUpPersonnel, IBpmVariableSignUpPersonnelRepository>
{
    void InsertSignUpPersonnel(ITaskService taskService, string taskId, string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers);
}
