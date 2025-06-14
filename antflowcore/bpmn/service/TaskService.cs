using System.Linq.Expressions;
using System.Text.Json;
using antflowcore.bpmn.listener;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.bpmn.service;

public class TaskService
{
    private readonly AFTaskService _afTaskService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly AFExecutionService _afExecutionService;
    private readonly IExecutionListener _executionListener;
    private readonly BpmVariableSignUpPersonnelService _signUpPersonnelService;
    private readonly AFDeploymentService _afDeploymentService;

    public TaskService(
        AFTaskService afTaskService,
        AfTaskInstService afTaskInstService,
        AFExecutionService afExecutionService,
        IExecutionListener executionListener,
        BpmVariableSignUpPersonnelService signUpPersonnelService,
        AFDeploymentService afDeploymentService)
    {
        _afTaskService = afTaskService;
        _afTaskInstService = afTaskInstService;
        _afExecutionService = afExecutionService;
        _executionListener = executionListener;
        _signUpPersonnelService = signUpPersonnelService;
        _afDeploymentService = afDeploymentService;
    }
    public List<BpmAfTask> CreateTaskQuery(Expression<Func<BpmAfTask,bool>> filter)
    {
        List<BpmAfTask> bpmAfTasks = _afTaskService.baseRepo.Where(filter).ToList();
        return bpmAfTasks;
    }

    
    public void Complete( BpmAfTask task)
    {
       
        DateTime nowTime = DateTime.Now;
        string deleteReason = StringConstants.DEFAULT_TASK_DELETE_REASON;
        string taskId = task.Id;
        string procDefId = task.ProcDefId;
        string taskDefKey = task.TaskDefKey;
        string procInstId = task.ProcInstId;
        List<BpmAfTask> afTasks = _afTaskService.baseRepo.Where(a => a.ProcInstId == procInstId).ToList();
        BpmAfTask bpmAfTask = afTasks.First(a => a.Id == taskId);
      
        if (bpmAfTask == null)
        {
            throw new ApplicationException($"Task with id {taskId} not found");
        }
        BpmAfExecution currentExecution = _afExecutionService.baseRepo.Where(a=>a.Id==bpmAfTask.ExecutionId&&a.ActId==task.TaskDefKey).First();
        if (currentExecution == null)
        {
            throw new AFBizException("未能找到当前流程执行实例!");
        }
        int? currentSignType = currentExecution.SignType??1;
        
       
        if (currentSignType == 2||task.IsNextNodeSignUp)
        {
            _afTaskService.Frsql.Delete<BpmAfTask>()
                .Where(a => a.TaskDefKey == taskDefKey)
                .ExecuteAffrows();
        }
        else
        {
            _afTaskService.Frsql.Delete<BpmAfTask>().Where(a=>a.Id==taskId).ExecuteAffrows();
        }
        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a=>a.Id==procDefId).First();
        if (bpmAfDeployment == null)
        {
            throw new ApplicationException($"deployment with id {procDefId} not found");
        }
        
