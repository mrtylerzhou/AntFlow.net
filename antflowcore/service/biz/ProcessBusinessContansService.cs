using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;

namespace antflowcore.service.biz;

public class ProcessBusinessContansService
{
    private readonly AFExecutionService _executionService;
    private readonly AfTaskInstService _taskInstService;
    private readonly AFTaskService _taskService;

    public ProcessBusinessContansService(
        AFExecutionService executionService,
        AfTaskInstService taskInstService,
        AFTaskService taskService
    )
    {
        _executionService = executionService;
        _taskInstService = taskInstService;
        _taskService = taskService;
    }

    public void DeleteProcessInstance(string processInstanceId)
    {
        _executionService.baseRepo.Delete(a => a.ProcInstId == processInstanceId);
        _taskService.baseRepo.Delete(a => a.ProcInstId == processInstanceId);
    }

    public BpmAfTaskInst GetPrevTask(String taskDefKey, String procInstId)
    {
        if (string.IsNullOrEmpty(taskDefKey))
        {
            return null;
        }
        if (string.IsNullOrEmpty(procInstId))
        {
            throw new AFBizException("taskId不为空,流程实例Id不存在!");
        }

        List<BpmAfTaskInst> bpmAfTaskInsts = _taskInstService
            .baseRepo
            .Where(a => a.ProcInstId == procInstId)
            .OrderByDescending(a => a.StartTime)
            .ToList();
        BpmAfTaskInst bpmAfTaskInst = bpmAfTaskInsts.First(a => a.TaskDefKey != taskDefKey);
        return bpmAfTaskInst;
    }
}