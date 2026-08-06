# 14种审批人规则详解

## 概述

AntFlowCore 提供 14 种内置审批人规则，覆盖企业办公中几乎所有审批人指定场景。通过 `NodePropertyEnum` 枚举定义，每种规则对应不同的参数类型和人员获取方式。

## 审批人规则总览

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/NodePropertyEnum.cs
public enum NodePropertyEnum
{
    NODE_PROPERTY_LOOP = 2,           // 层层审批
    NODE_PROPERTY_LEVEL = 3,          // 指定层级审批
    NODE_PROPERTY_ROLE = 4,           // 指定角色
    NODE_PROPERTY_PERSONNEL = 5,      // 指定人员
    NODE_PROPERTY_HRBP = 6,           // HRBP
    NODE_PROPERTY_CUSTOMIZE = 7,      // 发起人自选
    NODE_PROPERTY_BUSINESSTABLE = 8,  // 关联业务表
    NODE_PROPERTY_OUT_SIDE_ACCESS = 11, // 外部传入人员
    NODE_PROPERTY_START_USER = 12,    // 发起人自己
    NODE_PROPERTY_DIRECT_LEADER = 13, // 直属领导
    NODE_PROPERTY_APPROVED_USERS = 15, // 被审批人自己
    NODE_PROPERTY_FORM_RELATED = 16,  // 表单中相关人员
    NODE_PROPERTY_ZDY_RULES = 17,     // 自定义规则
    NODE_PROPERTY_PREV_NODE_RELATED = 18 // 上一节点相关人员
}
```

## 审批人规则与枚举映射

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/PersonnelEnum.cs
public enum PersonnelEnum
{
    NODE_LOOP_PERSONNEL,         // 层层审批
    NODE_LEVEL_PERSONNEL,        // 指定层级审批
    ROLE_PERSONNEL,              // 指定角色
    USERAPPOINTED_PERSONNEL,     // 指定人员
    CUSTOMIZABLE_PERSONNEL,      // 发起人自选
    HRBP_PERSONNEL,              // HRBP
    OUT_SIDE_ACCESS_PERSONNEL,   // 外部传入人员
    START_USER_PERSONNEL,        // 发起人自己
    DIRECT_LEADER_PERSONNEL,     // 直属领导
    APPROVED_USERS_PERSONNEL,    // 被审批人自己
    BUSINESS_TABLE_PERSONNEL,    // 关联业务表
    FORM_USERS_PERSONNEL,        // 表单中相关人员
    UDR_USERS_PERSONNEL,         // 用户自定义规则人员
    PREV_NODE_USERS_PERSONNEL    // 上一节点上下文人员
}
```

## 逐条详解

### 1. 层层审批 (NODE_PROPERTY_LOOP = 2)

**描述**：从发起人的直属领导开始，逐层向上审批，直到满足终止条件。

**参数类型**：单人 (BPMN_NODE_PARAM_SINGLE)

**配置项**：
- `LoopEndType`：终止类型（按层级/按指定人员/按职级）
- `LoopNumberPlies`：审批层数
- `LoopEndGrade`：终止职级
- `LoopEndPersonList`：终止人员列表

```
┌─────────────┐
│   发起人 A   │
└──────┬──────┘
       │ 层层审批（终止：总经理）
       ▼
┌─────────────┐
│  直属领导 B  │ ← 第一层
└──────┬──────┘
       ▼
┌─────────────┐
│   总监 C     │ ← 第二层
└──────┬──────┘
       ▼
┌─────────────┐
│  总经理 D    │ ← 终止（到达终止人员）
└─────────────┘
```

**适用场景**：按组织层级逐级上报的审批场景，如晋升审批、大额采购审批。

---

### 2. 指定层级审批 (NODE_PROPERTY_LEVEL = 3)

**描述**：审批发起人的指定层级领导，如直属上级、二级上级等。

**参数类型**：单人 (BPMN_NODE_PARAM_SINGLE)

**配置项**：
- `AssignLevelType`：层级类型
- `AssignLevelGrade`：指定层级数（1=直属领导，2=二级领导，以此类推）

