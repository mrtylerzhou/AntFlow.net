
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service;

public interface IBpmnCreateAndStartService
{
    void CreateBpmnAndStart(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions);
}
