using System.Linq.Expressions;
using AntFlowCore.Core.entity;

namespace AntFlowCore.Extensions.service;

public interface ITaskService
{
    List<BpmAfTask> CreateTaskQuery(Expression<Func<BpmAfTask, bool>> filter);
    void Complete(BpmAfTask task);
}
