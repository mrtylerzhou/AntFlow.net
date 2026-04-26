using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.interf;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmProcessNodeSubmitService : IBpmProcessNodeSubmitService
{
    private readonly ITaskService _taskService;
    private readonly IProcessNodeJumpService _processNodeJumpService;
    private readonly AFDeploymentService _afDeploymentService;

    public BpmProcessNodeSubmitService(
        ITaskService taskService,
        IProcessNodeJumpService processNodeJumpService,
        AFDeploymentService afDeploymentService,
        IBpmProcessNodeSubmitRepository repository)
    {
        _taskService = taskService;
        _processNodeJumpService = processNodeJumpService;
        _afDeploymentService = afDeploymentService;
        _repository = repository;
    }

    public IBpmProcessNodeSubmitRepository _repository { get; }

    public BpmProcessNodeSubmit FindBpmProcessNodeSubmit(String processInstanceId)
    {
        return _repository.FindLatestByProcessInstanceId(processInstanceId);
    }

    public bool AddProcessNode(BpmProcessNodeSubmit processNodeSubmit)
    {
        _repository.DeleteByProcessInstanceId(processNodeSubmit.ProcessInstanceId);
        _repository.Add(processNodeSubmit);
        return true;
    }
}
