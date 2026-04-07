using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.service;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableSignUpPersonnelService : IBaseRepositoryService<BpmVariableSignUpPersonnel>
{
    void InsertSignUpPersonnel(ITaskService taskService, string taskId, string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers);
}
