using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.bpmnnodeadp;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.exception;
using AntFlowCore.Core.util;

namespace AntFlowCore.Core.factory.tagparser;

public class BpmnNodeAdaptorTagParser:TagParser<IBpmnNodeAdaptor, BpmnNodeAdpConfEnum>
{
    public  IBpmnNodeAdaptor ParseTag(BpmnNodeAdpConfEnum data)
    {
        if(data==null){
            throw new AFBizException("provided data to find a bpmnNodeAdaptor method is null");
        }

        IEnumerable<IAdaptorService> bpmnNodeAdaptors = ServiceProviderUtils.GetServices<IAdaptorService>();
        foreach (IAdaptorService bpmnNodeAdaptor in bpmnNodeAdaptors) {
            if(bpmnNodeAdaptor.IsSupportBusinessObject(data)){
                return (IBpmnNodeAdaptor)bpmnNodeAdaptor;
            }
        }
        return null;
    }
}