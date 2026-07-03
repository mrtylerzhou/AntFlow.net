using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

public class BpmvariableBizService: IBpmvariableBizService
{
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmVariableMultiplayerService _multiplayerService;
    private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly ILogger<BpmvariableBizService> _logger;

    public BpmvariableBizService(IBpmVariableService bpmVariableService,
        IBpmVariableMultiplayerService multiplayerService,
        IBpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
        ILogger<BpmvariableBizService> logger)
    {
        _bpmVariableService = bpmVariableService;
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

    /// <summary>
    /// 判断是否为或签多人节点且尚未被承
    /// 1. 查询 multiplayer LEFT JOIN personnel(每条 personnel 一行,UnderTakeStatus 承载 undertake_status)
    /// 2. 过滤出 underTakeStatus==0 的记录(未被承办)
    /// 3. 未被承办的人数 > 1 且 signType==2(或签)时返回 true
    /// </summary>
    public bool IsMoreNode(string processNum, string elementId)
    {
        List<BpmVariableMultiplayer> list = _multiplayerService._repository.IsMoreNode(processNum, elementId);
        if (list == null || list.Count == 0)
        {
            return false;
        }
        // 过滤出未被承办的记录(undertake_status==0)
        List<BpmVariableMultiplayer> notUndertaken = list
            .Where(a => a.UnderTakeStatus.HasValue && a.UnderTakeStatus.Value == 0)
            .ToList();
        return notUndertaken.Count > 1 && notUndertaken[0].SignType == 2;
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
                // BpmVariableSingle has been removed; single assignee update is no longer supported
            }
            else
            {
                _bpmVariableMultiplayerPersonnelService._repository.UpdateAssignee(long.Parse(old.VariableId), changed.Id, changed.Name, $"管理员变更{old.Id}:{old.Name}=>{changed.Id}:{changed.Name}");
            }
        }
    }
}