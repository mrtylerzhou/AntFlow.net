using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

/// <summary>
/// Provider for form-related personnel nodes (nodeProperty = 16, NODE_PROPERTY_FORM_RELATED).
/// Extracts assignee ids from the form data (stored in Node2formRelatedAssignees on BusinessDataVo),
/// then queries the actual approvers based on the FormAssigneeProperty type
/// (e.g. form person, form person's direct leader, form role, etc.).
/// </summary>
[NamedService(nameof(FormRelatedPersonnelProvider))]
public class FormRelatedPersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<FormRelatedPersonnelProvider> _logger;

    public FormRelatedPersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        IUserService userService,
        IRoleService roleService,
        ILogger<FormRelatedPersonnelProvider> logger) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        long id = bpmnNodeVo.Id;
        BusinessDataVo businessDataVo = startConditionsVo.BusinessDataVo;
        Dictionary<string, List<string>> node2formRelatedAssignees = businessDataVo?.Node2formRelatedAssignees;

        List<string> ids = null;
        if (node2formRelatedAssignees != null && node2formRelatedAssignees.TryGetValue(id.ToString(), out var assigneeIds))
        {
            ids = assigneeIds;
        }

        if (ids == null || ids.Count == 0)
        {
            _logger.LogWarning("节点:{Id},名称:{NodeName},未获取到表单相关人员配置", id, bpmnNodeVo.NodeName);
            return new List<BpmnNodeParamsAssigneeVo>();
        }

        int? formAssigneeProperty = bpmnNodeVo.Property?.FormAssigneeProperty;
        if (formAssigneeProperty == null)
        {
            throw new AFBizException("参数:formAssigneeProperty不能为空!");
        }

        NodeFormAssigneePropertyEnum? formAssigneePropertyEnum = NodeFormAssigneePropertyEnumExtensions.GetByCode(formAssigneeProperty);
        if (formAssigneePropertyEnum == null)
        {
            throw new AFBizException("formAssigneeProperty参数值未定义!");
        }

        List<BaseIdTranStruVo> assignees;
        switch (formAssigneePropertyEnum)
        {
            case NodeFormAssigneePropertyEnum.FORM_ASSIGNEE:
                assignees = _userService.QueryUserByIds(ids);
                break;
            case NodeFormAssigneePropertyEnum.FORM_ROLE:
                assignees = _roleService.QueryUserByRoleIds(ids);
                break;
            case NodeFormAssigneePropertyEnum.FORM_USER_HRBP:
                assignees = _userService.QueryEmployeeHrpbsByEmployeeIds(ids);
                break;
            case NodeFormAssigneePropertyEnum.FORM_USER_DIRECT_LEADER:
                assignees = _userService.QueryEmployeeDirectLeaderByIds(ids);
                break;
            case NodeFormAssigneePropertyEnum.FORM_USER_DEPART_LEADER:
                // TODO: .NET IUserService does not yet provide QueryDepartmentLeaderByIds
                throw new AFBizException("表单中人员所在部门负责人暂未实现!");
            case NodeFormAssigneePropertyEnum.FORM_DEPART_LEADER:
            case NodeFormAssigneePropertyEnum.FORM_USER_LEVEL_LEADER:
            case NodeFormAssigneePropertyEnum.FORM_USER_LOOP_LEADER:
                throw new AFBizException($"formAssigneeProperty类型:{formAssigneePropertyEnum}暂未实现!");
            default:
                throw new AFBizException("formAssigneeProperty参数值未定义!");
        }

        return ProvideAssigneeList(bpmnNodeVo, assignees ?? new List<BaseIdTranStruVo>());
    }
}
