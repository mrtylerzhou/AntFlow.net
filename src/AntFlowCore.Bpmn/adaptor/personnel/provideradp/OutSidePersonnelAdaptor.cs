using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Engine.Engine.service;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

public class OutSidePersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public OutSidePersonnelAdaptor(OutSidePersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.OUT_SIDE_ACCESS_PERSONNEL);
    }
}