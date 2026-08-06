# AntFlowCore .NET 审批人扩展指南

## 1. 概述

审批人扩展是 AntFlowCore 最常用的扩展点。当内置的 14 种审批人类型不满足业务需求时，可以通过实现自定义审批人适配器和审批人提供者来扩展。

## 2. 审批人系统架构

```
流程节点 (BpmnNodeVo)
     │
     │ nodeProperty = 审批人类型
     ▼
┌──────────────────────────────────────────┐
│  AbstractBpmnPersonnelAdaptor            │
│  (抽象基类 - 通用审批人设置逻辑)          │
│  - SetNodeParams()                       │
│  - SetEmployeeName()                     │
│  - AssigneeListUniq()                    │
└──────────────────┬───────────────────────┘
                   │
         ┌─────────┴─────────┐
         │                   │
    Personnel           Personnel
    Provider            Adaptor
    (找人)              (适配)
         │                   │
         ▼                   ▼
  ┌─────────────┐    ┌─────────────┐
  │IBpmnPersonnel│    │AbstractBpmn │
  │Provider      │    │Personnel    │
  │Service       │    │Adaptor      │
  │GetAssignee   │    │SetSupport   │
  │List()        │    │BusinessObjects│
  └─────────────┘    └─────────────┘
```

## 3. 内置审批人类型

### 3.1 审批人类型枚举

引擎使用 `PersonnelEnum` 枚举区分不同的审批人类型：

```csharp
public enum PersonnelEnum
{
    DIRECT_LEADER_PERSONNEL = 1,      // 直属领导
    CUSTOMIZABLE_PERSONNEL = 2,       // 指定人员
    ROLE_PERSONNEL = 3,               // 角色
    LEVEL_PERSONNEL = 4,              // 级别
    HRBP_PERSONNEL = 5,               // HRBP
    START_USER_PERSONNEL = 6,         // 发起人
    USER_POINTED_PERSONNEL = 7,       // 用户指定
    LOOP_PERSONNEL = 8,               // 循环
    OUTSIDE_PERSONNEL = 9,            // 外部系统
    BUSINESS_TABLE_PERSONNEL = 10,    // 业务表字段
    FORM_RELATED_PERSONNEL = 11,      // 表单关联
    UDR_PERSONNEL = 12,               // 用户自定义
    PREV_NODE_RELATED_PERSONNEL = 13, // 上一节点关联
    APPROVED_USERS_PERSONNEL = 14     // 已审批人
}
```

### 3.2 内置审批人提供者实现示例

#### DirectLeaderPersonnelProvider（直属领导）

```csharp
public class DirectLeaderPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProvider;

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        string startUserId = startConditionsVo.StartUserId;
        // 根据用户ID查询直属领导
        string leaderId = _employeeInfoProvider.GetLeaderId(startUserId);
        return new List<BpmnNodeParamsAssigneeVo>
        {
            new BpmnNodeParamsAssigneeVo { Assignee = leaderId }
        };
    }
}
```

#### RolePersonnelProvider（角色审批）

```csharp
[NamedService(nameof(RolePersonnelProvider))]
public class RolePersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _roleInfoProvider;

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        var propertysVo = bpmnNodeVo.Property;
        var roleIds = propertysVo.RoleIds;
        
        // 根据角色ID获取所有成员
        Dictionary<string, string> roleEmployeeInfo = 
            _roleInfoProvider.ProvideRoleEmployeeInfo(roleIds);

        List<BaseIdTranStruVo> baseIdTranStruVoList = roleEmployeeInfo
            .Select(a => new BaseIdTranStruVo(a.Key, a.Value))
            .ToList();
            
        return base.ProvideAssigneeList(bpmnNodeVo, baseIdTranStruVoList);
    }
}
```

#### BusinessTablePersonnelProvider（业务表字段）

```csharp
public class BusinessTablePersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly IAdaptorFactory _adaptorFactory;

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        BpmnNodePropertysVo property = bpmnNodeVo.Property;
        int? configurationTableType = property.ConfigurationTableType;
        int? tableFieldType = property.TableFieldType;
        
        // 通过适配器工厂获取业务配置适配器
        var tableFieldEnum = BusinessConfTableFieldEnumExtensions
            .GetTableFieldEnumByCode(tableFieldType.Value);
        var configTableEnum = ConfigurationTableAdapterEnumExtensions
            .GetByTableFieldEnum(tableFieldEnum);
        
        AbstractBusinessConfigurationAdaptor adaptor = 
            _adaptorFactory.GetBusinessConfigurationAdaptor(configTableEnum);
            
        return adaptor.doFindBusinessPerson(bpmnNodeVo, startConditionsVo);
    }
}
```

