# 节点类型详解

## 概述

AntFlowCore 定义了 11 种节点类型，覆盖了企业级工作流中的所有场景。节点类型通过 `NodeTypeEnum` 枚举定义，每种类型在流程中扮演不同的角色。

## 节点类型总览

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/NodeTypeEnum.cs
public enum NodeTypeEnum
{
    NODE_TYPE_START = 1,              // 发起人节点
    NODE_TYPE_GATEWAY = 2,            // 网关节点
    NODE_TYPE_CONDITIONS = 3,         // 条件节点
    NODE_TYPE_APPROVER = 4,           // 审批人节点
    NODE_TYPE_OUT_SIDE_CONDITIONS = 5, // 接入方条件节点
    NODE_TYPE_COPY = 6,               // 抄送节点
    NODE_TYPE_PARALLEL_GATEWAY = 7,   // 并行网关
    NODE_TYPE_COPY_V2 = 8,            // 抄送节点v2
    NODE_TYPE_AUTO_NODE = 9,          // 自动节点
    NODE_TYPE_CONDITION_APPROVE = 12, // 条件审批节点
    NODE_TYPE_CONDITION_COPY = 13     // 条件抄送节点
}
```

## 各节点类型详解

### 1. 发起人节点 (NODE_TYPE_START = 1)

流程的起始节点，标识流程的入口。每个流程模板有且仅有一个发起人节点。

```
┌─────────────────┐
│    🚀 开始       │
│   (发起人节点)    │
└────────┬────────┘
         │
         ▼
```

**特性**：
- 自动识别为流程起点
- 不需要配置审批人
- 发起人在提交表单时自动触发

### 2. 网关节点 (NODE_TYPE_GATEWAY = 2)

用于流程的逻辑控制，实现条件分支和并行分支。网关节点本身不产生审批任务，仅根据条件决定流程走向。

```
              ┌─────────────┐
              │   网关节点    │
              └──────┬──────┘
           ┌─────────┼─────────┐
           ▼         ▼         ▼
      ┌────────┐ ┌────────┐ ┌────────┐
      │ 分支 A  │ │ 分支 B  │ │ 分支 C  │
      └────────┘ └────────┘ └────────┘
```

### 3. 条件节点 (NODE_TYPE_CONDITIONS = 3)

根据预设条件决定流程走向的分支节点。条件节点支持嵌套 AND/OR 逻辑，可组合多个条件进行复杂判断。

```
         ┌───────────────┐
         │   条件节点      │
         │  金额 > 5000?   │
         └───────┬───────┘
           是 ╱     ╲ 否
            ▼       ▼
      ┌────────┐ ┌────────┐
      │ 总经理  │ │ 部门经理│
      │ 审批    │ │ 审批    │
      └────────┘ └────────┘
```

**条件节点属性**：
- `IsDefault`：是否为默认分支
- `Sort`：条件排序（按顺序评估）
- `GroupRelation`：条件组间关系（AND/OR）

### 4. 审批人节点 (NODE_TYPE_APPROVER = 4)

流程中最核心的节点类型，产生实际的审批任务。审批人节点支持 14 种审批人规则和 3 种签署类型。

```
┌─────────────────────────────┐
│        审批人节点             │
│  名称: 部门经理审批           │
│  属性: 指定角色               │
│  签署: 会签                   │
└─────────────────────────────┘
```

**关键配置项**：
- `NodeProperty`：审批人规则（指定人员、角色、直属领导等）
- `SignType`：签署类型（会签/或签/顺序会签）
- `ApprovalStandard`：审批标准（发起人/被审批人/上一节点审批人的）
- `IsDeduplication`：是否去重
- `IsSignUp`：是否允许加签

### 5. 接入方条件节点 (NODE_TYPE_OUT_SIDE_CONDITIONS = 5)

用于第三方系统接入场景的条件判断节点。条件由外部系统通过 API 传入，引擎根据外部返回的结果决定流程走向。

### 6. 抄送节点 (NODE_TYPE_COPY = 6)

将流程信息抄送给指定人员，抄送节点不产生审批任务，不影响流程流转。

```
┌──────────────┐
│   抄送节点    │  仅通知，不阻塞流程
│  抄送: 人事部 │
└──────────────┘
```

**特性**：
- 支持指定人员抄送
- 不影响流程流转
- 不在流程图中显示（V1版本）

### 7. 并行网关 (NODE_TYPE_PARALLEL_GATEWAY = 7)

实现并行分支的网关节点，允许流程同时进入多个分支并行执行。

```
              ┌─────────────┐
              │  并行网关     │
              └──────┬──────┘
           ┌─────────┼─────────┐
           ▼         ▼         ▼
      ┌────────┐ ┌────────┐ ┌────────┐
      │ 财务审批│ │ 人事审批│ │ 行政审批│
      └────┬───┘ └────┬───┘ └────┬───┘
           │         │         │
           ▼         ▼         ▼
              ┌─────────────┐
              │  聚合网关     │ 等待所有分支完成
              └──────┬──────┘
                     ▼
