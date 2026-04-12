using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Bpmn.adaptor.personnel.provider;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

public class LoopPersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public LoopPersonnelAdaptor(LoopPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.NODE_LOOP_PERSONNEL);
    }
}