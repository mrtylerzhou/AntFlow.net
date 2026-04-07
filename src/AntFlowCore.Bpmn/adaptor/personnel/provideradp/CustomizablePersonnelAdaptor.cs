using AntFlowCore.Abstraction;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Engine.Engine.service;
using AntFlowCore.Extensions.Extensions.service;

namespace AntFlowCore.Extensions.Extensions.adaptor.personnel;

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