```

**并行流程说明**：
- 所有并行分支同时发起
- 需要等待所有分支完成后才会继续向后流转
- 任一分支拒绝则整个流程拒绝

### 8. 抄送节点V2 (NODE_TYPE_COPY_V2 = 8)

V2 版本的抄送节点，相比 V1 有显著增强：

```csharp
// 源码位置: AntFlowCore.Base/util/AfNodeUtils.cs:39-43
if (nodeType == (int)NodeTypeEnum.NODE_TYPE_COPY_V2)
{
    bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
    bpmnNodeVo.IsCarbonCopyNode = true;
}
```

**V2 vs V1 对比**：

| 特性 | V1 (NODE_TYPE_COPY) | V2 (NODE_TYPE_COPY_V2) |
|------|---------------------|----------------------|
| 是否在引擎中运行 | 否 | 是 |
| 选人规则 | 仅指定人员 | 支持全部14种规则 |
| 流程图展示 | 不显示 | 显示 |
| 条件配置 | 不支持 | 支持（条件抄送） |

### 9. 自动节点 (NODE_TYPE_AUTO_NODE = 9)

自动执行的节点，无需人工干预，到达后自动完成并继续流转。

```csharp
// 源码位置: AntFlowCore.Base/util/AfNodeUtils.cs:46-61
else if (nodeType == (int)NodeTypeEnum.NODE_TYPE_AUTO_NODE)
{
    bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
    bpmnNodeVo.IsAutomaticNode = true;
    bpmnNodeVo.NodeProperty = (int)NodePropertyEnum.NODE_PROPERTY_PERSONNEL;
    // 设置虚拟审批人 AUTO_NODE_SKIP(-3)
    AddOrEditProperty(bpmnNodeVo, prop => {
        prop.SignType ??= 1;
        prop.EmplIds = new List<string> { "-3" }; // AUTO_NODE_SKIP
    });
}
```

**自动节点特性**：
- 使用虚拟审批人 `-3`（AUTO_NODE_SKIP）
- 到达后自动通过
- 适用于系统自动处理的步骤（如数据校验、状态更新等）

### 10. 条件审批节点 (NODE_TYPE_CONDITION_APPROVE = 12)

将条件判断与审批人节点合并的特殊节点。条件满足时自动通过，条件不满足时需要人工审批。

```csharp
// 源码位置: AntFlowCore.Base/vo/BpmnNodeVo.cs
public bool? IsConditionApproveNode { get; set; }
```

**运行逻辑**：

```
         ┌─────────────────────┐
         │     条件审批节点      │
         │  条件: 金额 <= 10000  │
         └──────────┬──────────┘
                    │
         ┌──────────┴──────────┐
         ▼                     ▼
   条件满足               条件不满足
   自动通过               需要人工审批
         │                     │
         ▼                     ▼
   继续下一节点            等待审批人操作
