using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Engine.Engine.service;

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