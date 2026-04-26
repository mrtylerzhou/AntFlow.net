using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

public class BpmvariableBizService: IBpmvariableBizService
{
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmVariableSingleService _bpmVariableSingleService;
    private readonly IBpmVariableMultiplayerService _multiplayerService;
    private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly ILogger<BpmvariableBizService> _logger;

    public BpmvariableBizService(IBpmVariableService bpmVariableService,
        IBpmVariableSingleService bpmVariableSingleService,
        IBpmVariableMultiplayerService multiplayerService,
        IBpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
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
        var result = _bpmVariableService.GetNodeIdByElementId(processNumber, elementId);
        if (result == null)
        {
            throw new AFBizException("未能根据指定节点Id找到elementId");
        }
        return result;
    }

    public List<string> GetNodeIdByElementIds(string processNumber, List<string> elementIds)
    {
        return _bpmVariableService.GetNodeIdByElementIds(processNumber, elementIds);
    }
    public NodeElementDto GetElementIdByNodeId(String processNumber, String nodeId)
    {
        var result = _bpmVariableService.GetElementIdByNodeId(processNumber, nodeId);
        if (result == null)
        {
            throw new AFBizException("未能根据指定节点Id找到elementId");
        }
        return result;
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

        _bpmVariableMultiplayerPersonnelService._repository.AddRange(bpmVariableMultiplayerPersonnels);
    }
    
    public List<BpmVariableMultiplayer> QuerymultiplayersByProcessNumAndElementId(String processNum, String elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = this._multiplayerService._repository.QueryMultiplayersByProcessNumAndElementId(processNum, elementId);
        return bpmVariableMultiplayers;
    }

    public void InvalidNodeAssignees(List<string> assigneeIds,string processNumber, bool isSingle)
    {
        _bpmVariableService.InvalidNodeAssignees(assigneeIds, processNumber, isSingle);
    }

    public BpmVariableMultiplayer GetCurrentMultiPlayerNode(string processNumber, string elementId, string nodeId=null)
    {
        return _bpmVariableService.GetCurrentMultiPlayerNode(processNumber, elementId, nodeId);
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
                _bpmVariableSingleService._repository.UpdateAssignee(long.Parse(old.VariableId), changed.Id, changed.Name, $"管理员变更{old.Id}:{old.Name}=>{changed.Id}:{changed.Name}");
            }
            else
            {
                _bpmVariableMultiplayerPersonnelService._repository.UpdateAssignee(long.Parse(old.VariableId), changed.Id, changed.Name, $"管理员变更{old.Id}:{old.Name}=>{changed.Id}:{changed.Name}");
            }
        }
    }
}