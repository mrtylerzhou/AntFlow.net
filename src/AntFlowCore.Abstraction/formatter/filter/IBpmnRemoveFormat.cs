using AntFlowCore.Base.interf;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

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