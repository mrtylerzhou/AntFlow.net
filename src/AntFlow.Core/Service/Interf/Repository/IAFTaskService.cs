using AntFlow.Core.Entity;

namespace AntFlow.Core.Service.Interface;

public interface IAFTaskService
{
    public List<BpmAfTask> FindTaskByEmpId(string userId);
    public void InsertTasks(List<BpmAfTask> tasks);
}