using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableRepository : RepositoryBase<BpmVariable>, IBpmVariableRepository
{
    public SsBpmVariableRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmVariable FindByProcessNum(string processNumber)
    {
        return Db.Queryable<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber)
            .First();
    }

    public List<string> GetNodeIdsByeElementId(string processNumber, string elementId)
    {
        var query1 = Db.Queryable<BpmVariable>()
            .LeftJoin<BpmVariableSingle>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == elementId)
            .Select((a, b) => b.NodeId);

        var query2 = Db.Queryable<BpmVariable>()
            .LeftJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == elementId)
            .Select((a, b) => b.NodeId);

        return Db.UnionAll(query1, query2).ToList();
    }

    public List<string> GetElementIdsdByNodeId(string processNumber, string nodeId)
    {
        var query1 = Db.Queryable<BpmVariable>()
            .LeftJoin<BpmVariableSingle>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .Select((a, b) => b.ElementId);

        var query2 = Db.Queryable<BpmVariable>()
            .LeftJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .Select((a, b) => b.ElementId);

        return Db.UnionAll(query1, query2).ToList();
    }

    public NodeElementDto GetNodeIdByElementId(string processNumber, string elementId)
    {
        NodeElementDto? nodeSingleElementDto = null;
        var firstOrDefault = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableSingle>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == elementId)
            .Select((a, b) => new { Variable = a, Single = b })
            .First();
        if (firstOrDefault != null)
        {
            nodeSingleElementDto = new NodeElementDto();
            nodeSingleElementDto.IsSingle = true;
            nodeSingleElementDto.NodeId = firstOrDefault.Single.NodeId;
            nodeSingleElementDto.ElementId = elementId;
            nodeSingleElementDto.VarName = firstOrDefault.Single.AssigneeParamName;
            nodeSingleElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>()
            {
                new BaseInfoTranStructVo()
                {
                    Id = firstOrDefault.Single.Assignee,
                    Name = firstOrDefault.Single.AssigneeName,
                    VariableId = firstOrDefault.Single.Id.ToString(),
                }
            };
        }

        var tuples = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .InnerJoin<BpmVariableMultiplayerPersonnel>((a, b, c) => b.Id == c.VariableMultiplayerId)
            .Where((a, b, c) => a.ProcessNum == processNumber && b.ElementId == elementId)
            .OrderBy((a, b, c) => c.UpdateTime)
            .Select((a, b, c) => new { Variable = a, Multiplayer = b, Personnel = c })
            .ToList();
        NodeElementDto nodeMultiplayerElementDto = null;
        if (!tuples.IsEmpty())
        {
            nodeMultiplayerElementDto = new NodeElementDto();
            nodeMultiplayerElementDto.NodeId = tuples[0].Multiplayer.NodeId;
            nodeMultiplayerElementDto.ElementId = elementId;
            nodeMultiplayerElementDto.IsSingle = false;
            nodeMultiplayerElementDto.VarName = tuples[0].Multiplayer.CollectionName;
            nodeMultiplayerElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>();

            nodeMultiplayerElementDto.AssigneeInfoList
                .AddRange(tuples.Select(a => new BaseInfoTranStructVo
                {
                    Id = a.Personnel.Assignee,
                    Name = a.Personnel.AssigneeName,
                    VariableId = a.Personnel.Id.ToString(),
                }));
        }

        if (nodeSingleElementDto != null)
        {
            return nodeSingleElementDto;
        }
        else if (nodeMultiplayerElementDto != null)
        {
            return nodeMultiplayerElementDto;
        }
        return null;
    }

    public NodeElementDto GetElementIdByNodeId(string processNumber, string nodeId)
    {
        NodeElementDto? nodeSingleElementDto = null;
        var firstOrDefault = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableSingle>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .Select((a, b) => new { Variable = a, Single = b })
            .First();
        if (firstOrDefault != null)
        {
            nodeSingleElementDto = new NodeElementDto();
            nodeSingleElementDto.IsSingle = true;
            nodeSingleElementDto.NodeId = nodeId;
            nodeSingleElementDto.ElementId = firstOrDefault.Single.ElementId;
            nodeSingleElementDto.VarName = firstOrDefault.Single.AssigneeParamName;
            nodeSingleElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>()
            {
                new BaseInfoTranStructVo()
                {
                    Id = firstOrDefault.Single.Assignee,
                    Name = firstOrDefault.Single.AssigneeName,
                    VariableId = firstOrDefault.Single.Id.ToString(),
                }
            };
        }

        var tuples = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .InnerJoin<BpmVariableMultiplayerPersonnel>((a, b, c) => b.Id == c.VariableMultiplayerId)
            .Where((a, b, c) => a.ProcessNum == processNumber && b.NodeId == nodeId)
            .OrderBy((a, b, c) => c.UpdateTime)
            .Select((a, b, c) => new { Variable = a, Multiplayer = b, Personnel = c })
            .ToList();
        NodeElementDto nodeMultiplayerElementDto = null;
        if (!tuples.IsEmpty())
        {
            nodeMultiplayerElementDto = new NodeElementDto();
            nodeMultiplayerElementDto.NodeId = nodeId;
            nodeMultiplayerElementDto.ElementId = tuples[0].Multiplayer.ElementId;
            nodeMultiplayerElementDto.IsSingle = false;
            nodeMultiplayerElementDto.VarName = tuples[0].Multiplayer.CollectionName;
            nodeMultiplayerElementDto.AssigneeInfoList = new List<BaseInfoTranStructVo>();

            nodeMultiplayerElementDto.AssigneeInfoList
                .AddRange(tuples.Select(a => new BaseInfoTranStructVo
                {
                    Id = a.Personnel.Assignee,
                    Name = a.Personnel.AssigneeName,
                    VariableId = a.Personnel.Id.ToString(),
                }));
        }

        if (nodeSingleElementDto != null)
        {
            return nodeSingleElementDto;
        }
        else if (nodeMultiplayerElementDto != null)
        {
            return nodeMultiplayerElementDto;
        }
        return null;
    }

    public List<string> GetNodeIdByElementIds(string processNumber, List<string> elementIds)
    {
        List<string> nodeIds = new List<string>();
        List<BpmVariableSingle> bpmVariableSingles = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableSingle>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && elementIds.Contains(b.ElementId))
            .Select((a, b) => b)
            .ToList();
        if (!bpmVariableSingles.IsEmpty())
        {
            nodeIds.AddRange(bpmVariableSingles.Select(a => a.NodeId));
        }

        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && elementIds.Contains(b.ElementId))
            .Select((a, b) => b)
            .ToList();
        if (!bpmVariableMultiplayers.IsEmpty())
        {
            nodeIds.AddRange(bpmVariableMultiplayers.Select(a => a.NodeId));
        }
        return nodeIds.Distinct().ToList();
    }

    public BpmVariableMultiplayer GetCurrentMultiPlayerNode(string processNumber, string elementId, string nodeId)
    {
        var query = Db.Queryable<BpmVariable>()
            .InnerJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber);

        if (!string.IsNullOrEmpty(nodeId))
        {
            query = query.Where((a, b) => b.ElementId == elementId);
        }

        if (!string.IsNullOrEmpty(elementId))
        {
            query = query.Where((a, b) => b.NodeId == nodeId);
        }

        return query.Select((a, b) => b)
            .ToList()
            .FirstOrDefault();
    }

    public void InvalidNodeAssignees(List<string> assigneeIds, string processNumber, bool isSingle)
    {
        BpmVariable bpmVariable = Db.Queryable<BpmVariable>()
            .Where(a => a.ProcessNum == processNumber)
            .First();
        if (bpmVariable == null)
        {
            throw new AFBizException($"未能根据流程编号找到变量信息!{processNumber}");
        }

        long bpmVariableId = bpmVariable.Id;
        if (isSingle)
        {
            Db.Updateable<BpmVariableSignUp>()
                .SetColumns(a => a.IsDel == 1)
                .SetColumns(a => a.Remark == "管理员减签")
                .SetColumns(a => a.UpdateTime == DateTime.Now)
                .Where(a => a.Id == bpmVariableId)
                .ExecuteCommand();
            return;
        }

        var bpmVariableMultiplayer = Db.Queryable<BpmVariableMultiplayer>()
            .Where(a => a.VariableId == bpmVariableId)
            .First();
        if (bpmVariableMultiplayer == null)
        {
            throw new AFBizException($"未能根据流程编号找到流程多变量信息!{processNumber}");
        }

        long multiPlayerId = bpmVariableMultiplayer.Id;
        Db.Updateable<BpmVariableMultiplayerPersonnel>()
            .SetColumns(a => a.IsDel == 1)
            .SetColumns(a => a.Remark == "管理员减签")
            .Where(a => a.VariableMultiplayerId == multiPlayerId && assigneeIds.Contains(a.Assignee))
            .ExecuteCommand();
    }
}
