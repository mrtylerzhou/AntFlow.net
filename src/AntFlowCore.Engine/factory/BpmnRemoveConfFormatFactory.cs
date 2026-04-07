using AntFlowCore.Abstraction.factory;
using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Engine.factory;

public class BpmnRemoveConfFormatFactory : IBpmnRemoveConfFormatFactory
{
    private readonly IEnumerable<IBpmnRemoveFormat> _bpmnRemoveFormats;

    public BpmnRemoveConfFormatFactory(IEnumerable<IBpmnRemoveFormat> bpmnRemoveFormats)
    {
        _bpmnRemoveFormats = bpmnRemoveFormats;
    }
    public void RemoveBpmnConf (BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions){
        foreach (IBpmnRemoveFormat bpmnRemoveFormat in _bpmnRemoveFormats) {
            bpmnRemoveFormat.RemoveBpmnConf(bpmnConfVo,bpmnStartConditions);
        }
    }
}