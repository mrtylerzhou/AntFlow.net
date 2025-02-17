using AntFlowCore.Vo;

namespace antflowcore.adaptor.variable;

public interface IBpmnInsertVariableSubs
{
    void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId);
}