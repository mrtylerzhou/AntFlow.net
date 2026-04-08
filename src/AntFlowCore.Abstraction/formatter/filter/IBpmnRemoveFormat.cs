using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.formatter.filter;

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