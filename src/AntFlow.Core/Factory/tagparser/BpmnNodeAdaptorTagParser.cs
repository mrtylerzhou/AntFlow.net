using AntFlow.Core.Adaptor;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Exception;
using AntFlow.Core.Util;

namespace AntFlow.Core.Factory;

public class BpmnNodeAdaptorTagParser : TagParser<BpmnNodeAdaptor, BpmnNodeAdpConfEnum>
{
    public BpmnNodeAdaptor ParseTag(BpmnNodeAdpConfEnum data)
    {
        if (data == null)
        {
            throw new AFBizException("provided data to find a bpmnNodeAdaptor method is null");
        }

        IEnumerable<IAdaptorService> bpmnNodeAdaptors = ServiceProviderUtils.GetServices<IAdaptorService>();
        foreach (IAdaptorService bpmnNodeAdaptor in bpmnNodeAdaptors)
        {
            if (bpmnNodeAdaptor.IsSupportBusinessObject(data))
            {
                return (BpmnNodeAdaptor)bpmnNodeAdaptor;
            }
        }

        return null;
    }
}