using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.vo;

namespace antflowcore.service.biz;

public class BpmvariableBizService
{
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmVariableSingleService _bpmVariableSingleService;
    private readonly BpmVariableMultiplayerService _multiplayerService;
    private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly ILogger<BpmvariableBizService> _logger;

    public BpmvariableBizService(BpmVariableService bpmVariableService,
        BpmVariableSingleService bpmVariableSingleService,
        BpmVariableMultiplayerService multiplayerService,
        BpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
        ILogger<BpmvariableBizService> logger)
    {
        _bpmVariableService = bpmVariableService;
        _bpmVariableSingleService = bpmVariableSingleService;
        _multiplayerService = multiplayerService;
        _bpmVariableMultiplayerPersonnelService = bpmVariableMultiplayerPersonnelService;
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

    public void AddNodeAssignees(String processNumber, String elementId, List<BaseIdTranStruVo> assignees)
    {
        List<BpmVariableMultiplayer> multiplayers = QuerymultiplayersByProcessNumAndElementId(processNumber,elementId);
        List<BpmVariableMultiplayerPersonnel> bpmVariableMultiplayerPersonnels=new List<BpmVariableMultiplayerPersonnel>();
        foreach (BaseIdTranStruVo assignee in assignees)
        {
            BpmVariableMultiplayerPersonnel multiplayerPersonnel = new BpmVariableMultiplayerPersonnel
            {
                VariableMultiplayerId = multiplayers[0].Id,
                Assignee = assignee.Id,
                AssigneeName = assignee.Name,
                UndertakeStatus = 0,
                Remark = "管理员加签",
            };
            bpmVariableMultiplayerPersonnels.Add(multiplayerPersonnel);
        }

        _bpmVariableMultiplayerPersonnelService.baseRepo.Insert(bpmVariableMultiplayerPersonnels);
    }
    
    public List<BpmVariableMultiplayer> QuerymultiplayersByProcessNumAndElementId(String processNum, String elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = this._multiplayerService.Frsql
            .Select<BpmVariable,BpmVariableMultiplayer>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNum&&b.ElementId==elementId)
            .ToList<BpmVariableMultiplayer>();
        return bpmVariableMultiplayers;
    }
}