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

    public NodeElementDto GetNodeIdByElementId(string processNumber, string elementId)
    {
        NodeElementDto? nodeSingleElementDto = null;
        HzyTuple<BpmVariable,BpmVariableSingle>? firstOrDefault = _bpmVariableService.Frsql
            .Select<BpmVariable,BpmVariableSingle>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&b.ElementId==elementId)
            .ToList<HzyTuple<BpmVariable,BpmVariableSingle>>((a,b)=>new HzyTuple<BpmVariable, BpmVariableSingle>(a,b))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            nodeSingleElementDto = new NodeElementDto();
            nodeSingleElementDto.IsSingle = true;
            nodeSingleElementDto.NodeId = firstOrDefault.t2.NodeId;;
            nodeSingleElementDto.ElementId =elementId;
            nodeSingleElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>()
            {
                new BaseInfoTranStructVo()
                {
                    Id = firstOrDefault.t2.Assignee,
                    Name = firstOrDefault.t2.AssigneeName,
                    VariableId = firstOrDefault.t2.Id.ToString(),//存储有人的节点的id
                }
            };
        }
        
        var tuples = _bpmVariableService.Frsql
            .Select<BpmVariable,BpmVariableMultiplayer,BpmVariableMultiplayerPersonnel>()
            .InnerJoin((a,b,c)=>a.Id==b.VariableId)
            .InnerJoin((a, b, c) => b.Id == c.VariableMultiplayerId)
            .Where((a,b,c)=>a.ProcessNum==processNumber&&b.ElementId==elementId)
            .OrderBy((a,b,c)=>c.UpdateTime)
            .ToList<HzyTuple<BpmVariable, BpmVariableMultiplayer, BpmVariableMultiplayerPersonnel>>(
                (a,b,c)=>new HzyTuple<BpmVariable, BpmVariableMultiplayer, BpmVariableMultiplayerPersonnel>(a,b,c)
            );
        NodeElementDto nodeMultiplayerElementDto = null;
        if (!tuples.IsEmpty())
        {
            nodeMultiplayerElementDto = new NodeElementDto();
            nodeMultiplayerElementDto.NodeId = tuples[0].t2.NodeId;
            nodeMultiplayerElementDto.ElementId = elementId;
            nodeMultiplayerElementDto.IsSingle = false;
            nodeMultiplayerElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>();
            
            nodeMultiplayerElementDto.AssigneeInfoList
                .AddRange(tuples.Select(a=>new BaseInfoTranStructVo
                {
                    Id = a.t3.Assignee,
                    Name = a.t3.AssigneeName,
                    VariableId = a.t3.Id.ToString(),
                }));
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

    public NodeElementDto GetElementIdByNodeId(String processNumber, String nodeId)
    {
        NodeElementDto? nodeSingleElementDto = null;
        HzyTuple<BpmVariable,BpmVariableSingle>? firstOrDefault = _bpmVariableService.Frsql
            .Select<BpmVariable,BpmVariableSingle>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&b.NodeId==nodeId)
            .ToList<HzyTuple<BpmVariable,BpmVariableSingle>>((a,b)=>new HzyTuple<BpmVariable, BpmVariableSingle>(a,b))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            nodeSingleElementDto = new NodeElementDto();
            nodeSingleElementDto.IsSingle = true;
            nodeSingleElementDto.NodeId = nodeId;
            nodeSingleElementDto.ElementId = firstOrDefault.t2.ElementId;
            nodeSingleElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>()
            {
                new BaseInfoTranStructVo()
                {
                    Id = firstOrDefault.t2.Assignee,
                    Name = firstOrDefault.t2.AssigneeName,
                    VariableId = firstOrDefault.t2.Id.ToString(),//存储有人的节点的id
                }
            };
        }

        var tuples = _bpmVariableService.Frsql
            .Select<BpmVariable, BpmVariableMultiplayer, BpmVariableMultiplayerPersonnel>()
            .InnerJoin((a, b, c) => a.Id == b.VariableId)
            .InnerJoin((a, b, c) => b.Id == c.VariableMultiplayerId)
            .Where((a, b, c) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .OrderBy((a,b,c)=>c.UpdateTime)
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
            nodeMultiplayerElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>();
            
            nodeMultiplayerElementDto.AssigneeInfoList
                .AddRange(tuples.Select(a=>new BaseInfoTranStructVo
                {
                    Id = a.t3.Assignee,
                    Name = a.t3.AssigneeName,
                    VariableId = a.t3.Id.ToString(),
                }));
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
                .Set(a=> a.Remark,"管理员减签")
                .Set(a=> a.UpdateTime, DateTime.Now)
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

    public void ChangeVariableAssignees(IDictionary<BaseInfoTranStructVo,BaseIdTranStruVo> changedAssignees,bool isSingle)
    {
        if (changedAssignees.IsEmpty())
        {
            return;
        }

        foreach (var (old, changed) in changedAssignees)
        {
            if (isSingle)
            {
                _bpmVariableSingleService.Frsql
                    .Update<BpmVariableSingle>()
                    .Set(a => a.Assignee, changed.Id)
                    .Set(a => a.AssigneeName, changed.Name)
                    .Set(a=>a.Remark,$"管理员变更{old.Id}:{old.Name}=>{changed.Id}:{changed.Name}")
                    .Where(a => a.Id == long.Parse(old.VariableId))
                    .ExecuteAffrows();
            }
            else
            {
                _bpmVariableMultiplayerPersonnelService.Frsql
                    .Update<BpmVariableMultiplayerPersonnel>()
                    .Set(a => a.Assignee, changed.Id)
                    .Set(a => a.AssigneeName, changed.Name)
                    .Set(a => a.Remark, $"管理员变更{old.Id}:{old.Name}=>{changed.Id}:{changed.Name}")
                    .Where(a => a.Id == long.Parse(old.VariableId))
                    .ExecuteAffrows();
            }
        }
    }
}