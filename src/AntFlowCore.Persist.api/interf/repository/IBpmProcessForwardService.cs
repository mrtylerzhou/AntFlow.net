using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessForwardService : IAntFlowRepositoryMix<BpmProcessForward, IBpmProcessForwardRepository>
{
    void AddProcessForward(BpmProcessForward bpmProcessForward);
    void UpdateProcessForward(BpmProcessForward bpmProcessForward);
    void LoadProcessForward(string userId);
    void LoadTask(string userId);
    BpmAfTask? GetTask(string processInstanceId);
    BpmProcessForward? GetProcessForward(string processInstanceId);
    bool IsForward(string recordProcessInstanceId);
}
