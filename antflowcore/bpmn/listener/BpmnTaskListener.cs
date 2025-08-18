using antflowcore.bpmn.service;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.bpmn.listener;

public class BpmnTaskListener: ITaskListener
{
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmProcessForwardService _bpmProcessForwardService;
    private readonly TaskService _taskService;
    private readonly UserEntrustService _userEntrustService;
    private readonly BpmVariableMessageListenerService _bpmVariableMessageListenerService;
    private readonly ProcessBusinessContansService _processBusinessContansService;
    private readonly ILogger<BpmnTaskListener> _logger;

    public BpmnTaskListener(
        BpmnConfService bpmnConfService,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmProcessForwardService bpmProcessForwardService,
        TaskService taskService,
        UserEntrustService userEntrustService,
        BpmVariableMessageListenerService bpmVariableMessageListenerService,
        ProcessBusinessContansService processBusinessContansService,
        ILogger<BpmnTaskListener> logger)
    {
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _taskService = taskService;
        _userEntrustService = userEntrustService;
        _bpmVariableMessageListenerService = bpmVariableMessageListenerService;
        _processBusinessContansService = processBusinessContansService;
        _logger = logger;
    }
  
    public void Notify(BpmAfTask delegateTask,string eventName)
    {
        if(delegateTask.NodeType==(int)NodeTypeEnum.NODE_TYPE_COPY)
        {
            BpmProcessForward bpmProcessForward = new BpmProcessForward()
            {
                CreateUserId = SecurityUtils.GetLogInEmpIdStr(),
                ForwardUserId = delegateTask.Assignee,
                ForwardUserName = delegateTask.AssigneeName,
                ProcessNumber = delegateTask.ProcessNumber,
                ProcessInstanceId = delegateTask.ProcInstId,
                IsRead = 0,
                CreateTime = DateTime.Now,
            };
            _bpmProcessForwardService.AddProcessForward(bpmProcessForward);
            delegateTask.Assignee = AFSpecialAssigneeEnum.COPY_NODE.Id;
            delegateTask.AssigneeName = AFSpecialAssigneeEnum.COPY_NODE.Desc;
           _taskService.Complete(delegateTask);
           return;
        }
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.baseRepo
            .Where(a=>a.BusinessNumber==delegateTask.ProcessNumber)
            .ToOne();
        if (bpmBusinessProcess == null)
        {
            _logger.LogError("流程实例不存在，流程号：{}", delegateTask.ProcessNumber);
            throw new AFBizException($"流程实例不存在，流程号：{delegateTask.ProcessNumber}");
        }
        BpmnConf bpmnConf = _bpmnConfService.baseRepo
            .Where(a => a.BpmnCode == bpmBusinessProcess.Version)
            .ToOne();
        if (bpmnConf == null)
        {
            _logger.LogError("流程配置不存在，流程号：{}", delegateTask.ProcessNumber);
            throw new AFBizException($"流程配置不存在，流程号：{delegateTask.ProcessNumber}");
        }

        string formCode = bpmBusinessProcess.ProcessinessKey;
        bool isOutside=(bpmnConf.IsOutSideProcess??0)==1;
        string processNumber = bpmBusinessProcess.BusinessNumber;
        string bpmnCode = bpmnConf.BpmnCode;
        BpmVariableMessageVo bpmVariableMessageVo = new BpmVariableMessageVo
        {
            ProcessNumber = processNumber,
            FormCode = formCode,
            EventType=(int)EventTypeEnum.PROCESS_FLOW,
            MessageType = EventTypeEnum.PROCESS_FLOW.IsInNode()?2:1,
            ElementId = delegateTask.TaskDefKey,
            Assignee = delegateTask.Assignee,
            EventTypeEnum = EventTypeEnum.PROCESS_FLOW,
            Type = 2,
        };
        bool sendByTemplate = _bpmVariableMessageListenerService.ListenerCheckIsSendByTemplate(bpmVariableMessageVo);
        if (sendByTemplate)
        {
            //set is outside
            bpmVariableMessageVo.IsOutside=isOutside;

            //set template message
            _bpmVariableMessageListenerService.ListenerSendTemplateMessages(bpmVariableMessageVo);
        }
        else
        {
            ProcessInforVo processInforVo = new ProcessInforVo
            {
                ProcessinessKey = bpmnCode,
                BusinessNumber = processNumber,
                FormCode = formCode,
                Type = 2,
            };
            string emailUrl = _processBusinessContansService.GetRoute(ProcessNoticeEnum.EMAIL_TYPE.Code, processInforVo , isOutside);
            string appUrl = _processBusinessContansService.GetRoute(ProcessNoticeEnum.EMAIL_TYPE.Code, processInforVo , isOutside);
            ActivitiBpmMsgVo activitiBpmMsgVo = new ActivitiBpmMsgVo
            {
                UserId = delegateTask.Assignee,
                ProcessId = processNumber,
                BpmnCode = bpmnCode,
                FormCode = formCode,
                ProcessName = bpmnConf.BpmnName,
                EmailUrl = emailUrl,
                Url = emailUrl,
                AppPushUrl = appUrl,
                TaskId = delegateTask.ProcInstId,
            };
            
        }
    }
}