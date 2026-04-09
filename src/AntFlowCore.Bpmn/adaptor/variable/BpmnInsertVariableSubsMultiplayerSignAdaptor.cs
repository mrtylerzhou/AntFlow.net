using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.variable;

public class BpmnInsertVariableSubsMultiplayerSignAdaptor: IBpmnInsertVariableSubs
{
    private readonly IBpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

    public BpmnInsertVariableSubsMultiplayerSignAdaptor(IBpmVariableMultiplayerService bpmVariableMultiplayerService,
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
            ElementName = elementVo.ElementName,
            NodeId = elementVo.NodeId,
            CollectionName = elementVo.CollectionName,
            SignType = (int)SignTypeEnum.SIGN_TYPE_SIGN,
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };

        _bpmVariableMultiplayerService.baseRepo.Insert(variableMultiplayer);

        IDictionary<string,string> assigneeMap = elementVo.AssigneeMap;
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

        _bpmVariableMultiplayerPersonnelService.baseRepo.Insert(personnelList);
    }

}