using AntFlowCore.Entities;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.variable;

public class BpmnInsertVariableSubsSingleAdaptor: IBpmnInsertVariableSubs
{
    private readonly BpmVariableSingleService _bpmVariableSingleService;

    public BpmnInsertVariableSubsSingleAdaptor(BpmVariableSingleService bpmVariableSingleService)
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
                : ""
        };

        _bpmVariableSingleService.baseRepo.Insert(variableSingle);
    }

}