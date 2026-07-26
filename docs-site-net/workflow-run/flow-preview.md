# 流程预览功能

## 概述

流程预览是 AntFlowCore 的核心功能之一，允许用户在实际发起流程前查看完整的审批路径。管理员在设计流程时也可以使用预览功能验证流程配置的正确性。

## 预览类型

AntFlowCore 提供三种预览类型：

1. **流程预览（preview）**：管理员在设计流程时预览完整流程
2. **发起页预览（startPagePreviewNode）**：发起人在提交前预览审批路径
3. **任务页预览（taskPagePreviewNode）**：审批人在处理任务时查看流程图

## 流程预览接口

### 1. 管理员流程预览

管理员在设计流程时使用，返回完整的流程节点信息：

```http
POST /BpmnConf/preview
Content-Type: application/json

{
  "bpmnConfVo": {
    "bpmnName": "请假审批流程",
    "formCode": "leave_form",
    "nodes": [...]
  },
  "formData": {
    "amount": 5000,
    "days": 3
  }
}
```

**源码实现**（`BpmnConfController.cs:87-93`）：

```csharp
[HttpPost("preview")]
public Result<PreviewNode> Preview([FromServices] IHttpContextAccessor accessor)
{
    string values = accessor.HttpContext!.ReadRawBodyAsString();
    PreviewNode previewNode = _bpmnConfCommonService.PreviewNode(values);
    return Result<PreviewNode>.Succ(previewNode);
}
```

### 2. 发起页预览

发起人在填写表单后、提交前查看审批路径：

```http
POST /BpmnConf/startPagePreviewNode
Content-Type: application/json

{
  "isStartPreview": true,
  "formCode": "leave_form",
  "formData": {
    "amount": 5000,
    "days": 3
  }
}
```

**源码实现**（`BpmnConfController.cs:100-113`）：

```csharp
[HttpPost("startPagePreviewNode")]
public Result<PreviewNode> StartPagePreviewNode([FromServices] IHttpContextAccessor accessor)
{
    string paramsJson = accessor.HttpContext!.ReadRawBodyAsString();
    JsonNode? jsonObject = JsonNodeHelper.SafeParse(paramsJson);
    bool isStartPreview = jsonObject?["isStartPreview"]?.GetValue<bool>() ?? false;

    if (isStartPreview)
    {
        return Result<PreviewNode>.Succ(_bpmnConfCommonService.StartPagePreviewNode(paramsJson));
    }
    return Result<PreviewNode>.Succ(_bpmnConfCommonService.TaskPagePreviewNode(paramsJson));
}
```

## PreviewNode 数据结构

```csharp
// 源码位置: AntFlowCore.Base/vo/PreviewNode.cs
public class PreviewNode
{
    public string BpmnName { get; set; }                    // 流程名称
    public string FormCode { get; set; }                    // 表单编码
    public List<BpmnNodeVo> BpmnNodeList { get; set; }      // 节点列表
    public PrevEmployeeInfo StartUserInfo { get; set; }     // 发起人信息
    public PrevEmployeeInfo EmployeeInfo { get; set; }      // 员工信息
    public int? DeduplicationType { get; set; }             // 去重类型
    public string DeduplicationTypeName { get; set; }       // 去重类型名称
    public String CurrentNodeId { get; set; }               // 当前节点ID
    public List<String> BeforeNodeIds { get; set; }         // 前置节点ID列表
    public List<String> AfterNodeIds { get; set; }          // 后置节点ID列表
}
```

## 预览数据结构详解

### 节点列表（BpmnNodeList）

```json
{
  "bpmnNodeList": [
    {
      "nodeId": "start",
      "nodeType": 1,
      "nodeName": "发起人",
      "nodeProperty": null,
      "property": null
    },
    {
      "nodeId": "condition_001",
      "nodeType": 3,
      "nodeName": "金额判断",
      "nodeProperty": null,
      "property": {
        "conditionsConf": {
          "isDefault": 0,
          "sort": 1,
          "groupRelation": 1
        }
      }
    },
    {
      "nodeId": "approve_001",
      "nodeType": 4,
      "nodeName": "部门经理审批",
      "nodeProperty": 13,
      "nodePropertyName": "直属领导",
      "property": {
        "emplIds": ["emp_manager_001"],
        "emplList": [{"id": "emp_manager_001", "name": "张经理"}],
        "signType": 1
      }
    },
    {
      "nodeId": "approve_002",
      "nodeType": 4,
      "nodeName": "总经理审批",
      "nodeProperty": 5,
      "property": {
        "emplIds": ["emp_ceo_001"],
        "emplList": [{"id": "emp_ceo_001", "name": "李总"}],
        "signType": 2
      }
    }
  ]
}
```

### 前后置节点信息

```json
{
  "currentNodeId": "approve_001",
  "beforeNodeIds": ["start", "condition_001"],
  "afterNodeIds": ["approve_002", "end"]
}
```

## 预览流程图

预览功能生成的流程图数据，前端可渲染为可视化流程图：

