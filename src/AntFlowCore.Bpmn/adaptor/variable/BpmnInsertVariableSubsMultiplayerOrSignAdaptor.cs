using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.variable;

public class BpmnInsertVariableSubsMultiplayerOrSignAdaptor: IBpmnInsertVariableSubs
{
    private readonly IBpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

    public BpmnInsertVariableSubsMultiplayerOrSignAdaptor(
        IBpmVariableMultiplayerService bpmVariableMultiplayerService,
        IBpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
        ILogger<BpmnInsertVariableSubsMultiplayerOrSignAdaptor> logger)
    {
        _bpmVariableMultiplayerService = bpmVariableMultiplayerService;
        _bpmVariableMultiplayerPersonnelService = bpmVariableMultiplayerPersonnelService;
    }
   
    public void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId)
    {
        var variableMultiplayer = new BpmVariableMultiplayer
        {
            VariableId = variableId,
            ElementId = elementVo.ElementId,
            NodeId = elementVo.NodeId,
            ElementName = elementVo.ElementName,
            CollectionName = elementVo.CollectionName,
            SignType = (int)SignTypeEnum.SIGN_TYPE_OR_SIGN,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmVariableMultiplayerService.baseRepo.Insert(variableMultiplayer);
       

        var assigneeMap = elementVo.AssigneeMap;

        // Set multiplayerId
        var variableMultiplayerId = variableMultiplayer.Id;
        var personnelList = elementVo.CollectionValue
            .Select(o => new BpmVariableMultiplayerPersonnel
            {
                VariableMultiplayerId = variableMultiplayerId,
                Assignee = o,
                AssigneeName = assigneeMap != null && assigneeMap.TryGetValue(o, out var name) ? name : "",
                UndertakeStatus = 0
            })
            .ToList();
        _bpmVariableMultiplayerPersonnelService.baseRepo.Insert(personnelList);
    }

}