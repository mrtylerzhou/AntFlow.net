using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmAfTaskInstRepository: RepositoryBase<BpmAfTaskInst>, IBpmAfTaskInstRepository
{
    public FsBpmAfTaskInstRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }

    public int UpdateTaskDurationAndEndTime(string taskId, int durationMinutes, DateTime endTime, string deleteReason, bool isCopyNode, string assignee = null, string assigneeName = null)
    {
        var update = _ormContext.FreeSql.Update<BpmAfTaskInst>()
            .Set(a => a.Duration, durationMinutes)
            .Set(a => a.EndTime, endTime)
            .Set(a => a.DeleteReason, deleteReason);
        if (isCopyNode)
        {
            update.Set(a => a.Assignee, assignee)
                  .Set(a => a.AssigneeName, assigneeName);
        }

        int affrows = update.Where(a => a.Id == taskId)
            .ExecuteAffrows();
        return affrows;
    }

    public int UpdateEndTimeByProcInstId(string procInstId, DateTime endTime, string deleteReason, string assignee)
    {
        int executeAffrows = _ormContext.FreeSql.Update<BpmAfTaskInst>()
            .Set(a => a.EndTime, endTime)
            .Set(a => a.DeleteReason, deleteReason)
            .Where(a => a.ProcInstId == procInstId && a.EndTime == null && a.Assignee == assignee)
            .ExecuteAffrows();
        return executeAffrows;
    }

    public int UpdateTaskInstByTaskId(string taskId, string deleteReason, int verifyStatus, string verifyDesc, DateTime endTime, int duration)
    {
        int executeAffrows = _ormContext.FreeSql.Update<BpmAfTaskInst>()
            .Set(a => a.DeleteReason, deleteReason)
            .Set(a => a.VerifyStatus, verifyStatus)
            .Set(a => a.VerifyDesc, verifyDesc)
            .Set(a => a.EndTime, endTime)
            .Set(a => a.Duration, duration)
            .Where(a => a.Id == taskId)
            .ExecuteAffrows();
        return executeAffrows;
    }

    public int UpdateTaskInstAssignee(string taskId, string assignee, string assigneeName, string description, string updateUser)
    {
        int executeAffrows = _ormContext.FreeSql.Update<BpmAfTaskInst>()
            .Set(a => a.Assignee, assignee)
            .Set(a => a.AssigneeName, assigneeName)
            .Set(a => a.Description, description)
            .Set(a => a.UpdateUser, updateUser)
            .Where(a => a.Id == taskId)
            .ExecuteAffrows();
        return executeAffrows;
    }
}
