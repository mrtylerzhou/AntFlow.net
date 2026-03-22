using antflowcore.conf.di;
using antflowcore.constant.enums;
using AntFlowCore.Constant.Enums;
using antflowcore.exception;
using antflowcore.service.interf.repository;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.personnel.provider;

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

        List<BaseIdTranStruVo> assignees = new List<BaseIdTranStruVo>();

        if (node2formRelatedAssignees == null || !node2formRelatedAssignees.TryGetValue(id.ToString(), out var ids) || ids == null || ids.Count == 0)
        {
            _logger.LogWarning("节点:{NodeId},名称:{NodeName},未获取到表单相关人员配置", id, bpmnNodeVo.NodeName);
            return new List<BpmnNodeParamsAssigneeVo>();
        }

        int? formAssigneeProperty = bpmnNodeVo.Property?.FormAssigneeProperty;
        if (formAssigneeProperty == null)
        {
            throw new AFBizException("参数:formAssigneeProperty不能为空!");
        }

        NodeFormAssigneeProperty formAssigneePropertyEnum = NodeFormAssigneeProperty.GetByCode(formAssigneeProperty);
        if (formAssigneePropertyEnum == null)
        {
            throw new AFBizException("参数:formAssigneeProperty转换后为空!");
        }

        // 根据类型获取审批人
        if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormAssignee)
        {
            // 表单中的人员
            assignees = _userService.QueryUserByIds(ids);
        }
        else if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormRole)
        {
            // 表单中的角色
            assignees = _roleService.QueryUserByRoleIds(ids);
        }
        else if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormUserHrbp)
        {
            // 表单中人员的HRBP
            assignees = _userService.QueryEmployeeHrpbsByEmployeeIds(ids);
        }
        else if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormUserDirectLeader)
        {
            // 表单中人员的直属领导
            assignees = _userService.QueryEmployeeDirectLeaderByIds(ids);
        }
        else if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormUserDepartLeader)
        {
            // 表单中人员所在部门负责人
            assignees = _userService.QueryDepartmentLeaderByIds(ids);
        }
        else if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormDepartLeader)
        {
            // 表单中部门的负责人
            // TODO: 待实现
            _logger.LogWarning("表单中部门的负责人功能待实现");
        }
        else if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormUserLevelLeader)
        {
            // 表单中人员多级领导
            // TODO: 待实现
            _logger.LogWarning("表单中人员多级领导功能待实现");
        }
        else if (formAssigneePropertyEnum == NodeFormAssigneeProperty.FormUserLoopLeader)
        {
            // 表单中人员全部层级领导
            // TODO: 待实现
            _logger.LogWarning("表单中人员全部层级领导功能待实现");
        }
        else
        {
            throw new AFBizException("参数:formAssigneeProperty转换后为空!");
        }

        return base.ProvideAssigneeList(bpmnNodeVo, assignees);
    }
}
