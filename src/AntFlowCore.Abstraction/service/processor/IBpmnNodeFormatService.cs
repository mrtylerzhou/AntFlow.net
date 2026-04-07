using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.processor;

public interface IBpmnNodeFormatService
{
    List<BpmnConfCommonElementVo> GetBpmnConfCommonElementVoList(
        BpmnConfCommonVo bpmnConfCommonVo,
        List<BpmnNodeVo> nodes,
        BpmnStartConditionsVo bpmnStartConditions);
}