        if (currentExecution.TaskCount!.Value >= 2)
        {
            int currentCount = currentExecution.TaskCount - 1 ?? 0;
            _afExecutionService.Frsql.Update<BpmAfExecution>()
                .Set(a => a.TaskCount, currentCount)
                .Where(a => a.Id == currentExecution.Id)
                .ExecuteAffrows();
            return;
        }
        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        BpmnConfCommonElementVo elementToDeal = null;
        List<string> verifyUserIds = new List<string>();
        if (currentSignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
        {
            BpmnConfCommonElementVo currentElement = BpmnFlowUtil.GetCurrentTaskElement(elements,taskDefKey);
            if (currentElement == null)
            {
                throw new AFBizException($"can not get current element by element id: {currentElement}");
            }

            BpmVerifyInfoService bpmVerifyInfoService = ServiceProviderUtils.GetService<BpmVerifyInfoService>();
            List<BpmVerifyInfo> bpmVerifyInfos = bpmVerifyInfoService.baseRepo
                .Where(a=>a.RunInfoId==procInstId&&a.TaskDefKey==taskDefKey)
                .ToList();
            verifyUserIds= bpmVerifyInfos.Select(a=>a.VerifyUserId).ToList();
            int currentNodeAssigneesCount = currentElement.AssigneeMap.Count;
            if (verifyUserIds.Count != currentNodeAssigneesCount)
            {
                elementToDeal = currentElement;
            }
            else
            {
                var (nextUserElement, nextFlowElement) = BpmnFlowUtil.GetNextNodeAndFlowNode(elements,taskDefKey);
                elementToDeal=nextUserElement;
            }
           
        }
        else
        {
            var (nextUserElement, nextFlowElement) = BpmnFlowUtil.GetNextNodeAndFlowNode(elements,taskDefKey);
            if (nextUserElement.FlowTo == nextFlowElement.FlowTo)
            { 
                
                string flowTo = nextFlowElement.FlowTo;

                elementToDeal = elements.First(a => a.ElementId == flowTo);
            }
            else
            {
                elementToDeal=nextUserElement;
            }
           
        }

        if (elementToDeal.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code)
        {
            List<BpmAfTask> otherNodeTasks = afTasks.Where(a => a.Id != taskId).ToList();
            if(otherNodeTasks.Count>0)
            {
                return;
            }
            else
            {
                elementToDeal = BpmnFlowUtil.GetNodeFromCurrentNext(elements,elementToDeal.ElementId);
            }
        }
        IDictionary<string,string> assigneeMap = elementToDeal.AssigneeMap;
        if (elementToDeal.IsSignUpSubElement == 1)
        {
            List<KeyValuePair<string,string>> signupNodeAssigneeMap = this._signUpPersonnelService.Frsql
                .Select<BpmBusinessProcess,BpmVariable,BpmVariableSignUpPersonnel>()
                .InnerJoin((a,b,c)=>a.BusinessNumber==b.ProcessNum)
                .InnerJoin((a,b,c)=>b.Id==c.VariableId)
                .Where((a,b,c)=>a.ProcInstId==procInstId)
                .ToList<KeyValuePair<string,string>>((a,b,c)=>new KeyValuePair<string, string>(c.Assignee,c.AssigneeName));
            if (signupNodeAssigneeMap.Count <= 0)
            {
                var (nextUserElement, nextFlowElement) = BpmnFlowUtil.GetNextNodeAndFlowNode(elements, elementToDeal.ElementId);
                elementToDeal=nextUserElement;
                assigneeMap=elementToDeal.AssigneeMap;
            }
            else
            {
                assigneeMap = signupNodeAssigneeMap.ToDictionary(k => k.Key, v => v.Value);
            }
        }
        int taskCount=currentSignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode()?1:assigneeMap?.Count??0;
        BpmAfExecution execution = new BpmAfExecution
        {
            Id = currentExecution.Id,
            ProcInstId = procInstId,
            //BusinessKey = bpmnStartConditions.BusinessId, //todo注意观察此字段更新时是否会丢失
            ProcDefId = procDefId,
            ActId = elementToDeal.ElementId,
            Name = elementToDeal.ElementName,
            StartTime = nowTime,
            StartUserId = SecurityUtils.GetLogInEmpId(),
            TaskCount =taskCount,
        };
       
        _afExecutionService.baseRepo.Update(execution);
        if (elementToDeal.ElementType == ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code)
        {
             deleteReason =StringConstants.TASK_FINISH_REASON;
            _executionListener.Notify(execution,IExecutionListener.EVENTNAME_END);
        }
        TimeSpan taskDuration = nowTime-bpmAfTask.CreateTime;
        int durationMinutes = taskDuration.Minutes;
        _afTaskInstService.Frsql
            .Update<BpmAfTaskInst>()
            .Set(a=>a.Duration, durationMinutes)
            .Set(a => a.EndTime, nowTime)
            .Set(a => a.DeleteReason, deleteReason)
            .Where(a => a.Id == taskId)
            .ExecuteAffrows();
        if (deleteReason.Equals(StringConstants.TASK_FINISH_REASON))
        {
            return;
        }
        List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
        List<BpmAfTask> tasks=new List<BpmAfTask>();
        foreach (var (key, value) in assigneeMap)
        {
            if (verifyUserIds.Contains(key))
            {
                continue;
            }
            BpmAfTask newTask = new BpmAfTask()
            {
                Id = StrongUuidGenerator.GetNextId(),
                ProcInstId = procInstId,
                ProcDefId = procDefId,
                ExecutionId = currentExecution.Id,
                Name = elementToDeal.ElementName,
                TaskDefKey = elementToDeal.ElementId,
                Owner = bpmAfTask.Owner,
                Assignee = key,
                AssigneeName = value,
                CreateTime = nowTime,
                FormKey = bpmAfTask.FormKey,
            };
            tasks.Add(newTask);
        }
        _afTaskService.InsertTasks(tasks);
        foreach (BpmAfTask afTask in tasks)
        {
            BpmAfTaskInst bpmAfTaskInst = afTask.ToInst();
            historyTaskInsts.Add(bpmAfTaskInst);
        }
        _afTaskInstService.baseRepo.Insert(historyTaskInsts);
    }
}