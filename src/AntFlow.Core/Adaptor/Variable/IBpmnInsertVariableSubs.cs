using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Variable;

public interface IBpmnInsertVariableSubs
{
    void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId);
}