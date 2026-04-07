using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.bpmnnodeadp;

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