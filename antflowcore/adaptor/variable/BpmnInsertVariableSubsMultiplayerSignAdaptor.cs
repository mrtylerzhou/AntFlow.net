using antflowcore.constant.enus;
using AntFlowCore.Entities;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.variable;

public class BpmnInsertVariableSubsMultiplayerSignAdaptor: IBpmnInsertVariableSubs
{
    private readonly BpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

    public BpmnInsertVariableSubsMultiplayerSignAdaptor(BpmVariableMultiplayerService bpmVariableMultiplayerService,
        BpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService)
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
            ElementName = elementVo.ElementName,
            NodeId = elementVo.NodeId,
            CollectionName = elementVo.CollectionName,
            SignType = (int)SignTypeEnum.SIGN_TYPE_SIGN
        };

        _bpmVariableMultiplayerService.baseRepo.Insert(variableMultiplayer);

        var assigneeMap = elementVo.AssigneeMap;
        var variableMultiplayerId = variableMultiplayer.Id;

        var personnelList = elementVo.CollectionValue
            .Select(o => new BpmVariableMultiplayerPersonnel
            {
                VariableMultiplayerId = variableMultiplayerId,
                Assignee = o,
                AssigneeName = assigneeMap != null && assigneeMap.ContainsKey(o) ? assigneeMap[o] : "",
                UndertakeStatus = 0
            })
            .ToList();

        _bpmVariableMultiplayerPersonnelService.baseRepo.Insert(personnelList);
    }

}