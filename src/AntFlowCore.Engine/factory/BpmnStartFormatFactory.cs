using AntFlowCore.Abstraction.factory;
using AntFlowCore.Abstraction.formatter;
using AntFlowCore.Common.util;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Engine.factory;

public class BpmnStartFormatFactory : IBpmnStartFormatFactory
{
    public void formatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions){
        IEnumerable<IBpmnStartFormat> startFormats = ServiceProviderUtils.GetServices<IBpmnStartFormat>();
        foreach (IBpmnStartFormat startFormat in startFormats) {
            //to check implementation
            startFormat.FormatBpmnConf(bpmnConfVo,bpmnStartConditions);
        }
    }
}