using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Variable;

public class BpmnInsertVariableSubsMultiplayerOrSignAdaptor : IBpmnInsertVariableSubs
{
    private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly BpmVariableMultiplayerService _bpmVariableMultiplayerService;

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
        BpmVariableMultiplayer? variableMultiplayer = new()
        {
            VariableId = variableId,
            ElementId = elementVo.ElementId,
            NodeId = elementVo.NodeId,
            ElementName = elementVo.ElementName,
            CollectionName = elementVo.CollectionName,
            SignType = (int)SignTypeEnum.SIGN_TYPE_OR_SIGN,
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };
        _bpmVariableMultiplayerService.baseRepo.Insert(variableMultiplayer);


        IDictionary<string, string>? assigneeMap = elementVo.AssigneeMap;

        // Set multiplayerId
        long variableMultiplayerId = variableMultiplayer.Id;
        List<BpmVariableMultiplayerPersonnel>? personnelList = elementVo.CollectionValue
            .Select(o => new BpmVariableMultiplayerPersonnel
            {
                VariableMultiplayerId = variableMultiplayerId,
                Assignee = o,
                AssigneeName = assigneeMap != null && assigneeMap.TryGetValue(o, out string? name) ? name : "",
                UndertakeStatus = 0
            })
            .ToList();
        _bpmVariableMultiplayerPersonnelService.baseRepo.Insert(personnelList);
    }
}