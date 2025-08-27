using AntFlow.Core.Service.Processor;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Factory;

public class BpmnStartFormatFactory
{
    public void formatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        IEnumerable<IBpmnStartFormat> startFormats = ServiceProviderUtils.GetServices<IBpmnStartFormat>();
        foreach (IBpmnStartFormat startFormat in startFormats)
        {
            //to check implementation
            startFormat.FormatBpmnConf(bpmnConfVo, bpmnStartConditions);
        }
    }
}