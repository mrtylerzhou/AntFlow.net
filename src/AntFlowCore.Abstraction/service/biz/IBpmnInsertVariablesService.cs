
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmnInsertVariablesService
{
    void InsertVariables(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions);
}
