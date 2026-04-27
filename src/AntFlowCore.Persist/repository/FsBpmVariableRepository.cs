using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableRepository : RepositoryBase<BpmVariable>, IBpmVariableRepository
{
    public FsBpmVariableRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmVariable FindByProcessNum(string processNumber)
    {
        return _ormContext.FreeSql.Select<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber)
            .First();
    }

    public List<string> GetNodeIdsByeElementId(string processNumber, string elementId)
    {
        List<string> nodeIds = _ormContext.FreeSql.Select<BpmVariable, BpmVariableSingle>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == elementId)
            .WithTempQuery((a, b) => b.NodeId)

            .UnionAll(
                _ormContext.FreeSql.Select<BpmVariable, BpmVariableMultiplayer>()
                    .InnerJoin((a, b) => a.Id == b.VariableId)
                    .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == elementId)
                    .WithTempQuery((a, b) => b.NodeId)
            )
            .ToList();
        return nodeIds;
    }

    public List<string> GetElementIdsdByNodeId(string processNumber, string nodeId)
    {
        List<string> elementIds = _ormContext.FreeSql.Select<BpmVariable, BpmVariableSingle>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .WithTempQuery((a, b) => b.ElementId)
            .UnionAll(
                _ormContext.FreeSql.Select<BpmVariable, BpmVariableMultiplayer>()
                    .InnerJoin((a, b) => a.Id == b.VariableId)
                    .Where((a, b) => a.ProcessNum == processNumber && b.NodeId == nodeId)
                    .WithTempQuery((a, b) => b.ElementId)
            )
            .ToList();
        return elementIds;
    }

    public NodeElementDto GetNodeIdByElementId(string processNumber, string elementId)
    {
        NodeElementDto? nodeSingleElementDto = null;
        HzyTuple<BpmVariable,BpmVariableSingle>? firstOrDefault = _ormContext.FreeSql
            .Select<BpmVariable,BpmVariableSingle>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&b.ElementId==elementId)
            .ToList<HzyTuple<BpmVariable,BpmVariableSingle>>((a,b)=>new HzyTuple<BpmVariable, BpmVariableSingle>(a,b))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            nodeSingleElementDto = new NodeElementDto();
            nodeSingleElementDto.IsSingle = true;
            nodeSingleElementDto.NodeId = firstOrDefault.t2.NodeId;
            nodeSingleElementDto.ElementId =elementId;
            nodeSingleElementDto.VarName = firstOrDefault.t2.AssigneeParamName;
            nodeSingleElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>()
            {
                new BaseInfoTranStructVo()
                {
                    Id = firstOrDefault.t2.Assignee,
                    Name = firstOrDefault.t2.AssigneeName,
                    VariableId = firstOrDefault.t2.Id.ToString(),
                }
            };
        }
        
        var tuples = _ormContext.FreeSql
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
            nodeMultiplayerElementDto.VarName = tuples[0].t2.CollectionName;
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
        return null;
    }

    public NodeElementDto GetElementIdByNodeId(string processNumber, string nodeId)
    {
        NodeElementDto? nodeSingleElementDto = null;
        HzyTuple<BpmVariable,BpmVariableSingle>? firstOrDefault = _ormContext.FreeSql
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
            nodeSingleElementDto.VarName = firstOrDefault.t2.AssigneeParamName;
            nodeSingleElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>()
            {
                new BaseInfoTranStructVo()
                {
                    Id = firstOrDefault.t2.Assignee,
                    Name = firstOrDefault.t2.AssigneeName,
                    VariableId = firstOrDefault.t2.Id.ToString(),
                }
            };
        }

        var tuples = _ormContext.FreeSql
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
            nodeMultiplayerElementDto.VarName = tuples[0].t2.CollectionName;
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
        return null;
    }

    public List<string> GetNodeIdByElementIds(string processNumber, List<string> elementIds)
    {
        List<string> nodeIds = new List<string>();
        List<BpmVariableSingle> bpmVariableSingles = _ormContext.FreeSql
            .Select<BpmVariable,BpmVariableSingle>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&elementIds.Contains(b.ElementId))
            .ToList<BpmVariableSingle>();
        if (!bpmVariableSingles.IsEmpty())
        {
            nodeIds.AddRange(bpmVariableSingles.Select(a=>a.NodeId));
        }

        List<BpmVariableMultiplayer> bpmVariableMultiplayers = _ormContext.FreeSql
            .Select<BpmVariable,BpmVariableMultiplayer>()
            .InnerJoin((a,b)=>a.Id==b.VariableId)
            .Where((a,b)=>a.ProcessNum==processNumber&&elementIds.Contains(b.ElementId))
            .ToList<BpmVariableMultiplayer>();
        if (!bpmVariableMultiplayers.IsEmpty())
        {
            nodeIds.AddRange(bpmVariableMultiplayers.Select(a=>a.NodeId));
        }
        return nodeIds.Distinct().ToList();
    }

    public BpmVariableMultiplayer GetCurrentMultiPlayerNode(string processNumber, string elementId, string nodeId)
    {
        Expression<Func<BpmVariable,BpmVariableMultiplayer,bool>> whereCond = LinqExtensions.True<BpmVariable,BpmVariableMultiplayer>();
        whereCond = LinqExtensions.And(whereCond, (a, b) => a.ProcessNum == processNumber);
        whereCond= whereCond.WhereIf(!string.IsNullOrEmpty(nodeId), (a, b) => b.ElementId == elementId);
        
        whereCond= whereCond.WhereIf(!string.IsNullOrEmpty(elementId), (a, b) => b.NodeId == nodeId);
        BpmVariableMultiplayer? bpmVariableMultiplayer = _ormContext.FreeSql
            .Select<BpmVariable,BpmVariableMultiplayer>()
            .Where(whereCond)
            .ToList<BpmVariableMultiplayer>((a,b)=>b)
            .FirstOrDefault();

        return bpmVariableMultiplayer;
    }

    public void InvalidNodeAssignees(List<string> assigneeIds, string processNumber, bool isSingle)
    {
        BpmVariable bpmVariable = _ormContext.FreeSql.Select<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber)
            .First();
        if (bpmVariable == null)
        {
            throw new AFBizException($"未能根据流程编号找到变量信息!{processNumber}");
        }

        long bpmVariableId = bpmVariable.Id;
        if (isSingle)
        {
            _ormContext.FreeSql.Update<BpmVariableSignUp>()
                .Set(a => a.IsDel, 1)
                .Set(a => a.Remark, "管理员减签")
                .Set(a => a.UpdateTime, DateTime.Now)
                .Where(a => a.Id == bpmVariableId)
                .ExecuteAffrows();
            return;
        }

        var bpmVariableMultiplayer = _ormContext.FreeSql.Select<BpmVariableMultiplayer>()
            .Where(a => a.VariableId == bpmVariableId)
            .First();
        if (bpmVariableMultiplayer == null)
        {
            throw new AFBizException($"未能根据流程编号找到流程多变量信息!{processNumber}");
        }

        long multiPlayerId = bpmVariableMultiplayer.Id;
        _ormContext.FreeSql.Update<BpmVariableMultiplayerPersonnel>()
            .Set(a => a.IsDel, 1)
            .Set(a => a.Remark, "管理员减签")
            .Where(a => a.VariableMultiplayerId == multiPlayerId && assigneeIds.Contains(a.Assignee))
            .ExecuteAffrows();
    }
}
