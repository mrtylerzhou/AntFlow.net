using System.Linq.Expressions;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Base.interf;

public interface ITaskService
{
    List<BpmAfTask> CreateTaskQuery(Expression<Func<BpmAfTask, bool>> filter);
    void Complete(BpmAfTask task);
}
