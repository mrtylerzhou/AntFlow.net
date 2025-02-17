using System.Collections.ObjectModel;
using antflowcore.adaptor;
using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.util;

namespace antflowcore.factory;

public class BpmnNodeAdaptorTagParser:TagParser<BpmnNodeAdaptor, BpmnNodeAdpConfEnum>
{
    public  BpmnNodeAdaptor ParseTag(BpmnNodeAdpConfEnum data)
    {
        if(data==null){
            throw new AFBizException("provided data to find a bpmnNodeAdaptor method is null");
        }

        IEnumerable<IAdaptorService> bpmnNodeAdaptors = ServiceProviderUtils.GetServices<IAdaptorService>();
        foreach (IAdaptorService bpmnNodeAdaptor in bpmnNodeAdaptors) {
            if(bpmnNodeAdaptor.IsSupportBusinessObject(data)){
                return (BpmnNodeAdaptor)bpmnNodeAdaptor;
            }
        }
        return null;
    }
}