```
┌─────────────┐
│   员工 A     │  ← Level 0
└──────┬──────┘
       │ Level 1（直属领导）
       ▼
┌─────────────┐
│   经理 B     │  ← Level 1
└──────┬──────┘
       │ Level 2（总监）
       ▼
┌─────────────┐
│   总监 C     │  ← Level 2
└─────────────┘
```

**适用场景**：跨层级审批，如跳过直属领导直接由总监审批。

---

### 3. 指定角色 (NODE_PROPERTY_ROLE = 4)

**描述**：拥有指定角色的人员作为审批人。

**参数类型** : 多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**配置项**：
- `RoleIds`：角色ID列表
- `RoleList`：角色对象列表

```json
{
  "nodeProperty": 4,
  "property": {
    "roleIds": ["role_001", "role_002"],
    "roleList": [
      { "id": "role_001", "name": "财务审批人" },
      { "id": "role_002", "name": "人事审批人" }
    ]
  }
}
```

**适用场景**：按职能角色分配审批人，如"财务审批人"、"法务审批人"。

---

### 4. 指定人员 (NODE_PROPERTY_PERSONNEL = 5)

**描述**：直接指定具体人员作为审批人。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**配置项**：
- `EmplIds`：人员ID列表
- `EmplList`：人员对象列表

```json
{
  "nodeProperty": 5,
  "property": {
    "emplIds": ["emp_001", "emp_002"],
    "emplList": [
      { "id": "emp_001", "name": "张三" },
      { "id": "emp_002", "name": "李四" }
    ]
  }
}
```

**适用场景**：审批人固定且明确的场景，如"CEO审批"、"财务总监审批"。

---

### 5. HRBP (NODE_PROPERTY_HRBP = 6)

**描述**：由发起人对应的 HRBP（人力资源业务伙伴）作为审批人。

**参数类型**：单人 (BPMN_NODE_PARAM_SINGLE)

**配置项**：
- `HrbpConfType`：HRBP配置类型（按部门/按人员）

```
┌─────────────┐
│  发起人 A    │  部门：技术部
└──────┬──────┘
       │ 查找对应HRBP
       ▼
┌─────────────┐
│  HRBP B      │  技术部对应的HRBP
└─────────────┘
```

**适用场景**：人事相关审批，如入职审批、转正审批、离职审批。

---

### 6. 发起人自选 (NODE_PROPERTY_CUSTOMIZE = 7)

**描述**：发起人在提交表单时自行选择审批人。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**配置项**：
- `FunctionId`：关联功能ID
- `FunctionName`：功能名称

```
┌─────────────┐
│   发起人     │
│  请选择审批人 │  ← 发起时弹窗选择
└──────┬──────┘
       ▼
┌─────────────┐
│  自选审批人  │
└─────────────┘
```

**适用场景**：审批人不固定、需要发起人根据实际情况选择的场景。

---

### 7. 关联业务表 (NODE_PROPERTY_BUSINESSTABLE = 8)

**描述**：从业务数据表中获取审批人信息。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**配置项**：
- `ConfigurationTableType`：配置表类型
- `TableFieldType`：字段类型

**适用场景**：审批人存储在业务系统中的场景，如项目审批人、客户负责人。

---

### 8. 外部传入人员 (NODE_PROPERTY_OUT_SIDE_ACCESS = 11)

**描述**：审批人由外部系统通过 API 参数传入，适用于第三方系统接入场景。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**适用场景**：SaaS 多租户场景，外部系统通过 Open API 发起流程时指定审批人。

---

### 9. 发起人自己 (NODE_PROPERTY_START_USER = 12)

**描述**：审批人就是发起人本人。

**参数类型**：单人 (BPMN_NODE_PARAM_SINGLE)

**适用场景**：自我确认环节，如"本人确认信息无误"、"本人承诺"。

---

### 10. 直属领导 (NODE_PROPERTY_DIRECT_LEADER = 13)

**描述**：以发起人为基准（或根据审批标准配置），由其直属领导审批。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**说明**：直属领导的确定取决于 `ApprovalStandard` 配置：
- 审批标准 = 发起人(1)：发起人的直属领导
- 审批标准 = 被审批人(2)：被审批人的直属领导
- 审批标准 = 上一节点审批人(3)：上一节点审批人的直属领导

