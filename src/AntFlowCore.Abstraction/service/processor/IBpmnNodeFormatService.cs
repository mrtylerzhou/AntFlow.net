using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.processor;

public interface IBpmnNodeFormatService
{
    List<BpmnConfCommonElementVo> GetBpmnConfCommonElementVoList(
        BpmnConfCommonVo bpmnConfCommonVo,
        List<BpmnNodeVo> nodes,
        BpmnStartConditionsVo bpmnStartConditions);
}
