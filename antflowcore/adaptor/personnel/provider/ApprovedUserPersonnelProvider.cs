using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

/// <summary>
/// 被审批人自己审批人提供者
/// 直接从启动条件的ApprovalEmpls中构建审批人列表
/// </summary>
public class ApprovedUserPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public ApprovedUserPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        string elementName = bpmnNodeVo.NodeName;
        return _assigneeVoBuildUtils.BuildVOs(startConditionsVo.ApprovalEmpls, elementName, false);
    }
}
