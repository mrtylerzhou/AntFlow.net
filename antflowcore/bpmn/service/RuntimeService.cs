﻿using antflowcore.bpmn.listener;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.bpmn.service;

public class RuntimeService
{
     private readonly AfTaskInstService _taskInstService;
     private readonly AFTaskService _taskService;
     private readonly ITaskListener _taskListener;
     private readonly AFExecutionService _executionService;

     public RuntimeService(
          AfTaskInstService taskInstService, AFTaskService taskService, ITaskListener taskListener, 
          AFExecutionService executionService)
     {
          _taskInstService = taskInstService; _taskService = taskService;
          _taskListener = taskListener; _executionService = executionService;
     }

     public ExecutionEntity StartProcessInstance(BpmnConfCommonVo bpmnConfCommonVo,
          BpmnStartConditionsVo bpmnStartConditions, string deploymentId)
     {
          List<BpmnConfCommonElementVo> bpmnConfCommonElementVos = bpmnConfCommonVo.ElementList;
          string procInstId = StrongUuidGenerator.GetNextId();
         
          List<BpmnConfCommonElementVo> firstAssigneeNodes =
               BpmnFlowUtil.GetFirstAssigneeNodes(bpmnConfCommonElementVos);
          IDictionary<string, IDictionary<string, string>> node2AssigneeMap =
               new Dictionary<string, IDictionary<string, string>>();
          List < BpmAfExecution > executions = new List<BpmAfExecution>();
          for (var i = 0; i < firstAssigneeNodes.Count; i++)
          {
               BpmnConfCommonElementVo firstAssigneeNode = firstAssigneeNodes[i]; 
                 IDictionary<string, string> assigneeMap =firstAssigneeNode.AssigneeMap;
               string executionId = StrongUuidGenerator.GetNextId();
               DateTime nowTime = DateTime.Now;
               int signType = firstAssigneeNode.SignType;
               int taskCount =
                    signType == SignTypeEnum.SIGN_TYPE_OR_SIGN.GetCode() ||
                    signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode()
                         ? 1
                         : assigneeMap.Count;
               BpmAfExecution execution = new BpmAfExecution
               {
                    Id = executionId,
                    ProcInstId = procInstId,
                    BusinessKey = bpmnConfCommonVo.FormCode,
                    ProcDefId = deploymentId,
                    ActId = firstAssigneeNode.ElementId,
                    Name = firstAssigneeNode.ElementName,
                    StartTime = nowTime,
                    StartUserId = SecurityUtils.GetLogInEmpId(),
                    TaskCount = taskCount,
                    SignType = signType
               };
               executions.Add(execution);
               _executionService.baseRepo.Insert(execution);
               List<BpmAfTask> tasks = new List<BpmAfTask>();
               List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
               if (i == 0)
               {
                    BpmAfTaskInst startTask = new BpmAfTaskInst
                    {
                         Id = StrongUuidGenerator.GetNextId(),
                         ProcInstId = procInstId,
                         ProcDefId = deploymentId,
                         ExecutionId = executionId,
                         Name = StringConstants.START_USER_NODE_NAME,
                         TaskDefKey = ProcessNodeEnum.START_TASK_KEY.Description,
                         Owner = bpmnStartConditions.StartUserId,
                         Assignee = bpmnStartConditions.StartUserId,
                         AssigneeName = bpmnStartConditions.StartUserName,
                         StartTime = nowTime,
                         EndTime = nowTime,
                         Priority = 0,
                         DeleteReason = "发起人节点自动完成",
                         FormKey = bpmnConfCommonVo.FormCode,
                    };
                    historyTaskInsts.Add(startTask);
               }
               foreach (var (key, value) in assigneeMap)
               {
                    BpmAfTask bpmAfTask = new BpmAfTask()
                    {
                         Id = StrongUuidGenerator.GetNextId(),
                         ProcInstId = procInstId,
                         ProcDefId = deploymentId,
                         ExecutionId = executionId,
                         Name = firstAssigneeNode.ElementName,
                         TaskDefKey = firstAssigneeNode.ElementId,
                         Owner = bpmnStartConditions.StartUserId,
                         Assignee = key,
                         AssigneeName = value,
                         CreateTime = nowTime.AddSeconds(1),
                         FormKey = bpmnConfCommonVo.FormCode,
                    };
                    tasks.Add(bpmAfTask);
                    if (signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
                    {
                         break;
                    }
               }
               
               _taskService.InsertTasks(tasks);
               foreach (BpmAfTask bpmAfTask in tasks)
               {
                    historyTaskInsts.Add(bpmAfTask.ToInst());
               }

               _taskInstService.baseRepo.Insert(historyTaskInsts);
          }

          ExecutionEntity executionEntity = new ExecutionEntity()
          {
               Name = executions[0].Name,
               ProcessInstanceId = procInstId,
          };
          return executionEntity;
     }
     
}