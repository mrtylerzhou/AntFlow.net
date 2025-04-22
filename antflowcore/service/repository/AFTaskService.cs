using antflowcore.entity;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class AFTaskService : AFBaseCurdRepositoryService<BpmAfTask>
{
    public AFTaskService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmAfTask> FindTaskByEmpId(String userId)
    {
        List<BpmAfTask> bpmAfTasks = this.baseRepo
            .Where(a => a.Assignee == userId)
            .ToList();
        return bpmAfTasks;
    }

    public void InsertTasks(List<BpmAfTask> tasks)
    {
        UserEntrustService userEntrustService = ServiceProviderUtils.GetService<UserEntrustService>();
        foreach (BpmAfTask bpmAfTask in tasks)
        {
            string assignee = bpmAfTask.Assignee;
            string assigneeName = bpmAfTask.AssigneeName;
            BaseIdTranStruVo entrustEmployee = userEntrustService.GetEntrustEmployee(assignee, assigneeName, bpmAfTask.FormKey);
            String userId = entrustEmployee.Id;
            if (!string.IsNullOrEmpty(userId) && !userId.Equals(assignee))
            {
                String userName = entrustEmployee.Name;
                bpmAfTask.Assignee = userId;
                bpmAfTask.AssigneeName = userName;

                BpmFlowrunEntrust entrust = new BpmFlowrunEntrust()
                {
                    Type = 1,
                    RunTaskId = bpmAfTask.Id,
                    Actual = userId,
                    ActualName = userName,
                    Original = assignee,
                    OriginalName = assigneeName,
                    IsRead = 2,
                    ProcDefId = bpmAfTask.FormKey,
                    RunInfoId = bpmAfTask.ProcInstId,
                };
                BpmFlowrunEntrustService bpmFlowrunEntrustService = ServiceProviderUtils.GetService<BpmFlowrunEntrustService>();
                bpmFlowrunEntrustService.baseRepo.Insert(entrust);
            }
        }

        this.baseRepo.Insert(tasks);
    }
}