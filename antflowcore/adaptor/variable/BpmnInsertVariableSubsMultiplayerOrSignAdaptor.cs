using antflowcore.constant.enus;
using AntFlowCore.Entities;
using antflowcore.service.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.variable;

public class BpmnInsertVariableSubsMultiplayerOrSignAdaptor: IBpmnInsertVariableSubs
{
    private readonly BpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

    public BpmnInsertVariableSubsMultiplayerOrSignAdaptor(
        BpmVariableMultiplayerService bpmVariableMultiplayerService,
        BpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
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
            SignType = (int)SignTypeEnum.SIGN_TYPE_OR_SIGN
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