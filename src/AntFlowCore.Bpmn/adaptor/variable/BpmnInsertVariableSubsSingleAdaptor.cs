using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.variable;

public class BpmnInsertVariableSubsSingleAdaptor: IBpmnInsertVariableSubs
{
    public void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId)
    {
        // BpmVariableSingle entity has been removed; single assignee variable insertion is no longer supported
    }
}
