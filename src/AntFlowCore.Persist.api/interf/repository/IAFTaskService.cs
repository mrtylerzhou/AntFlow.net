using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAFTaskService : IAntFlowRepositoryMix<BpmAfTask,IAFTaskRepository>
{
    public List<BpmAfTask> FindTaskByEmpId(String userId);
    public void InsertTasks(List<BpmAfTask> tasks);
}
