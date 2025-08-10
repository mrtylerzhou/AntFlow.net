using antflowcore.entity;

namespace antflowcore.service.interf;

public interface IAFTaskService
{
    public List<BpmAfTask> FindTaskByEmpId(String userId);
    public void InsertTasks(List<BpmAfTask> tasks);
}