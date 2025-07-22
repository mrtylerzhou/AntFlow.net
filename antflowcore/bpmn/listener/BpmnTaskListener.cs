using antflowcore.bpmn.service;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.bpmn.listener;

public class BpmnTaskListener: ITaskListener
{
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmProcessForwardService _bpmProcessForwardService;
    private readonly TaskService _taskService;
    private readonly ILogger<BpmnTaskListener> _logger;

    public BpmnTaskListener(
        BpmnConfService bpmnConfService,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmProcessForwardService bpmProcessForwardService,
        TaskService taskService,
        ILogger<BpmnTaskListener> logger)
    {
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _taskService = taskService;
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
        bool isOutside=(bpmnConf.IsOutSideProcess??0)==1;
        
    }
}