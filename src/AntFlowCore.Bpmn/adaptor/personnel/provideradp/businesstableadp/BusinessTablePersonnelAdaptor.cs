using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using AntFlowCore.Core.constant.enums;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp.businesstableadp;

public class BusinessTablePersonnelAdaptor : AbstractBpmnPersonnelAdaptor {
        public BusinessTablePersonnelAdaptor(BusinessTablePersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
        {
        }
        
        public override void SetSupportBusinessObjects() {
            ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.BUSINESS_TABLE_PERSONNEL);
        }
}