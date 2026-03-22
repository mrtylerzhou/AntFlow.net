# Personnel Provider Templates

## Template 1: AbstractDifferentStandardAssignNodeAssigneeVoProvider

Use this base class when your rule needs to find users based on input user IDs. Examples include finding a user's manager, department leader, HRBP, etc.

```csharp
using antflowcore.constant.enums;
using antflowcore.service.interf.repository;
using antflowcore.vo;

namespace antflowcore.adaptor.personnel.provider;

/// <summary>
/// [你的规则名称]审批人提供者
/// [规则描述]
/// </summary>
public class YourRulePersonnelProvider : AbstractDifferentStandardAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;

    public YourRulePersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }

    /// <summary>
    /// 根据用户ID列表查询目标审批人
    /// </summary>
    /// <param name="userIds">用户ID列表</param>
    /// <returns>审批人列表</returns>
    protected override List<BaseIdTranStruVo> QueryUsers(List<string> userIds)
    {
        // 实现你的业务逻辑
        // 例如：根据用户ID查找其部门负责人、直属领导等
        return _userService.YourCustomQueryMethod(userIds);
    }
}
```

## Template 2: AbstractMissingAssignNodeAssigneeVoProvider

Use this base class when your rule needs completely custom logic for determining approvers. This provides full flexibility.

```csharp
using antflowcore.constant.enums;
using antflowcore.service.interf.repository;
using antflowcore.vo;
using antflowcore.exception;

namespace antflowcore.adaptor.personnel.provider;

/// <summary>
/// [你的规则名称]审批人提供者
/// [规则描述]
/// </summary>
public class YourRulePersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public YourRulePersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        IUserService userService,
        IRoleService roleService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
        _roleService = roleService;
    }

    /// <summary>
    /// 获取审批人列表
    /// </summary>
    /// 根据节点配置和启动条件获取审批人列表
    /// </summary>
    /// <param name="bpmnNodeVo">节点配置</param>
    /// <param name="startConditionsVo">启动条件</param>
    /// <returns>审批人列表</returns>
    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, 
        BpmnStartConditionsVo startConditionsVo)
    {
        // 1. 从节点配置或启动条件中获取必要的数据
        // var yourData = bpmnNodeVo.Property.YourData;
        // 或
        // var yourData = startConditionsVo.BusinessDataVo.YourData;
        
        // 2. 根据业务逻辑查询审批人
        // var users = YourBusinessLogic(yourData);
        
        // 3. 调用基类方法生成最终结果
        // return base.ProvideAssigneeList(bpmnNodeVo, users);
        
        throw new AFBizException("请实现具体的审批人获取逻辑");
    }
}
```

## Template 3: Form-Related Personnel Provider

Use this template when your rule needs to get approvers from form data or business data.

```csharp
using antflowcore.constant.enums;
using antflowcore.service.interf.repository;
using antflowcore.vo;
using antflowcore.exception;
using antflowcore.util;

namespace antflowcore.adaptor.personnel.provider;

/// <summary>
/// 表单关联[你的规则]审批人提供者
/// 从表单数据中获取审批人
/// </summary>
public class FormRelatedYourRulePersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public FormRelatedYourRulePersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        IUserService userService,
        IRoleService roleService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
        _roleService = roleService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, 
        BpmnStartConditionsVo startConditionsVo)
    {
        // 从BusinessDataVo获取表单关联数据
        var businessDataVo = startConditionsVo.BusinessDataVo;
        if (businessDataVo == null)
        {
            throw new AFBizException("业务数据不能为空");
        }

        // 获取节点配置中的表单关联审批人配置
        var nodeFormAssignees = businessDataVo.Node2formRelatedAssignees;
        if (nodeFormAssignees == null || !nodeFormAssignees.Any())
        {
            return new List<BpmnNodeParamsAssigneeVo>();
        }

        // 找到当前节点的配置
        var nodeConfig = nodeFormAssignees.FirstOrDefault(x => x.Key == bpmnNodeVo.NodeCode);
        if (nodeConfig.Value == null || !nodeConfig.Value.Any())
        {
            return new List<BpmnNodeParamsAssigneeVo>();
        }

        var result = new List<BpmnNodeParamsAssigneeVo>();

        foreach (var assigneeConfig in nodeConfig.Value)
        {
            var formAssigneeProperty = assigneeConfig.FormAssigneeProperty;
            var ids = assigneeConfig.Ids;

            List<BaseIdTranStruVo> users = formAssigneeProperty switch
            {
                // 根据不同类型查询用户
                NodeFormAssigneePropertyEnum.FORM_ASSIGNEE => _userService.QueryUserByIds(ids),
                NodeFormAssigneePropertyEnum.FORM_ROLE => _roleService.QueryUserByRoleIds(ids),
                // 添加更多类型...
                _ => throw new AFBizException($"不支持的审批人类型: {formAssigneeProperty}")
            };

            if (users != null && users.Any())
            {
                result.AddRange(users.Select(u => new BpmnNodeParamsAssigneeVo
                {
                    Assignee = u.Id,
                    AssigneeName = u.Name,
                    ElementName = assigneeConfig.ElementName ?? "审批人"
                }));
            }
        }

        return base.ProvideAssigneeList(bpmnNodeVo, result.Select(r => new BaseIdTranStruVo 
        { 
            Id = r.Assignee, 
            Name = r.AssigneeName 
        }).ToList());
    }
}
```

## Common Services Available

When implementing providers, you can inject these common services:

| Service | Purpose |
|---------|---------|
| `IUserService` | User-related queries (find users, managers, department leaders) |
| `IRoleService` | Role-related queries (find users by role) |
| `DepartmentService` | Department-related queries |
| `IBpmnProcessAdminProvider` | Process admin related queries |

## Key Methods in Base Classes

### AbstractDifferentStandardAssignNodeAssigneeVoProvider

- `QueryUsers(List<string> userIds)` - **Must override** - Query target approvers based on input user IDs
- `GetAssigneeList(...)` - Already implemented, handles approval standard logic

### AbstractMissingAssignNodeAssigneeVoProvider

- `GetAssigneeList(...)` - **Must override** - Full control over approver determination logic
- `ProvideAssigneeList(...)` - Helper method to convert users to assignee VOs
