using AntFlowCore.Core.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAFTaskService : IBaseRepositoryService<BpmAfTask>
{
    public List<BpmAfTask> FindTaskByEmpId(String userId);
    public void InsertTasks(List<BpmAfTask> tasks);
}
