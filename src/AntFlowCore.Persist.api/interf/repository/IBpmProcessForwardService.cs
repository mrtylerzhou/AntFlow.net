using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessForwardService : IBaseRepositoryService<BpmProcessForward>
{
    void AddProcessForward(BpmProcessForward bpmProcessForward);
    void UpdateProcessForward(BpmProcessForward bpmProcessForward);
    void LoadProcessForward(string userId);
    void LoadTask(string userId);
    BpmAfTask? GetTask(string processInstanceId);
    BpmProcessForward? GetProcessForward(string processInstanceId);
    bool IsForward(string recordProcessInstanceId);
}
