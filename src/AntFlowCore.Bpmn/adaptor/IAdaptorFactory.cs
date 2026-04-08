using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.adaptor.personnel.businesstableadp;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.factory.tagparser;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor.bpmnelementadp;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core;
using AntFlowCore.Core.constant.enums;

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