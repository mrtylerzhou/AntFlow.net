using System.Collections.ObjectModel;
using antflowcore.service.processor;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.factory;

public class BpmnStartFormatFactory
{
    public void formatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions){
        IEnumerable<IBpmnStartFormat> startFormats = ServiceProviderUtils.GetServices<IBpmnStartFormat>();
        foreach (IBpmnStartFormat startFormat in startFormats) {
            //to check implementation
            startFormat.FormatBpmnConf(bpmnConfVo,bpmnStartConditions);
        }
    }
}