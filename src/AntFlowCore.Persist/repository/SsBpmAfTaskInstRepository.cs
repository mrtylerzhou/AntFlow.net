using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmAfTaskInstRepository : RepositoryBase<BpmAfTaskInst>, IBpmAfTaskInstRepository
{
    public SsBpmAfTaskInstRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }

    public int UpdateTaskDurationAndEndTime(string taskId, int durationMinutes, DateTime endTime, string deleteReason, bool isCopyNode, string assignee = null, string assigneeName = null)
    {
        var update = Db.Updateable<BpmAfTaskInst>()
            .SetColumns(a => a.Duration == durationMinutes)
            .SetColumns(a => a.EndTime == endTime)
            .SetColumns(a => a.DeleteReason == deleteReason);
        
        if (isCopyNode)
        {
            update.SetColumns(a => a.Assignee == assignee)
                  .SetColumns(a => a.AssigneeName == assigneeName);
        }

        int affrows = update.Where(a => a.Id == taskId)
            .ExecuteCommand();
        return affrows;
    }

    public int UpdateEndTimeByProcInstId(string procInstId, DateTime endTime, string deleteReason, string assignee)
    {
        int executeAffrows = Db.Updateable<BpmAfTaskInst>()
            .SetColumns(a => a.EndTime == endTime)
            .SetColumns(a => a.DeleteReason == deleteReason)
            .Where(a => a.ProcInstId == procInstId && a.EndTime == null && a.Assignee == assignee)
            .ExecuteCommand();
        return executeAffrows;
    }

    public int UpdateTaskInstByTaskId(string taskId, string deleteReason, int verifyStatus, string verifyDesc, DateTime endTime, int duration)
    {
        int executeAffrows = Db.Updateable<BpmAfTaskInst>()
            .SetColumns(a => a.DeleteReason == deleteReason)
            .SetColumns(a => a.VerifyStatus == verifyStatus)
            .SetColumns(a => a.VerifyDesc == verifyDesc)
            .SetColumns(a => a.EndTime == endTime)
            .SetColumns(a => a.Duration == duration)
            .Where(a => a.Id == taskId)
            .ExecuteCommand();
        return executeAffrows;
    }

    public int UpdateTaskInstAssignee(string taskId, string assignee, string assigneeName, string description, string updateUser)
    {
        int executeAffrows = Db.Updateable<BpmAfTaskInst>()
            .SetColumns(a => a.Assignee == assignee)
            .SetColumns(a => a.AssigneeName == assigneeName)
            .SetColumns(a => a.Description == description)
            .SetColumns(a => a.UpdateUser == updateUser)
            .Where(a => a.Id == taskId)
            .ExecuteCommand();
        return executeAffrows;
    }
}
