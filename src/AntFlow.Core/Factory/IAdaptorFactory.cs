using AntFlow.Core.Adaptor;
using AntFlow.Core.Adaptor.BpmnElementAdp;
using AntFlow.Core.Adaptor.Personnel.BusinessTableAdp;
using AntFlow.Core.Adaptor.ProcessOperation;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Factory.TagParser;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Factory;

public interface IAdaptorFactory
{
    //[SpfService(typeof(BpmnNodeAdaptorTagParser))]
    [AutoParse]
    BpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum adpConfEnum);


    [SpfService(typeof(ActivitiTagParser<>))]
    public IFormOperationAdaptor<BusinessDataVo> GetActivitiService(BusinessDataVo dataVo);

    [SpfService(typeof(FormOperationTagParser))]
    IProcessOperationAdaptor GetProcessOperation(BusinessDataVo vo);

    [AutoParse]
    AbstractBusinessConfigurationAdaptor
        GetBusinessConfigurationAdaptor(ConfigurationTableAdapterEnum byTableFieldEnum);

    [AutoParse]
    BpmnElementAdaptor GetBpmnElementAdaptor(NodePropertyEnum nodePropertyEnum);
}