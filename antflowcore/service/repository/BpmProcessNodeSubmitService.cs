using antflowcore.bpmn;
using antflowcore.bpmn.service;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.util;

namespace antflowcore.service.repository;

public class BpmProcessNodeSubmitService: AFBaseCurdRepositoryService<BpmProcessNodeSubmit>
{
   
    private readonly TaskService _taskService;

    public BpmProcessNodeSubmitService(
        TaskService taskService,
        IFreeSql freeSql) : base(freeSql)
    {
       
        _taskService = taskService;
    }

    public void ProcessComplete(BpmAfTask task)
    {
        BpmProcessNodeSubmit processNodeSubmit = this.FindBpmProcessNodeSubmit(task.ProcDefId);
        if (processNodeSubmit != null)
        {
            this.AddProcessNode(new BpmProcessNodeSubmit
            {
                State = 0,
                NodeKey = task.TaskDefKey,
                ProcessInstanceId = task.ProcInstId,
                BackType = 0,
                CreateUser = SecurityUtils.GetLogInEmpName()
            });
            if (processNodeSubmit.State==0) {
               //todo complete task
            }
            else
            {
                switch (processNodeSubmit.BackType)
                {
                    case 1:
                    case 2:
                    case 3:
                        //todo
                        break;
                    case 5:
                        //todo
                        break;
                    default:
                       //todo
                        break;
                }
            }
        }
        else
        {
            _taskService.Complete(task.Id);
        }
    }

    public BpmProcessNodeSubmit FindBpmProcessNodeSubmit(String processInstanceId)
    {
        BpmProcessNodeSubmit bpmProcessNodeSubmit = this
            .baseRepo
            .Where(a=>a.ProcessInstanceId.Equals(processInstanceId)).OrderByDescending(a=>a.CreateTime)
            .First();
        return bpmProcessNodeSubmit;
    }
    public bool AddProcessNode(BpmProcessNodeSubmit processNodeSubmit) {
        this.baseRepo.Delete(a => a.ProcessInstanceId == processNodeSubmit.ProcessInstanceId);
        this.baseRepo.Insert(processNodeSubmit);
        return true;
    }
}