using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Engine.factory.provider;

namespace AntFlowCore.Engine.provideradaptor.businesstableadp;

public class BusinessTablePersonnelAdaptor : AbstractBpmnPersonnelAdaptor {
        public BusinessTablePersonnelAdaptor(BusinessTablePersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
        {
        }
        
        public override void SetSupportBusinessObjects() {
            ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.BUSINESS_TABLE_PERSONNEL);
        }
}