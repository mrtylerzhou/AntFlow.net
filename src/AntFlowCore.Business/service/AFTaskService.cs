using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class AFTaskService: IAFTaskService
{
    public AFTaskService(IAFTaskRepository repository) 
    {
        _repository = repository;
    }
    

    public List<BpmAfTask> FindTaskByEmpId(String userId)
    {
        List<BpmAfTask> bpmAfTasks = this._repository.Find(a => a.Assignee == userId);
        return bpmAfTasks;
    }

    public void InsertTasks( List<BpmAfTask> tasks)
    {
        UserEntrustService userEntrustService = ServiceProviderUtils.GetService<UserEntrustService>();
        foreach (BpmAfTask bpmAfTask in tasks)
        {
            string assignee = bpmAfTask.Assignee;
            string assigneeName = bpmAfTask.AssigneeName;
            BaseIdTranStruVo entrustEmployee = userEntrustService.GetEntrustEmployee(assignee, assigneeName,bpmAfTask.FormKey);
            String userId =entrustEmployee.Id;
            if (!string.IsNullOrEmpty(userId)&&!userId.Equals(assignee))
            {
                String userName=entrustEmployee.Name;
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
                bpmFlowrunEntrustService._repository.Add(entrust);
            }
        }

        this._repository.AddRange(tasks);
    }

    public IAFTaskRepository _repository { get; }
}