

using AntFlowCore.Vo;

namespace AntFlowCore.Extensions.Extensions.adaptor.variable;

public interface IBpmnInsertVariableSubs
{
    void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId);
}