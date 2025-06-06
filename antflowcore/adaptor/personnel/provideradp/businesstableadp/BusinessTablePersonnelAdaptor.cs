﻿using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enus;
using antflowcore.service;

namespace antflowcore.adaptor.personnel.businesstableadp;

public class BusinessTablePersonnelAdaptor : AbstractBpmnPersonnelAdaptor {
        public BusinessTablePersonnelAdaptor(BusinessTablePersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
        {
        }
        
        public override void SetSupportBusinessObjects() {
            ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.BUSINESS_TABLE_PERSONNEL);
        }
}