using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmAfTaskInstRepository: IBaseRepository<BpmAfTaskInst>
{
    int UpdateTaskDurationAndEndTime(string taskId, int durationMinutes, DateTime endTime, string deleteReason, bool isCopyNode, string assignee = null, string assigneeName = null);
    int UpdateEndTimeByProcInstId(string procInstId, DateTime endTime, string deleteReason, string assignee);
    int UpdateTaskInstByTaskId(string taskId, string deleteReason, int verifyStatus, string verifyDesc, DateTime endTime, int duration);
    int UpdateTaskInstAssignee(string taskId, string assignee, string assigneeName, string description, string updateUser);
}
