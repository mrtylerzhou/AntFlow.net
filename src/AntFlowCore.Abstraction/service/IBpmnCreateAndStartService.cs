
using AntFlowCore.Vo;

namespace AntFlowCore.Extensions.service;

public interface IBpmnCreateAndStartService
{
    void CreateBpmnAndStart(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions);
}
