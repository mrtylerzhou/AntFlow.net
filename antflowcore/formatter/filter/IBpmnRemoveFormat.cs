using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.filter;

public interface IBpmnRemoveFormat: IOrderedService
{
    /// <summary>
    /// Remove unused node
    /// </summary>
    /// <param name="bpmnConfVo"></param>
    /// <param name="bpmnStartConditions"></param>
    void RemoveBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
    List<Func<bool>> TrueFuncs(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditionsVo);
}