```
┌─────────────────────────────────────────────────────────────────┐
│                    请假审批流程                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────┐     ┌──────────┐     ┌──────────┐               │
│  │  发起人   │────▶│ 金额判断  │────▶│ 部门经理  │               │
│  │  (已完成) │     │ (条件)   │     │ (待处理) │               │
│  └──────────┘     └────┬─────┘     └────┬─────┘               │
│                        │                │                       │
│                   ┌────┴─────┐          │                       │
│                   │ 总经理    │◀─────────┘                       │
│                   │ (后续)   │                                  │
│                   └────┬─────┘                                  │
│                        │                                        │
│                   ┌────┴─────┐                                  │
│                   │   结束   │                                  │
│                   └──────────┘                                  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## 预览与实际运行的区别

| 特性 | 预览 | 实际运行 |
|------|------|---------|
| 创建流程实例 | 否 | 是 |
| 生成审批任务 | 否 | 是 |
| 发送通知 | 否 | 是 |
| 条件评估 | 是 | 是 |
| 审批人去重 | 是（模拟） | 是（实际） |
| 审批人解析 | 是（模拟） | 是（实际） |

## 去重类型预览

预览功能会根据去重配置模拟去重效果：

```json
{
  "deduplicationType": 2,
  "deduplicationTypeName": "当一个审批人重复出现时，只在最后一次审批（前去重）"
}
```

### 去重策略预览效果

| 去重类型 | 预览效果 |
|---------|---------|
| 不去重 | 显示所有审批人 |
| 前去重 | 重复审批人只显示在最后一个节点 |
| 后去重 | 重复审批人只显示在第一个节点 |
| 跳过去重 | 相邻节点重复的审批人自动跳过 |

## 节点标签显示

预览时会显示特殊节点的标签信息：

```json
{
  "bpmnNodeList": [
    {
      "nodeId": "copy_001",
      "nodeType": 4,
      "nodeName": "抄送HR",
      "labelList": [
        {
          "labelValue": "copy_node_v2",
          "labelName": "抄送节点V2"
        }
      ]
    }
  ]
}
```

## 预览中的审批标准

预览结果中包含审批标准的展示：

```json
{
  "nodeId": "approve_001",
  "approvalStandard": 1,
  "nodePropertyName": "直属领导",
  "property": {
    "approvalStandardName": "发起人"
  }
}
```

## 条件预览

条件节点在预览时会显示所有可能的路径：

```json
{
  "nodeId": "condition_amount",
  "nodeType": 3,
  "nodeName": "金额判断",
  "nodeTo": ["approve_manager", "approve_ceo"],
  "property": {
    "conditionList": [
      [
        {
          "columnId": "38",
          "columnName": "总金额",
          "operator": 1,
          "operatorName": ">=",
          "value": "10000"
        }
      ]
    ],
    "isDefault": 0
  }
}
```

## 动态条件预览

动态条件节点在预览时根据表单数据实时评估：

```json
{
  "isDynamicCondition": true,
  "labelList": [
    {
      "labelValue": "dynamic_condition_node",
      "labelName": "动态条件节点"
    }
  ]
}
```

## 发起页预览流程

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  填写表单    │ -> │  点击预览    │ -> │  引擎计算    │ -> │  展示路径    │
│  FormData   │    │  Preview    │    │  Evaluate   │    │  Show Path  │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                                                               │
                                                               ▼
                                                         ┌─────────────┐
                                                         │  确认发起    │
                                                         │  Submit     │
                                                         └─────────────┘
```

## 审批路径记录

已发起的流程可以通过审批路径接口查看实际执行记录：

```http
GET /BpmnConf/getBpmVerifyInfoVos?processNumber={processNumber}
```

### 审批路径数据结构

```json
{
  "id": "verify_001",
  "runInfoId": "run_001",
  "verifyUserId": "emp_001",
  "verifyUserName": "张三",
  "verifyStatus": 2,
  "verifyStatusName": "已通过",
  "verifyDesc": "同意",
  "verifyDate": "2026-07-26 10:30:00",
  "taskName": "部门经理审批",
  "elementId": "approve_001",
  "nodeType": 4,
  "sort": 2
}
```

## 流程预览时序图

```
发起人          前端           API           引擎          数据库
  │             │             │             │             │
  │ 填写表单     │             │             │             │
  │────────────▶│             │             │             │
  │             │             │             │             │
  │ 点击预览     │             │             │             │
  │────────────▶│ startPage   │             │             │
  │             │ PreviewNode │             │             │
  │             │────────────▶│ 加载流程模板 │             │
  │             │             │────────────▶│ 查询配置    │
  │             │             │             │────────────▶│
  │             │             │             │◀────────────│
  │             │             │◀────────────│ 返回配置    │
  │             │             │ 评估条件     │             │
  │             │             │ 解析审批人   │             │
  │             │             │ 应用去重     │             │
  │             │◀────────────│ 返回PreviewNode            │
  │◀────────────│ 渲染流程图   │             │             │
  │             │             │             │             │
```

## 预览模式配置

管理员可以在流程配置中控制预览行为：

```json
{
  "bpmnConfVo": {
    "isAll": 1,
    "effectiveStatus": 1,
    "noticeChannelTypes": [1, 2]
  }
}
```

## 最佳实践

1. **发起前预览**：养成发起前先预览的习惯，及时发现流程配置问题
2. **管理员验证**：新流程发布前通过预览功能充分验证
3. **模拟多场景**：使用不同的表单数据多次预览，验证条件分支正确性
4. **关注去重效果**：预览时注意去重策略是否符合预期
5. **检查审批人**：确认预览中的审批人列表正确无误
