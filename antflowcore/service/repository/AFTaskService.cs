using antflowcore.entity;

namespace antflowcore.service.repository;

public class AFTaskService: AFBaseCurdRepositoryService<BpmAfTask>
{
    public AFTaskService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public List<BpmAfTask> FindTaskByEmpId(String userId)
    {
        List<BpmAfTask> bpmAfTasks = this.baseRepo
            .Where(a=>a.Assignee==userId)
            .ToList();
        return bpmAfTasks;
    }
}