## 4. 自定义审批人扩展步骤

### Step 1: 定义新的审批人枚举

```csharp
public enum PersonnelEnum
{
    // ... 现有枚举值
    DEPARTMENT_MANAGER = 100,  // 部门经理审批
    PROJECT_MANAGER = 101       // 项目经理审批
}
```

### Step 2: 实现审批人提供者

```csharp
/// <summary>
/// 部门经理审批人提供者
/// </summary>
public class DepartmentManagerPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProvider;

    public DepartmentManagerPersonnelProvider(
        IDepartmentRepository departmentRepository,
        IBpmnEmployeeInfoProviderService employeeInfoProvider)
    {
        _departmentRepository = departmentRepository;
        _employeeInfoProvider = employeeInfoProvider;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        // 1. 获取发起人所在部门
        string startUserId = startConditionsVo.StartUserId;
        string deptId = _employeeInfoProvider.GetUserDepartmentId(startUserId);
        
        // 2. 查询部门经理
        Department dept = _departmentRepository.Find(d => d.Id == deptId).FirstOrDefault();
        if (dept == null || string.IsNullOrEmpty(dept.ManagerId))
        {
            // 找不到部门经理时返回空或设置默认审批人
            return new List<BpmnNodeParamsAssigneeVo>();
        }
        
        // 3. 返回审批人列表
        return new List<BpmnNodeParamsAssigneeVo>
        {
            new BpmnNodeParamsAssigneeVo 
            { 
                Assignee = dept.ManagerId,
                AssigneeName = dept.ManagerName
            }
        };
    }
}
```

### Step 3: 实现审批人适配器

```csharp
/// <summary>
/// 部门经理审批人适配器
/// </summary>
public class DepartmentManagerPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public DepartmentManagerPersonnelAdaptor(
        DepartmentManagerPersonnelProvider provider,
        IBpmnEmployeeInfoProviderService employeeService) 
        : base(provider, employeeService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(
            PersonnelEnum.DEPARTMENT_MANAGER);
    }
}
```

### Step 4: 注册到 DI 容器

```csharp
// 在 ServiceRegistration.AntFlowServiceSetUp 或自定义扩展方法中
services.AddSingleton<DepartmentManagerPersonnelProvider>();
services.AddSingleton<IBpmnPersonnelProviderService, DepartmentManagerPersonnelProvider>();
services.AddSingleton<AbstractBpmnPersonnelAdaptor, DepartmentManagerPersonnelAdaptor>();
```

### Step 5: 配置使用

在流程管理后台设计流程时，选择 `NodeProperty` 值为 `100`（对应 `DEPARTMENT_MANAGER`），引擎会自动使用自定义审批人提供者计算审批人。

## 5. 高级场景

### 5.1 动态审批人（基于业务数据）

```csharp
public class ProjectManagerPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProvider;

    public ProjectManagerPersonnelProvider(
        IProjectRepository projectRepository,
        IBpmnEmployeeInfoProviderService employeeInfoProvider)
    {
        _projectRepository = projectRepository;
        _employeeInfoProvider = employeeInfoProvider;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        // 从业务条件中获取项目ID
        if (!startConditionsVo.Conditions.TryGetValue("projectId", out var projectId))
        {
            throw new AFBizException("缺少项目ID参数");
        }

        // 查询项目经理
        var project = _projectRepository
            .Find(p => p.Id.ToString() == projectId?.ToString())
            .FirstOrDefault();
            
        if (project == null)
        {
            throw new AFBizException("项目不存在");
        }

        return new List<BpmnNodeParamsAssigneeVo>
        {
            new BpmnNodeParamsAssigneeVo
            {
                Assignee = project.ManagerId,
                AssigneeName = project.ManagerName
            }
        };
    }
}
```

### 5.2 会签审批人（多人审批）

```csharp
public class MultiDepartmentManagerProvider : IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        // 获取所有相关部门的经理
        var managerIds = startConditionsVo.Conditions
            .Where(c => c.Key.StartsWith("deptManager_"))
            .Select(c => c.Value?.ToString())
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        return managerIds.Select(id => new BpmnNodeParamsAssigneeVo
        {
            Assignee = id,
            AssigneeName = GetEmployeeName(id)
        }).ToList();
    }
}
```

