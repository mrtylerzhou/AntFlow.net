using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

public class CustomizablePersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public CustomizablePersonnelAdaptor(CustomizePersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.CUSTOMIZABLE_PERSONNEL);
    }
}