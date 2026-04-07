using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core;
using AntFlowCore.Core.adaptor.bpmnnodeadp;
using AntFlowCore.Core.adaptor.formoperation;
using AntFlowCore.Core.adaptor.personnel.businesstableadp;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.factory;
using AntFlowCore.Core.factory.tagparser;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor;

public interface IAdaptorFactory
{
    [SpfService(typeof(BpmnNodeAdaptorTagParser))]
    [AutoParse]
    IBpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum adpConfEnum);
    
    [SpfService(typeof(ActivitiTagParser<>))]
    IFormOperationAdaptor<BusinessDataVo> GetActivitiService(BusinessDataVo dataVo);
    
    [SpfService(typeof(FormOperationTagParser))]
    IProcessOperationAdaptor GetProcessOperation(BusinessDataVo vo);

    [AutoParse]
    AbstractBusinessConfigurationAdaptor GetBusinessConfigurationAdaptor(ConfigurationTableAdapterEnum byTableFieldEnum);
    [AutoParse]
    BpmnElementAdaptor GetBpmnElementAdaptor(NodePropertyEnum nodePropertyEnum);
}