```

**适用场景**：
- 小额自动通过，大额人工审批
- 常规场景自动处理，异常场景人工介入

### 11. 条件抄送节点 (NODE_TYPE_COPY_V2 = 13)

将条件判断与抄送节点合并的特殊节点。总是自动完成，但仅在条件满足时写抄送记录。

```csharp
// 源码位置: AntFlowCore.Base/vo/BpmnNodeVo.cs
public bool? IsConditionCopyNode { get; set; }
```

## 签署类型（会签/或签/顺序会签）

审批人节点支持三种签署类型，通过 `SignTypeEnum` 定义：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/SignTypeEnum.cs
public enum SignTypeEnum
{
    SIGN_TYPE_SIGN = 1,           // 会签
    SIGN_TYPE_OR_SIGN = 2,        // 或签
    SIGN_TYPE_SIGN_IN_ORDER = 3   // 顺序会签
}
```

### 会签（SIGN_TYPE_SIGN = 1）

需所有审批人同意，不限顺序。

```
┌─────────────────────────┐
│        会签节点          │
│  审批人: A, B, C        │
│  规则: 全部同意才算通过   │
└─────────────────────────┘

A 同意 ──┐
B 同意 ──┼──> 全部同意 → 通过
C 同意 ──┘

任一拒绝 → 拒绝
```

### 或签（SIGN_TYPE_OR_SIGN = 2）

只需一名审批人同意或拒绝即可。

```
┌─────────────────────────┐
│        或签节点          │
│  审批人: A, B, C        │
│  规则: 一人操作即生效     │
└─────────────────────────┘

A 同意 ──> 通过（B、C 不再审批）
A 拒绝 ──> 拒绝（B、C 不再审批）
```

### 顺序会签（SIGN_TYPE_SIGN_IN_ORDER = 3）

需所有审批人同意，且按照前端传入的顺序依次审批。

```
┌─────────────────────────┐
│      顺序会签节点         │
│  审批人: A → B → C      │
│  规则: 按顺序逐个审批     │
└─────────────────────────┘

A 同意 ──> B 同意 ──> C 同意 ──> 通过
    │          │          │
    ▼          ▼          ▼
  任一环节拒绝 → 整体拒绝
```

## 审批标准（ApprovalStandard）

