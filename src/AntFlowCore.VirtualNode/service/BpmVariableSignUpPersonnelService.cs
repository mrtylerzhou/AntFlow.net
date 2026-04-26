using AntFlowCore.Base.entity;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableSignUpPersonnelService : IBpmVariableSignUpPersonnelService
{
    public BpmVariableSignUpPersonnelService(IBpmVariableSignUpPersonnelRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableSignUpPersonnelRepository _repository { get; }

    public void InsertSignUpPersonnel(ITaskService taskService, string taskId, string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers)
    {
        _repository.InsertSignUpPersonnel(taskService, taskId, processNumber, taskTaskDefinitionKey, assignee, signUpUsers);
    }
}