### 5.3 审批人委托

引擎内置了审批人委托机制，通过 `BpmFlowrunEntrust` 表记录委托关系。当计算审批人时，引擎会自动检查委托配置：

```csharp
// 委托类型说明
// Type = 1: 委托任务（A委托给B处理）
// Type = 2: 转交任务（A转交给B处理）
// ActionType = 0: 全局用户配置的委托
// ActionType = 1: 变更处理人委托
// ActionType = 2: 添加处理人委托
// ActionType = 3: 移除处理人委托
```

### 5.4 找不到审批人的处理

引擎通过 `AbstractMissingAssignNodeAssigneeVoProvider` 提供默认处理：

```csharp
public abstract class AbstractMissingAssignNodeAssigneeVoProvider
{
    protected readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;
    protected readonly IBpmnProcessAdminProvider _processAdminProvider;

    protected List<BpmnNodeParamsAssigneeVo> ProvideAssigneeList(
        BpmnNodeVo bpmnNodeVo, List<BaseIdTranStruVo> employeeList)
    {
        // 如果找不到审批人，回退到流程管理员
        if (employeeList == null || !employeeList.Any())
        {
            return _processAdminProvider.GetAdminList(bpmnNodeVo);
        }
        // 正常返回审批人列表
        return employeeList.Select(e => new BpmnNodeParamsAssigneeVo
        {
            Assignee = e.Id,
            AssigneeName = e.Name
        }).ToList();
    }
}
```

## 6. 审批人提供者接口详解

### 6.1 IBpmnPersonnelProviderService

```csharp
public interface IBpmnPersonnelProviderService
{
    /// <summary>
    /// 获取审批人列表
    /// </summary>
    /// <param name="bpmnNodeVo">节点配置（包含节点属性、参数等）</param>
    /// <param name="startConditionsVo">流程启动条件（包含发起人、业务数据等）</param>
    /// <returns>审批人列表</returns>
    List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo);
}
```

### 6.2 BpmnNodeVo 中的关键属性

```csharp
public class BpmnNodeVo
{
    public string NodeId { get; set; }              // 节点ID
    public int? NodeProperty { get; set; }           // 节点属性（审批人类型）
    public BpmnNodePropertysVo Property { get; set; } // 节点详细属性
    public BpmnNodeParamsVo Params { get; set; }     // 节点参数
    public int? IsOutSideProcess { get; set; }       // 是否外部流程
    public int? ApprovalStandard { get; set; }       // 审批标准
}
```

### 6.3 BpmnStartConditionsVo 中的关键属性

```csharp
public class BpmnStartConditionsVo
{
    public string StartUserId { get; set; }           // 发起人ID
    public string StartUserName { get; set; }         // 发起人姓名
    public Dictionary<string, object> Conditions { get; set; } // 业务条件
    public BusinessDataVo BusinessDataVo { get; set; } // 业务数据VO
}
```

## 7. 审批人参数类型

```csharp
public enum BpmnNodeParamTypeEnum
{
    BPMN_NODE_PARAM_SINGLE = 1,      // 单人审批
    BPMN_NODE_PARAM_MULTIPLAYER = 2  // 多人审批（会签/或签）
}
```

单人审批时设置 `Params.Assignee`，多人审批时设置 `Params.AssigneeList`。

## 8. 注意事项

1. **返回值不可为空**：`GetAssigneeList` 必须返回非空列表，找不到审批人时应回退到流程管理员
2. **去重**：引擎会自动对审批人列表去重，无需在提供者中处理
3. **事务**：提供者方法不应开启事务，由上层统一控制
4. **性能**：审批人计算会被频繁调用，注意缓存和性能优化
5. **异常处理**：使用 `AFBizException` 抛出业务异常，引擎会捕获并返回友好错误信息

## 9. 调试技巧

```csharp
// 在提供者中添加日志
_logger.LogInformation(
    "计算审批人 - 节点:{NodeId}, 发起人:{UserId}, 结果:{Count}人", 
    bpmnNodeVo.NodeId, 
    startConditionsVo.StartUserId, 
    assigneeList.Count);

// 检查节点属性
if (bpmnNodeVo.Property == null)
{
    _logger.LogWarning("节点 {NodeId} 的 Property 为空", bpmnNodeVo.NodeId);
}
```
