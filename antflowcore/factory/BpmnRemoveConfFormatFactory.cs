using antflowcore.service.processor.filter;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.factory;

public class BpmnRemoveConfFormatFactory
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