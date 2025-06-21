using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.bpmn.listener;

public class BpmnTaskListener: ITaskListener
{
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly ILogger<BpmnTaskListener> _logger;

    public BpmnTaskListener(
        BpmnConfService bpmnConfService,
        BpmBusinessProcessService bpmBusinessProcessService,
        
        ILogger<BpmnTaskListener> logger)
    {
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _logger = logger;
    }
  
    public void Notify(BpmAfTask delegateTask,string eventName)
    {
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.baseRepo
            .Where(a=>a.ProcessinessKey==delegateTask.ProcessNumber)
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