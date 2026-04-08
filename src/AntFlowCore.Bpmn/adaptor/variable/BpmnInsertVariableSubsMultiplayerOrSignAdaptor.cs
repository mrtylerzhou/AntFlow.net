using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.util;
using AntFlowCore.Extensions.Extensions.adaptor.variable;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
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