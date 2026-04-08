

using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.variable;

public interface IBpmnInsertVariableSubs
{
    void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId);
}