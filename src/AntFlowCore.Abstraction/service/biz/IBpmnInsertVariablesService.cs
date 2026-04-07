
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmnInsertVariablesService
{
    void InsertVariables(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions);
}