审批标准决定审批人是相对于谁而言的：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/ApprovalStandardEnum.cs
public sealed class ApprovalStandardEnum
{
    public static readonly ApprovalStandardEnum START_USER = new(1, "发起人");
    public static readonly ApprovalStandardEnum APPROVAL = new(2, "被审批人");
    public static readonly ApprovalStandardEnum FROM_PREV_NODE = new(3, "上一节点审批人的");
}
```

| 标准 | 值 | 说明 | 示例 |
|------|---|------|------|
| 发起人 | 1 | 以发起人为基准 | 发起人的直属领导审批 |
| 被审批人 | 2 | 以被审批人为基准 | 被审批人的直属领导审批 |
| 上一节点审批人 | 3 | 以上一节点审批人为基准 | 上一节点审批人的直属领导审批 |

## 节点标签系统

AntFlowCore 使用标签系统标记特殊节点行为：

```csharp
// 源码位置: AntFlowCore.Base/vo/NodeLabelConstants.cs
public static class NodeLabelConstants
{
    public static readonly BpmnNodeLabelVO DynamicCondition;      // 动态条件节点
    public static readonly BpmnNodeLabelVO CopyNode;             // 抄送节点
    public static readonly BpmnNodeLabelVO CopyNodeV2;           // 抄送节点V2
    public static readonly BpmnNodeLabelVO AutomaticNode;         // 自动节点
    public static readonly BpmnNodeLabelVO SkippedAssignees;      // 跳过的审批人
    public static readonly BpmnNodeLabelVO ConditionApproveNode;  // 条件审批节点
    public static readonly BpmnNodeLabelVO ConditionCopyNode;     // 条件抄送节点
    public static readonly BpmnNodeLabelVO PrevNodeAppointed;     // 上一节点指定审批人
    public static readonly BpmnNodeLabelVO AppointNextNodeApprover; // 指定下一节点审批人
}
```

## 节点属性表（NodeProperty）

审批人节点支持的 14 种审批人规则，通过 `NodePropertyEnum` 定义：

| 属性 | 值 | 描述 | 参数类型 |
|------|---|------|---------|
| NODE_PROPERTY_LOOP | 2 | 层层审批 | 单人 |
| NODE_PROPERTY_LEVEL | 3 | 指定层级审批 | 单人 |
| NODE_PROPERTY_ROLE | 4 | 指定角色 | 多人 |
| NODE_PROPERTY_PERSONNEL | 5 | 指定人员 | 多人 |
| NODE_PROPERTY_HRBP | 6 | HRBP | 单人 |
| NODE_PROPERTY_CUSTOMIZE | 7 | 发起人自选 | 多人 |
| NODE_PROPERTY_BUSINESSTABLE | 8 | 关联业务表 | 多人 |
| NODE_PROPERTY_OUT_SIDE_ACCESS | 11 | 外部传入人员 | 多人 |
| NODE_PROPERTY_START_USER | 12 | 发起人自己 | 单人 |
| NODE_PROPERTY_DIRECT_LEADER | 13 | 直属领导 | 多人 |
| NODE_PROPERTY_APPROVED_USERS | 15 | 被审批人自己 | 多人 |
| NODE_PROPERTY_FORM_RELATED | 16 | 表单中相关人员 | 多人 |
| NODE_PROPERTY_ZDY_RULES | 17 | 自定义规则 | 多人 |
| NODE_PROPERTY_PREV_NODE_RELATED | 18 | 上一节点相关人员 | 多人 |

> 详细审批人规则说明请参考 [审批人规则详解](./approver-rules.md)。

## 节点特殊处理流程

在设计时保存前，`AfNodeUtils.NodeSpecialProcess` 方法会对特殊节点进行预处理：

```csharp
// 源码位置: AntFlowCore.Base/util/AfNodeUtils.cs
public static void NodeSpecialProcess(BpmnNodeVo bpmnNodeVo)
{
    // 1. 上一节点指定审批人 → 贴标签
    if (bpmnNodeVo.IsPrevNodeAppointed == true)
        bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.PrevNodeAppointed);

    // 2. 抄送V2节点 → 转为审批人节点 + 标记
    if (nodeType == NODE_TYPE_COPY_V2)
    {
        bpmnNodeVo.NodeType = NODE_TYPE_APPROVER;
        bpmnNodeVo.IsCarbonCopyNode = true;
    }
    
    // 3. 自动节点 → 转为审批人节点 + 设置虚拟审批人
    else if (nodeType == NODE_TYPE_AUTO_NODE)
    {
        bpmnNodeVo.NodeType = NODE_TYPE_APPROVER;
        bpmnNodeVo.IsAutomaticNode = true;
        // 设置虚拟审批人 AUTO_NODE_SKIP(-3)
    }
    
    // 4. 条件审批节点 → 转为审批人节点 + 标记
    else if (nodeType == NODE_TYPE_CONDITION_APPROVE)
    {
        bpmnNodeVo.NodeType = NODE_TYPE_APPROVER;
        bpmnNodeVo.IsConditionApproveNode = true;
    }
}
```

## 参数类型说明

每种审批人规则对应不同的参数类型：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/BpmnNodeParamTypeEnum.cs
public enum BpmnNodeParamTypeEnum
{
    BPMN_NODE_PARAM_SINGLE = 1,        // 单人
    BPMN_NODE_PARAM_MULTIPLAYER = 2,   // 多人
    Bpmn_NODE_PARAM_MULTIPLAYER_SORT = 3 // 多人有序
}
```
