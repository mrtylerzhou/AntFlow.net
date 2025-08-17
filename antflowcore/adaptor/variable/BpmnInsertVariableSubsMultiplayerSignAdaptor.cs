using antflowcore.constant.enus;
using AntFlowCore.Entities;
using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.util;
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
                AssigneeName = assigneeMap != null && assigneeMap.ContainsKey(o) ? assigneeMap[o] : "",
                UndertakeStatus = 0,
                CreateTime = DateTime.Now,
            })
            .ToList();

        _bpmVariableMultiplayerPersonnelService.baseRepo.Insert(personnelList);
    }

}