```
审批标准 = 发起人:
┌─────────────┐
│   发起人 A   │
└──────┬──────┘
       │ 直属领导
       ▼
┌─────────────┐
│   经理 B     │
└─────────────┘
```

**适用场景**：最常见的审批场景，如请假审批、报销审批。

---

### 11. 被审批人自己 (NODE_PROPERTY_APPROVED_USERS = 15)

**描述**：审批人为被审批人（即流程目标对象）自己。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**适用场景**：涉及特定人员的事件审批，如某人的人事调动需要当事人确认。

---

### 12. 表单中相关人员 (NODE_PROPERTY_FORM_RELATED = 16)

**描述**：审批人从表单数据中提取，由表单中的指定字段决定。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**配置项**：
- `FormInfos`：表单字段信息
- `FormAssigneeProperty`：表单审批人属性类型

```json
{
  "nodeProperty": 16,
  "property": {
    "formInfos": [
      { "id": "project_manager", "name": "项目经理" }
    ],
    "formAssigneeProperty": 1
  }
}
```

**适用场景**：审批人与表单内容关联，如表单中填写的项目经理作为审批人。

---

### 13. 自定义规则 (NODE_PROPERTY_ZDY_RULES = 17)

**描述**：通过自定义逻辑（UDR - User Defined Rule）确定审批人，支持最灵活的场景。

**参数类型**：多人 (BPMN_NODE_PARAM_MULTIPLAYER)

**配置项**：
- `UdrAssigneeProperty`：UDR审批人属性
- `UdrValueJson`：UDR值JSON

```
┌─────────────┐
│   流程引擎   │
└──────┬──────┘
       │ 调用自定义规则
       ▼
┌─────────────┐
│  UDR 服务    │  用户实现 IBpmnUdrService
│  自定义逻辑   │  可访问数据库、外部API等
└──────┬──────┘
       ▼
    审批人列表
```

**适用场景**：
- 根据业务数据动态计算审批人
- 需要调用外部系统获取审批人
- 复杂的多条件组合判断

---

### 14. 上一节点相关人员 (NODE_PROPERTY_PREV_NODE_RELATED = 18)

**描述**：以上一节点审批人的上下文信息确定审批人，如上一节点审批人的直属领导。

**参数类型**：多人 (Bpmn_NODE_PARAM_MULTIPLAYER)

**适用场景**：需要参考上一节点审批结果的升级审批场景。

---

## 缺失审批人处理策略

当审批人规则无法解析出有效审批人时，系统提供三种处理策略：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/MissingAssigneeProcessStrategyEnum.cs
public sealed class MissingAssigneeProcessStrategyEnum
{
    public static readonly MissingAssigneeProcessStrategyEnum NOTALLOWED = new(0, "不允许发起");
    public static readonly MissingAssigneeProcessStrategyEnum SKIP = new(1, "跳过");
    public static readonly MissingAssigneeProcessStrategyEnum TRANSFERTOADMIN = new(2, "转办给管理员");
}
```

| 策略 | 值 | 说明 |
|------|---|------|
| 不允许发起 | 0 | 流程无法发起，提示用户配置错误 |
| 跳过 | 1 | 不生成该节点的审批任务，直接进入下一节点 |
| 转办给管理员 | 2 | 转给管理员需实现 `IBpmnProcessAdminProvider` 接口 |

---

## 参数类型对照表

| NodeProperty | 枚举值 | 参数类型 | 说明 |
|-------------|-------|---------|------|
| NODE_PROPERTY_LOOP | 2 | SINGLE | 层层审批结果为单人 |
| NODE_PROPERTY_LEVEL | 3 | SINGLE | 指定层级结果为单人 |
| NODE_PROPERTY_ROLE | 4 | MULTIPLAYER | 角色下可能有多个人员 |
| NODE_PROPERTY_PERSONNEL | 5 | MULTIPLAYER | 可指定多个人员 |
| NODE_PROPERTY_HRBP | 6 | SINGLE | 一个员工对应一个HRBP |
| NODE_PROPERTY_CUSTOMIZE | 7 | MULTIPLAYER | 发起人可选多人 |
| NODE_PROPERTY_BUSINESSTABLE | 8 | MULTIPLAYER | 业务表可能返回多人 |
| NODE_PROPERTY_OUT_SIDE_ACCESS | 11 | MULTIPLAYER | 外部可传入多人 |
| NODE_PROPERTY_START_USER | 12 | SINGLE | 发起人只有一个人 |
| NODE_PROPERTY_DIRECT_LEADER | 13 | MULTIPLAYER | 一人可能有多个上级 |
| NODE_PROPERTY_APPROVED_USERS | 15 | MULTIPLAYER | 被审批人可能是多人 |
| NODE_PROPERTY_FORM_RELATED | 16 | MULTIPLAYER | 表单可能关联多人 |
| NODE_PROPERTY_ZDY_RULES | 17 | MULTIPLAYER | 自定义规则返回多人 |
| NODE_PROPERTY_PREV_NODE_RELATED | 18 | MULTIPLAYER | 上一节点可能有多人 |

---

## 审批人适配器架构

AntFlowCore 使用适配器模式处理不同审批人规则的解析：

```
                    ┌─────────────────────┐
                    │  IAdaptorFactory     │
                    │  适配器工厂          │
                    └──────────┬──────────┘
                               │
        ┌──────────────────────┼──────────────────────┐
        ▼                      ▼                      ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ 循环审批适配器 │    │ 角色审批适配器 │    │ 人员审批适配器 │
