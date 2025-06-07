using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.bpmn.listener;

public class BpmnTaskListener: ITaskListener
{
    private readonly UserEntrustService _userEntrustService;
    private readonly BpmFlowrunEntrustService _flowrunEntrustService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly ILogger<BpmnTaskListener> _logger;

    public BpmnTaskListener(UserEntrustService userEntrustService,
        BpmFlowrunEntrustService flowrunEntrustService,
        BpmBusinessProcessService bpmBusinessProcessService,
        ILogger<BpmnTaskListener> logger)
    {
        _userEntrustService = userEntrustService;
        _flowrunEntrustService = flowrunEntrustService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _logger = logger;
    }
  
    public void Notify(BpmAfTask delegateTask,string eventName)
    {
        string s="notifier";
        //todo to be implemented
    }
}