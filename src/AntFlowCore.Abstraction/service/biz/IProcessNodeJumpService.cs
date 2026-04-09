using AntFlowCore.Base.entity;

namespace AntFlowCore.Abstraction.service.biz;

public interface IProcessNodeJumpService
{
    void CommitProcess(BpmAfTask task, Dictionary<string, object> variables, string backNodeKey);
    void TurnTransition(string taskId, string taskToTurnToNodeKey, Dictionary<string, object> variables);
    void TurnTransition(BpmAfTask bpmAfTask, string taskToTurnToNodeKey, BpmAfDeployment? bpmAfDeployment, Dictionary<string, object> variables);
}
