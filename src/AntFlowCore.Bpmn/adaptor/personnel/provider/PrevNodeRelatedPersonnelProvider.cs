using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

/// <summary>
/// Provider for previous-node-related personnel nodes (nodeProperty = 18, NODE_PROPERTY_PREV_NODE_RELATED).
/// Gets assignee ids from ContextEmplList (set by AbstractBpmnPersonnelAdaptor from previous node's emplList),
/// then queries the actual approvers based on the FormAssigneeProperty type
/// (e.g. prev node person, prev node person's direct leader, prev node role, etc.).
/// </summary>
[NamedService(nameof(PrevNodeRelatedPersonnelProvider))]
public class PrevNodeRelatedPersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<PrevNodeRelatedPersonnelProvider> _logger;

    public PrevNodeRelatedPersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        IUserService userService,
        IRoleService roleService,
        ILogger<PrevNodeRelatedPersonnelProvider> logger) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        long id = bpmnNodeVo.Id;
        List<BaseIdTranStruVo> contextEmplList = bpmnNodeVo.Property?.ContextEmplList;

        if (contextEmplList == null || contextEmplList.Count == 0)
        {
            _logger.LogWarning("节点:{Id},名称:{NodeName},未获取到上一节点审批人配置", id, bpmnNodeVo.NodeName);
            return new List<BpmnNodeParamsAssigneeVo>();
        }

        List<string> ids = contextEmplList.Select(e => e.Id).ToList();

        int? prevNodeAssigneeProperty = bpmnNodeVo.Property?.FormAssigneeProperty;
        if (prevNodeAssigneeProperty == null)
        {
            throw new AFBizException("参数:formAssigneeProperty不能为空!");
        }

        NodePrevNodeAssigneePropertyEnum? prevNodeEnum = NodePrevNodeAssigneePropertyEnumExtensions.GetByCode(prevNodeAssigneeProperty);
        if (prevNodeEnum == null)
        {
            throw new AFBizException("formAssigneeProperty参数值未定义!");
        }

        List<BaseIdTranStruVo> assignees;
        switch (prevNodeEnum)
        {
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_ASSIGNEE:
                assignees = _userService.QueryUserByIds(ids);
                break;
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_ROLE:
                assignees = _roleService.QueryUserByRoleIds(ids);
                break;
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_USER_HRBP:
                assignees = _userService.QueryEmployeeHrpbsByEmployeeIds(ids);
                break;
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_USER_DIRECT_LEADER:
                assignees = _userService.QueryEmployeeDirectLeaderByIds(ids);
                break;
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_USER_DEPART_LEADER:
                // TODO: IUserService does not yet provide QueryDepartmentLeaderByIds
                throw new AFBizException("上一节点人员所在部门负责人暂未实现!");
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_DEPART_LEADER:
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_USER_LEVEL_LEADER:
            case NodePrevNodeAssigneePropertyEnum.PREV_NODE_USER_LOOP_LEADER:
                throw new AFBizException($"formAssigneeProperty类型:{prevNodeEnum}暂未实现!");
            default:
                throw new AFBizException("formAssigneeProperty参数值未定义!");
        }

        return ProvideAssigneeList(bpmnNodeVo, assignees ?? new List<BaseIdTranStruVo>());
    }
}