│ LoopAdaptor  │    │ RoleAdaptor  │    │ PersonnelAdp │
└──────────────┘    └──────────────┘    └──────────────┘
```

适配器配置映射（`BpmnNodeAdpConfEnum`）：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/BpmnNodeAdpConfEnum.cs
public enum BpmnNodeAdpConfEnum
{
    ADP_CONF_NODE_PROPERTY_LOOP = 1,
    ADP_CONF_NODE_PROPERTY_LEVEL = 2,
    ADP_CONF_NODE_PROPERTY_ROLE = 3,
    ADP_CONF_NODE_PROPERTY_PERSONNEL = 4,
    ADP_CONF_NODE_TYPE_CONDITIONS = 5,
    ADP_CONF_NODE_TYPE_COPY = 6,
    ADP_CONF_NODE_TYPE_OUT_SIDE_CONDITIONS = 7,
    ADP_CONF_NODE_PROPERTY_OUT_SIDE_ACCESS = 8,
    ADP_CONF_NODE_PROPERTY_START_USER = 9,
    ADP_CONF_NODE_PROPERTY_HRBP = 10,
    ADP_CONF_NODE_PROPERTY_BUSINESSTABLE = 11,
    ADP_CONF_NODE_PROPERTY_DIRECT_LEADER = 12,
    ADP_CONF_NODE_PROPERTY_CUSTOMIZE = 13,
    ADP_CONF_NODE_PROPERTY_FORM_RELATED_USERS = 14,
    ADP_CONF_NODE_PROPERTY_UDR_USERS = 15,
    ADP_CONF_NODE_PROPERTY_PREV_NODE_RELATED_USERS = 16,
    ADP_CONF_NODE_PROPERTY_APPROVED_USERS = 17,
}
```

---

## 审批人去重

AntFlowCore 支持节点级别的去重配置（`IsDeduplication`），防止同一审批人在同一流程中重复出现：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/DeduplicationTypeEnum.cs
public enum DeduplicationTypeEnum
{
    DEDUPLICATION_TYPE_NULL = 1,        // 不去重
    DEDUPLICATION_TYPE_FORWARD = 2,     // 前去重：只在最后一次审批
    DEDUPLICATION_TYPE_BACKWARD = 3,    // 后去重：只在第一次审批
    DEDUPLICATION_TYPE_SKIP_NEXT = 4    // 跳过去重：相邻节点重复时自动同意
}
```

---

## 最佳实践

1. **明确审批人来源**：设计流程前先确定每个节点的审批人从哪来
2. **合理选择参数类型**：单人场景用 SINGLE 类型，多人场景用 MULTIPLAYER 类型
3. **配置去重策略**：审批人可能重复的流程务必配置去重
4. **设置缺失处理**：建议选择"转办给管理员"，避免流程卡死
5. **使用自定义规则**：复杂场景使用 UDR，保持标准规则简单清晰
