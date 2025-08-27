using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Variable;

public class BpmnInsertVariableSubsSingleAdaptor : IBpmnInsertVariableSubs
{
    private readonly BpmVariableSingleService _bpmVariableSingleService;

    public BpmnInsertVariableSubsSingleAdaptor(BpmVariableSingleService bpmVariableSingleService)
    {
        _bpmVariableSingleService = bpmVariableSingleService;
    }

    public void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId)
    {
        IDictionary<string, string>? assigneeMap = elementVo.AssigneeMap;

        BpmVariableSingle? variableSingle = new()
        {
            VariableId = variableId,
            ElementId = elementVo.ElementId,
            ElementName = elementVo.ElementName,
            NodeId = elementVo.NodeId,
            AssigneeParamName = elementVo.AssigneeParamName,
            Assignee = elementVo.AssigneeParamValue,
            AssigneeName = assigneeMap != null && assigneeMap.ContainsKey(elementVo.AssigneeParamValue)
                ? assigneeMap[elementVo.AssigneeParamValue] ?? ""
                : "",
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };

        _bpmVariableSingleService.baseRepo.Insert(variableSingle);
    }
}