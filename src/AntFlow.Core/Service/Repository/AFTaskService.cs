using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class AFTaskService : AFBaseCurdRepositoryService<BpmAfTask>, IAFTaskService
{
    public AFTaskService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmAfTask> FindTaskByEmpId(string userId)
    {
        List<BpmAfTask> bpmAfTasks = baseRepo
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
            BaseIdTranStruVo entrustEmployee =
                userEntrustService.GetEntrustEmployee(assignee, assigneeName, bpmAfTask.FormKey);
            string userId = entrustEmployee.Id;
            if (!string.IsNullOrEmpty(userId) && !userId.Equals(assignee))
            {
                string userName = entrustEmployee.Name;
                bpmAfTask.Assignee = userId;
                bpmAfTask.AssigneeName = userName;

                BpmFlowrunEntrust entrust = new()
                {
                    Type = 1,
                    RunTaskId = bpmAfTask.Id,
                    Actual = userId,
                    ActualName = userName,
                    Original = assignee,
                    OriginalName = assigneeName,
                    IsRead = 2,
                    ProcDefId = bpmAfTask.FormKey,
                    RunInfoId = bpmAfTask.ProcInstId
                };
                BpmFlowrunEntrustService bpmFlowrunEntrustService =
                    ServiceProviderUtils.GetService<BpmFlowrunEntrustService>();
                bpmFlowrunEntrustService.baseRepo.Insert(entrust);
            }
        }

        baseRepo.Insert(tasks);
    }
}