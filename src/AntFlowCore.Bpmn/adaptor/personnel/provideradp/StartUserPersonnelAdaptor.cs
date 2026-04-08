using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.constant.enums;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

public class StartUserPersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public StartUserPersonnelAdaptor(StartUserPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.START_USER_PERSONNEL);
    }
}