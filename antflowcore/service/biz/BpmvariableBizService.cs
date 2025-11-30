using System.Linq.Expressions;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util.Extension;
using antflowcore.vo;
using FreeSql.Internal.Model;

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

    public NodeElementDto GetElementIdByNodeId(String processNumber, String nodeId)
    {
        NodeElementDto? nodeSingleElementDto = _bpmVariableService.Frsql
            .Select<BpmVariable,BpmVariableSingle>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&b.NodeId==nodeId)
            .ToList<NodeElementDto>((a,b)=>new NodeElementDto
            {
                NodeId = nodeId,
                ElementId = b.ElementId,
                AssigneeInfoList = new List<BaseIdTranStruVo>
                {
                    new BaseIdTranStruVo
                    {
                        Id = b.Assignee,
                        Name = b.AssigneeName
                    }
                }
            })
            .FirstOrDefault();


        var tuples = _bpmVariableService.Frsql
            .Select<BpmVariable, BpmVariableMultiplayer, BpmVariableMultiplayerPersonnel>()
            .InnerJoin((a, b, c) => a.Id == b.VariableId)
            .InnerJoin((a, b, c) => b.Id == c.VariableMultiplayerId)
            .Where((a, b, c) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .ToList<HzyTuple<BpmVariable, BpmVariableMultiplayer, BpmVariableMultiplayerPersonnel>>(
                (a,b,c)=>new HzyTuple<BpmVariable, BpmVariableMultiplayer, BpmVariableMultiplayerPersonnel>(a,b,c)
            );
        NodeElementDto nodeMultiplayerElementDto = null;
        if (!tuples.IsEmpty())
        {
            nodeMultiplayerElementDto = new NodeElementDto();
            nodeMultiplayerElementDto.NodeId = nodeId;
            nodeMultiplayerElementDto.ElementId = tuples[0].t2.ElementId;
            nodeMultiplayerElementDto.IsSingle = false;
            nodeMultiplayerElementDto.AssigneeInfoList = new List<BaseIdTranStruVo>();
            
            nodeMultiplayerElementDto.AssigneeInfoList
                .AddRange(tuples.Select(a=>new BaseIdTranStruVo(a.t3.Assignee,a.t3.AssigneeName)));
        }

        if (nodeSingleElementDto != null)
        {
            return nodeSingleElementDto;
        }else if (nodeMultiplayerElementDto != null)
        {
            return nodeMultiplayerElementDto;
        }

        throw new AFBizException("未能根据指定节点Id找到elementId");
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

    public void InvalidNodeAssignees(List<string> assigneeIds,string processNumber, bool isSingle)
    {
        BpmVariable bpmVariable = _bpmVariableService.baseRepo
            .Where(a=>a.ProcessNum==processNumber)
            .ToOne();
        if (bpmVariable == null)
        {
            throw new AFBizException($"未能根据流程编号找到变量信息!{processNumber}");
        }

        long bpmVariableId = bpmVariable.Id;
        if (isSingle)
        {
            _bpmVariableService.Frsql
                .Update<BpmVariableSignUp>()
                .Set(a => a.IsDel, 1)
                .Set(a=>a.Remark,"管理员减签")
                .Where(a => a.Id == bpmVariableId)
                .ExecuteAffrows();
            return;
        }

        BpmVariableMultiplayer bpmVariableMultiplayer = _multiplayerService.baseRepo
            .Where(a=>a.VariableId==bpmVariableId)
            .ToOne();
        if (bpmVariableMultiplayer == null)
        {
            throw new AFBizException($"未能根据流程编号找到流程多变量信息!{processNumber}");
        }

        long multiPlayerId = bpmVariableMultiplayer.Id;
        _bpmVariableService.Frsql
            .Update<BpmVariableMultiplayerPersonnel>()
            .Set(a => a.IsDel, 1)
            .Set(a => a.Remark, "管理员减签")
            .Where(a => a.VariableMultiplayerId == multiPlayerId&&assigneeIds.Contains(a.Assignee))
            .ExecuteAffrows();
    }

    public BpmVariableMultiplayer GetCurrentMultiPlayerNode(string processNumber, string elementId, string nodeId=null)
    {
        Expression<Func<BpmVariable,BpmVariableMultiplayer,bool>> whereCond = LinqExtensions.True<BpmVariable,BpmVariableMultiplayer>();
        whereCond = LinqExtensions.And(whereCond, (a, b) => a.ProcessNum == processNumber);
        whereCond= whereCond.WhereIf(!string.IsNullOrEmpty(nodeId), (a, b) => b.ElementId == elementId);
        
        whereCond = whereCond.WhereIf(!string.IsNullOrEmpty(elementId), (a, b) => b.NodeId == nodeId);
        BpmVariableMultiplayer? bpmVariableMultiplayer = _bpmVariableService.Frsql
            .Select<BpmVariable,BpmVariableMultiplayer>()
            .Where(whereCond)
            .ToList<BpmVariableMultiplayer>((a,b)=>b)
            .FirstOrDefault();


        return bpmVariableMultiplayer;
    }
}