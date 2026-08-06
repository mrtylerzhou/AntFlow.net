using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Bpmn.adaptor.personnel.provider;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

/// <summary>
/// Personnel adaptor for form-related user nodes.
/// Maps PersonnelEnum.FORM_USERS_PERSONNEL to FormRelatedPersonnelProvider.
/// </summary>
public class FormRelatedPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public FormRelatedPersonnelAdaptor(
        FormRelatedPersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.FORM_USERS_PERSONNEL);
    }
}
