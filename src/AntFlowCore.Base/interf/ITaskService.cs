using System.Linq.Expressions;
using AntFlowCore.Core.entity;

namespace AntFlowCore.Core.interf;

public interface ITaskService
{
    List<BpmAfTask> CreateTaskQuery(Expression<Func<BpmAfTask, bool>> filter);
    void Complete(BpmAfTask task);
}
