---
name: antflow-approver-rule
description: This skill should be used when the user wants to add a new custom approver rule to the AntFlow workflow engine. It provides step-by-step guidance for implementing new approval rules following the established patterns in the codebase. Use this skill when the user mentions adding approval rules, approver rules, or custom personnel types for workflow approval nodes.
---

# AntFlow Approver Rule Development

## Overview

This skill guides the implementation of new custom approver rules in the AntFlow workflow engine. Adding a new approver rule follows a consistent pattern that involves creating enumerations, providers, and adapters across multiple files.

## When to Use This Skill

Use this skill when the user requests to:
- Add a new approval rule or approver type
- Implement custom personnel selection for workflow nodes
- Create a new way to determine approvers based on business logic

## Implementation Steps

Adding a custom approver rule requires implementing the following 5 steps in order:

### Step 1: Add Enum to NodePropertyEnum

Location: `antflowcore/constant/enums/NodePropertyEnum.cs`

Add a new enum value with a unique code number (typically incrementing from the highest existing value):

```csharp
public enum NodePropertyEnum
{
    // ... existing values ...
    NODE_PROPERTY_YOUR_RULE = <next_code>,  // 你的规则描述
}
```

Also add the metadata mapping:

```csharp
private static readonly Dictionary<NodePropertyEnum, (string Desc, int HasPropertyTable, BpmnNodeParamTypeEnum ParamType)> Metadata =
    new()
    {
        // ... existing mappings ...
        { NodePropertyEnum.NODE_PROPERTY_YOUR_RULE, ("你的规则描述", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) },
    };
```

### Step 2: Add Enum to BpmnNodeAdpConfEnum

Location: `antflowcore/constant/enums/BpmnNodeAdpConfEnum.cs`

Add a new enum value for node adapter configuration:

```csharp
public enum BpmnNodeAdpConfEnum
{
    // ... existing values ...
    ADP_CONF_NODE_PROPERTY_YOUR_RULE = <next_code>,
}
```

Add the mapping to the dictionary:

```csharp
private static readonly Dictionary<BpmnNodeAdpConfEnum, Enum> MappingsDictionary = new()
{
    // ... existing mappings ...
    { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_YOUR_RULE, NodePropertyEnum.NODE_PROPERTY_YOUR_RULE },
};
```

### Step 3: Add Enum to PersonnelEnum

Location: `antflowcore/constant/enums/PersonnelEnum.cs`

Add the personnel enum value:

```csharp
public enum PersonnelEnum
{
    // ... existing values ...
    YOUR_RULE_PERSONNEL,
}
```

Add the mapping:

```csharp
private static readonly Dictionary<PersonnelEnum, (NodePropertyEnum NodeProperty, string Description)> PersonnelMappings =
    new()
    {
        // ... existing mappings ...
        { PersonnelEnum.YOUR_RULE_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_YOUR_RULE, "你的规则描述") },
    };
```

### Step 4: Implement Personnel Provider

Location: `antflowcore/adaptor/personnel/provider/YourRulePersonnelProvider.cs`

Create a provider class that either:
- Extends `AbstractDifferentStandardAssignNodeAssigneeVoProvider` (for rules based on existing users)
- Extends `AbstractMissingAssignNodeAssigneeVoProvider` (for rules that need custom logic)

See `references/provider_templates.md` for detailed code templates.

### Step 5: Implement Personnel Adapter

Location: `antflowcore/adaptor/personnel/provideradp/YourRulePersonnelAdaptor.cs`

Create an adapter that registers the provider:

```csharp
public class YourRulePersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public YourRulePersonnelAdaptor(
        YourRulePersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) 
        : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.YOUR_RULE_PERSONNEL);
    }
}
```

### Step 6: Implement Node Property Adapter

Location: `antflowcore/adaptor/bpmnnodeadp/NodePropertyYourRuleAdaptor.cs`

Create a node adapter (typically extends `AbstractAdditionSignNodeAdaptor`):

```csharp
public class NodePropertyYourRuleAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyYourRuleAdaptor(
        BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService) : base(bpmnNodeAdditionalSignConfService, roleService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_YOUR_RULE);
    }
}
```

### Step 7: Implement Element Adapter

Location: `antflowcore/adaptor/bpmnelementadp/BpmnElementYourRuleAdaptor.cs`

Choose the appropriate base class:
- `AbstractCommonBpmnElementSingleAdaptor` for single-person approvals
- `AbstractCommonBpmnElementMultiAdaptor` for multi-person approvals

```csharp
public class BpmnElementYourRuleAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "yourRule";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_YOUR_RULE;
}
```

### Step 8: Register Services

Location: `antflowcore/conf/di/serviceregistration/ServiceRegistration.cs`

Register all new services in the DI container:

```csharp
// Provider registration
services.AddSingleton<YourRulePersonnelProvider>();
services.AddSingleton<IBpmnPersonnelProviderService, YourRulePersonnelProvider>();

// Personnel adapter registration
services.AddSingleton<AbstractBpmnPersonnelAdaptor, YourRulePersonnelAdaptor>();

// Node adapter registration
services.AddSingleton<IAdaptorService, NodePropertyYourRuleAdaptor>();

// Element adapter registration
services.AddSingleton<IAdaptorService, BpmnElementYourRuleAdaptor>();
```

## Decision Guide

### Choosing Provider Base Class

| Scenario | Base Class |
|----------|------------|
| Rule finds users based on input user IDs (e.g., find user's manager, department leader) | `AbstractDifferentStandardAssignNodeAssigneeVoProvider` |
| Rule needs completely custom logic | `AbstractMissingAssignNodeAssigneeVoProvider` |

### Choosing Element Adapter Base Class

| Scenario | Base Class |
|----------|------------|
| Always single approver | `AbstractCommonBpmnElementSingleAdaptor` |
| Multiple approvers, supports or-sign/sign | `AbstractCommonBpmnElementMultiAdaptor` |

### Determining ParamType

| Approval Type | BpmnNodeParamTypeEnum |
|---------------|----------------------|
| Single approver | `BPMN_NODE_PARAM_SINGLE` |
| Multiple approvers | `BPMN_NODE_PARAM_MULTIPLAYER` |

## Resources

### references/provider_templates.md
Contains detailed code templates for personnel providers with different base classes.

### references/example_implementation.md
Contains a complete example implementation (DepartmentLeaderPersonnelProvider) for reference.
