using antflowcore.entity;
using antflowcore.service.repository;

namespace antflowcore.service.biz;

public class BpmvariableBizService
{
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmVariableSingleService _bpmVariableSingleService;
    private readonly BpmVariableMultiplayerService _multiplayerService;
    private readonly ILogger<BpmvariableBizService> _logger;

    public BpmvariableBizService(BpmVariableService bpmVariableService,
        BpmVariableSingleService bpmVariableSingleService,
        BpmVariableMultiplayerService multiplayerService,
        ILogger<BpmvariableBizService> logger)
    {
        _bpmVariableService = bpmVariableService;
        _bpmVariableSingleService = bpmVariableSingleService;
        _multiplayerService = multiplayerService;
        _logger = logger;
    }

    public string GetNodeIdByElementId(string processNumber, string elementId)
    {
        string nodeIdSingle = _bpmVariableService.Frsql
            .Select<BpmVariable,BpmVariableSingle>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&b.ElementId==elementId)
            .ToList<string>((a,b)=>b.NodeId)
            .First();
        string nodeIdMultiplayer = _bpmVariableService.Frsql
            .Select<BpmVariable,BpmVariableMultiplayer>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&b.ElementId==elementId)
            .ToList<string>((a,b)=>b.NodeId)
            .First();
        return !string.IsNullOrEmpty(nodeIdSingle) ? nodeIdSingle : nodeIdMultiplayer;
    }
}