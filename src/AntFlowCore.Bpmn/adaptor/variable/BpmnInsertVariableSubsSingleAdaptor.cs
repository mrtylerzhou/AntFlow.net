using AntFlowCore.Common.util;
using AntFlowCore.Core.entity;
using AntFlowCore.Extensions.Extensions.adaptor.variable;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.variable;

public class BpmnInsertVariableSubsSingleAdaptor: IBpmnInsertVariableSubs
{
    private readonly IBpmVariableSingleService _bpmVariableSingleService;

    public BpmnInsertVariableSubsSingleAdaptor(IBpmVariableSingleService bpmVariableSingleService)
    {
        _bpmVariableSingleService = bpmVariableSingleService;
    }
    public void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId)
    {
        var assigneeMap = elementVo.AssigneeMap;

        var variableSingle = new BpmVariableSingle
        {
            VariableId = variableId,
            ElementId = elementVo.ElementId,
            ElementName = elementVo.ElementName,
            NodeId = elementVo.NodeId,
            AssigneeParamName = elementVo.AssigneeParamName,
            Assignee = elementVo.AssigneeParamValue,
            AssigneeName = assigneeMap != null && assigneeMap.ContainsKey(elementVo.AssigneeParamValue)
                ? assigneeMap[elementVo.AssigneeParamValue]??""
                : "",
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };

        _bpmVariableSingleService.baseRepo.Insert(variableSingle);
    }

}