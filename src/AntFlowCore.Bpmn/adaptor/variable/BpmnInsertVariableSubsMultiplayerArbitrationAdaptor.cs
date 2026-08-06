using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.variable;

/// <summary>
/// Inserts variable sub-records (BpmVariableMultiplayer + BpmVariableMultiplayerPersonnel)
/// for an arbitration sign node. Mirrors Java BpmnInsertVariableSubsMultiplayerArbitrationAdp.
/// The arbitrationRatio itself is NOT stored here; it is carried in the deployment
/// content (BpmnConfCommonElementVo.ArbitrationRatio) and retrieved at runtime.
/// </summary>
public class BpmnInsertVariableSubsMultiplayerArbitrationAdaptor : IBpmnInsertVariableSubs
{
    private readonly IBpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

    public BpmnInsertVariableSubsMultiplayerArbitrationAdaptor(
        IBpmVariableMultiplayerService bpmVariableMultiplayerService,
        IBpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService)
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
            SignType = (int)SignTypeEnum.SIGN_TYPE_ARBITRATION,
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };

        _bpmVariableMultiplayerService._repository.Add(variableMultiplayer);

        IDictionary<string, string> assigneeMap = elementVo.AssigneeMap;
        long variableMultiplayerId = variableMultiplayer.Id;

        List<BpmVariableMultiplayerPersonnel> personnelList = elementVo.CollectionValue
            .Select(o => new BpmVariableMultiplayerPersonnel
            {
                VariableMultiplayerId = variableMultiplayerId,
                Assignee = o,
                AssigneeName = assigneeMap != null && assigneeMap.TryGetValue(o, out var value) ? value : "",
                UndertakeStatus = 0,
                CreateTime = DateTime.Now,
            })
            .ToList();

        _bpmVariableMultiplayerPersonnelService._repository.AddRange(personnelList);
    }
}