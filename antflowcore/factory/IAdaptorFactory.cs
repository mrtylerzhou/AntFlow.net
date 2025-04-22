using antflowcore.adaptor.bpmnelementadp;
using antflowcore.adaptor.bpmnnodeadp;
using antflowcore.adaptor.formoperation;
using antflowcore.adaptor.personnel.provideradp.businesstableadp;
using antflowcore.adaptor.processoperation;
using antflowcore.constant.enums;
using antflowcore.factory.tagparser;
using AntFlowCore.Vo;

namespace antflowcore.factory;

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
    AbstractBusinessConfigurationAdaptor GetBusinessConfigurationAdaptor(ConfigurationTableAdapterEnum byTableFieldEnum);

    [AutoParse]
    BpmnElementAdaptor GetBpmnElementAdaptor(NodePropertyEnum nodePropertyEnum);
}