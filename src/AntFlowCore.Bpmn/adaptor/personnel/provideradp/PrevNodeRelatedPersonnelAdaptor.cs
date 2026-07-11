using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Bpmn.adaptor.personnel.provider;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

/// <summary>
/// Personnel adaptor for previous-node-related user nodes.
/// Maps PersonnelEnum.PREV_NODE_USERS_PERSONNEL to PrevNodeRelatedPersonnelProvider.
/// </summary>
public class PrevNodeRelatedPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public PrevNodeRelatedPersonnelAdaptor(
        PrevNodeRelatedPersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.PREV_NODE_USERS_PERSONNEL);
    }
}
