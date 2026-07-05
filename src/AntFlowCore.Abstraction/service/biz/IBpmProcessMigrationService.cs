using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

/// <summary>
/// Service for migrating a running process when dynamic conditions change.
/// Re-submits the process and replays task completions up to the current node.
/// </summary>
public interface IBpmProcessMigrationService
{
    /// <summary>
    /// Migrate the process: re-submit with the latest form data, then complete
    /// tasks up to the current task definition key, effectively "replaying"
    /// the process to the same point with the new condition branch.
    /// </summary>
    /// <param name="currentTask">the current task being approved</param>
    /// <param name="bpmBusinessProcess">the current business process</param>
    /// <param name="vo">the business data vo (contains updated form data)</param>
    /// <param name="taskCompletionAction">callback to complete each replayed task</param>
    void MigrateAndJumpToCurrent(BpmAfTask currentTask, BpmBusinessProcess bpmBusinessProcess,
        BusinessDataVo vo, Action<BusinessDataVo, BpmAfTask, BpmBusinessProcess> taskCompletionAction);
}
