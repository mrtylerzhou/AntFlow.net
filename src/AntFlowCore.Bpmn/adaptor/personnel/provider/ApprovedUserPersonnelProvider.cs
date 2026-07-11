using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

/// <summary>
/// Provider for "被审批人自己" nodes (nodeProperty = 15, NODE_PROPERTY_APPROVED_USERS).
/// Returns the approved person(s) passed in via BpmnStartConditionsVo.ApprovalEmpls
/// as the assignees directly. Requires approvalEmpls to be non-empty.
/// </summary>
[NamedService(nameof(ApprovedUserPersonnelProvider))]
public class ApprovedUserPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public ApprovedUserPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        if (startConditionsVo.ApprovalEmpls == null || !startConditionsVo.ApprovalEmpls.Any())
        {
            throw new AFBizException("审批标准为被审批人,但是无被审批人!");
        }

        string elementName = bpmnNodeVo.NodeName;
        return _assigneeVoBuildUtils.BuildVOs(startConditionsVo.ApprovalEmpls, elementName, false);
    }
}
