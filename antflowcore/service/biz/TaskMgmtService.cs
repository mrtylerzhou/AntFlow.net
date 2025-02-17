using System.Collections;
using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class TaskMgmtService
{
    private readonly AFTaskService _taskService;
    private readonly AfTaskInstService _taskInstService;

    public TaskMgmtService(
        AFTaskService taskService,
        AfTaskInstService taskInstService
        )
    {
        _taskService = taskService;
        _taskInstService = taskInstService;
    }

    /// <summary>
    /// modify current node's history assignee
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    public int UpdateTaskInst(TaskMgmtVO taskMgmtVO)
    {
        int affrows = _taskInstService
            .Frsql
            .Update<BpmAfTaskInst>()
            .Set(a => a.Assignee, taskMgmtVO.ApplyUser)
            .Set(a => a.AssigneeName, taskMgmtVO.ApplyUserName)
            .Set(a=>a.Description,"变更处理人")
            .Set(a=>a.UpdateUser,SecurityUtils.GetLogInEmpId())
            .Where(a => a.Id == taskMgmtVO.TaskId)
            .ExecuteAffrows();
        return affrows;
    }

    /// <summary>
    /// modify current assignee
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    public int UpdateTask(TaskMgmtVO taskMgmtVO)
    {
        int affrows = _taskService
            .Frsql
            .Update<BpmAfTask>()
            .Set(a => a.Assignee, taskMgmtVO.ApplyUser)
            .Set(a => a.AssigneeName, taskMgmtVO.ApplyUserName)
            .Where(a => a.Id == taskMgmtVO.TaskId)
            .ExecuteAffrows();
       
        return affrows;
    }

    public List<BpmAfTask> GetAgencyList(string taskId, int code, string taskProcInstId)
    {
        IEnumerable<string> taskDefKeys = _taskService.baseRepo.Where(a=>a.Id==taskId).ToList().Select(a=>a.TaskDefKey);
        List<BpmAfTask> bpmAfTasks = _taskService.baseRepo.Where(a=>taskDefKeys.Contains(a.TaskDefKey)&&a.ProcInstId==taskProcInstId).ToList();
        List<BpmAfTask> afTasks = bpmAfTasks.Where(a=>a.TaskDefKey!=taskId).ToList();
        return afTasks;
    }

    public void DeleteTask(string taskId)
    {
       _taskService.baseRepo.Delete(a=>a.Id==taskId);
    }
}