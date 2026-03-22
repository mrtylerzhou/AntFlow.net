# Example Implementation: DepartmentLeaderPersonnelProvider

This document shows a complete implementation of the DepartmentLeaderPersonnelProvider as a reference example.

## Overview

The DepartmentLeaderPersonnelProvider finds the department leaders for users involved in the workflow. It demonstrates using `AbstractDifferentStandardAssignNodeAssigneeVoProvider` as the base class.

## Complete Implementation

### 1. Enum Definitions

**NodePropertyEnum.cs**
```csharp
public enum NodePropertyEnum
{
    // ... existing values ...
    NODE_PROPERTY_DEPARTMENT_LEADER = 14,
}

// Metadata mapping
{ NodePropertyEnum.NODE_PROPERTY_DEPARTMENT_LEADER, ("部门负责人", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) },
```

**BpmnNodeAdpConfEnum.cs**
```csharp
public enum BpmnNodeAdpConfEnum
{
    // ... existing values ...
    ADP_CONF_NODE_PROPERTY_DEPARTMENT_LEADER = 15,
}

// Mapping
{ BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_DEPARTMENT_LEADER, NodePropertyEnum.NODE_PROPERTY_DEPARTMENT_LEADER },
```

**PersonnelEnum.cs**
```csharp
public enum PersonnelEnum
{
    // ... existing values ...
    DEPARTMENT_LEADER_PERSONNEL,
}

// Mapping
{ PersonnelEnum.DEPARTMENT_LEADER_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_DEPARTMENT_LEADER, "部门负责人") },
```

### 2. DepartmentLeaderPersonnelProvider.cs

```csharp
using antflowcore.constant.enums;
using antflowcore.service.interf.repository;
using antflowcore.vo;

namespace antflowcore.adaptor.personnel.provider;

/// <summary>
/// 部门负责人审批人提供者
/// 根据用户所在部门查询部门负责人
/// </summary>
public class DepartmentLeaderPersonnelProvider : AbstractDifferentStandardAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;

    public DepartmentLeaderPersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }

    protected override List<BaseIdTranStruVo> QueryUsers(List<string> userIds)
    {
        return _userService.QueryDepartmentLeaderByIds(userIds);
    }
}
```

### 3. DepartmentLeaderPersonnelAdaptor.cs

```csharp
using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.service.interf;

namespace antflowcore.adaptor.personnel.provideradp;

/// <summary>
/// 部门负责人人员适配器
/// </summary>
public class DepartmentLeaderPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public DepartmentLeaderPersonnelAdaptor(
        DepartmentLeaderPersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) 
        : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.DEPARTMENT_LEADER_PERSONNEL);
    }
}
```

### 4. NodePropertyDepartmentLeaderAdaptor.cs

```csharp
using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnnodeadp;

/// <summary>
/// 部门负责人节点属性适配器
/// </summary>
public class NodePropertyDepartmentLeaderAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyDepartmentLeaderAdaptor(
        service.repository.BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        service.interf.repository.IRoleService roleService) 
        : base(bpmnNodeAdditionalSignConfService, roleService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_DEPARTMENT_LEADER);
    }
}
```

### 5. BpmnElementDepartmentLeaderAdaptor.cs

```csharp
using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 部门负责人BPMN元素适配器
/// </summary>
public class BpmnElementDepartmentLeaderAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "departmentLeader";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_DEPARTMENT_LEADER;
}
```

### 6. ServiceRegistration.cs

```csharp
// Provider registration
services.AddSingleton<DepartmentLeaderPersonnelProvider>();
services.AddSingleton<IBpmnPersonnelProviderService, DepartmentLeaderPersonnelProvider>();

// Personnel adapter registration
services.AddSingleton<AbstractBpmnPersonnelAdaptor, DepartmentLeaderPersonnelAdaptor>();

// Node adapter registration
services.AddSingleton<IAdaptorService, NodePropertyDepartmentLeaderAdaptor>();

// Element adapter registration
services.AddSingleton<IAdaptorService, BpmnElementDepartmentLeaderAdaptor>();
```

## File Locations Summary

| File | Location |
|------|----------|
| NodePropertyEnum.cs | `antflowcore/constant/enums/` |
| BpmnNodeAdpConfEnum.cs | `antflowcore/constant/enums/` |
| PersonnelEnum.cs | `antflowcore/constant/enums/` |
| DepartmentLeaderPersonnelProvider.cs | `antflowcore/adaptor/personnel/provider/` |
| DepartmentLeaderPersonnelAdaptor.cs | `antflowcore/adaptor/personnel/provideradp/` |
| NodePropertyDepartmentLeaderAdaptor.cs | `antflowcore/adaptor/bpmnnodeadp/` |
| BpmnElementDepartmentLeaderAdaptor.cs | `antflowcore/adaptor/bpmnelementadp/` |
| ServiceRegistration.cs | `antflowcore/conf/di/serviceregistration/` |
