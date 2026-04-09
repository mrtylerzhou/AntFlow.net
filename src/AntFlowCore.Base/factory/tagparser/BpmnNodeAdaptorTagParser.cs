using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;

namespace AntFlowCore.Base.factory.tagparser;

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