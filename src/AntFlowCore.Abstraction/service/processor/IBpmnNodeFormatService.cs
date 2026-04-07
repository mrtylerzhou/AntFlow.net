using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Extensions.Extensions.service.processor;

public interface IBpmnNodeFormatService
{
    List<BpmnConfCommonElementVo> GetBpmnConfCommonElementVoList(
        BpmnConfCommonVo bpmnConfCommonVo,
        List<BpmnNodeVo> nodes,
        BpmnStartConditionsVo bpmnStartConditions);
}
