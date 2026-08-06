# 审批操作详解

## 概述

AntFlowCore 提供丰富的审批操作，覆盖同意、拒绝、加批、转办、退回等常见审批场景。所有审批操作通过统一的 `ButtonsOperation` 接口处理，引擎根据操作类型分发到对应的策略执行器。

用户在待办任务列表中选择任务后，进入审批页面可以看到所有可用的审批按钮：

![待办任务列表](/images/todo-tasks.png)

## 审批操作总览

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/ProcessOperationEnum.cs
public enum ProcessOperationEnum
{
    BUTTON_TYPE_SUBMIT = 1,                // 流程提交
    BUTTON_TYPE_RESUBMIT = 2,              // 重新提交
    BUTTON_TYPE_AGREE = 3,                 // 同意
    BUTTON_TYPE_DIS_AGREE = 4,             // 不同意
    BUTTON_TYPE_ABANDON = 7,               // 作废
    BUTTON_TYPE_UNDERTAKE = 10,            // 承办
    BUTTON_TYPE_CHANGE_ASSIGNEE = 11,      // 变更处理人
    BUTTON_TYPE_STOP = 12,                 // 终止
    BUTTON_TYPE_FORWARD = 15,              // 转发
    BUTTON_TYPE_BACK_TO_MODIFY = 18,       // 打回修改
    BUTTON_TYPE_JP = 19,                   // 加批
    BUTTON_TYPE_ZB = 21,                   // 转办
    BUTTON_TYPE_BACK_TO_ANY_NODE = 23,     // 退回任意节点
    BUTTON_TYPE_REMOVE_ASSIGNEE = 24,      // 减签
    BUTTON_TYPE_ADD_ASSIGNEE = 25,         // 加签
    BUTTON_TYPE_PROCESS_DRAW_BACK = 29,    // 流程撤回
    BUTTON_TYPE_DRAW_BACK_AGREE = 32,      // 撤销同意
    BUTTON_TYPE_PROCESS_MOVE_AHEAD = 33,   // 流程推进
}
```

## 审批操作接口

所有审批操作统一使用 `ButtonsOperation` 接口：

```http
POST /BpmnConf/process/buttonsOperation?formCode={formCode}
Content-Type: application/json

{
  "operationType": 3,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "approvalComment": "同意"
}
```

**源码实现**（`BpmnConfController.cs:60-67`）：

```csharp
/// <summary>
/// 流程操作核心方法,流程同意,不同意,拒绝,加拒,变更处理人等出口都在此方法,
/// 通过策略模板实现处理逻辑分发
/// </summary>
[HttpPost("process/buttonsOperation")]
public Result<BusinessDataVo> ButtonsOperation(
    [FromServices] IHttpContextAccessor accessor, 
    [FromQuery] String formCode)
{
    string values = accessor.HttpContext!.ReadRawBodyAsString();
    BusinessDataVo dataVo = _processApprovalService.ButtonsOperation(values, formCode);
    return Result<BusinessDataVo>.Succ(dataVo);
}
```

## 同意（BUTTON_TYPE_AGREE = 3）

审批人同意当前节点的审批，流程继续向后流转。

```json
{
  "operationType": 3,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "approvalComment": "同意，请继续",
  "formCode": "leave_form"
}
```

### 同意处理流程

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  同意操作    │ -> │  完成当前    │ -> │  检查是否    │ -> │  创建下一    │
│  Agree      │    │  任务        │    │  全部完成    │    │  任务        │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                                            │
                                            ▼
                                      ┌─────────────┐
                                      │  是：继续    │
                                      │  下一节点    │
                                      └─────────────┘
```

### 会签/或签处理

- **会签**：需所有审批人同意才继续，一人同意只完成自己的任务
- **或签**：一人同意即继续，其他审批人的任务自动取消
- **顺序会签**：按顺序审批，全部同意后继续

## 不同意（BUTTON_TYPE_DIS_AGREE = 4）

审批人不同意当前节点的审批，根据不同意类型决定流程走向。

```json
{
  "operationType": 4,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "approvalComment": "不同意，材料不齐全",
  "flag": false,
  "backToNodeId": "start"
}
```

### 不同意类型

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/ProcessDisagreeTypeEnum.cs
public class ProcessDisagreeTypeEnum
{
    public static readonly ProcessDisagreeTypeEnum ONE_DISAGREE = new(1, "退回上一个节点提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum TWO_DISAGREE = new(2, "退回发起人提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum THREE_DISAGREE = new(3, "退回发起人提交回退节点");
    public static readonly ProcessDisagreeTypeEnum FOUR_DISAGREE = new(4, "退回历史节点提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum FIVE_DISAGREE = new(5, "退回历史节点提交回退节点");
}
```

| 类型 | 值 | 说明 |
|------|---|------|
| 退回上一节点提交下一节点 | 1 | 退回到上一个审批节点，修改后提交到下一个节点 |
| 退回发起人提交下一节点 | 2 | 退回到发起人，修改后提交到下一个节点 |
| 退回发起人提交回退节点 | 3 | 退回到发起人，修改后重新提交到回退的节点 |
| 退回历史节点提交下一节点 | 4 | 退回到历史任一节点，修改后提交到下一个节点 |
| 退回历史节点提交回退节点 | 5 | 退回到历史任一节点，修改后重新提交到回退的节点 |

## 打回修改（BUTTON_TYPE_BACK_TO_MODIFY = 18）

审批人将流程打回给发起人修改，修改后重新提交。

```json
{
  "operationType": 18,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "approvalComment": "请补充请假证明材料",
  "backToModifyType": 1,
  "backToNodeId": "start"
}
```

## 加批（BUTTON_TYPE_JP = 19）

在当前节点后临时添加一个额外的审批节点，加批完成后继续原流程。

```json
{
  "operationType": 19,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "approvalComment": "需要财务部门补充审批",
  "userIds": ["emp_010"],
  "nodeProperty": 5,
  "signType": 1
}
```

### 加批 vs 加签

| 操作 | 说明 | 影响范围 |
|------|------|---------|
| 加批 | 在当前节点后创建新的审批节点 | 原流程暂停，新节点完成后继续 |
| 加签 | 在当前节点增加额外的审批人 | 不影响原流程节点结构 |

## 转办（BUTTON_TYPE_ZB = 21）

将当前审批任务转给其他人处理。

```json
{
  "operationType": 21,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "userIds": ["emp_020"],
  "approvalComment": "转给王五处理"
}
```

## 退回任意节点（BUTTON_TYPE_BACK_TO_ANY_NODE = 23）

将流程退回到任意历史节点重新审批。

```json
{
  "operationType": 23,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "backToNodeId": "node_002",
  "backToEmployeeId": "emp_015",
  "approvalComment": "请重新评估金额"
}
```

## 加签（BUTTON_TYPE_ADD_ASSIGNEE = 25）

在当前节点增加额外的审批人（不创建新节点）。

```json
{
  "operationType": 25,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "userIds": ["emp_030"],
  "approvalComment": "请赵六会签"
}
```

## 减签（BUTTON_TYPE_REMOVE_ASSIGNEE = 24）

从当前节点的审批人中移除部分人员。

```json
{
  "operationType": 24,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "userIds": ["emp_030"],
  "approvalComment": "不需要赵六会签了"
}
```

## 变更处理人（BUTTON_TYPE_CHANGE_ASSIGNEE = 11）

变更当前节点的审批处理人。

```json
{
  "operationType": 11,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "userIds": ["emp_040"],
  "approvalComment": "变更给孙七处理"
}
```

## 转发（BUTTON_TYPE_FORWARD = 15）

将流程转发给其他人查看或处理。

```json
{
  "operationType": 15,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "userIds": ["emp_050"],
  "approvalComment": "请周八知悉"
}
```

## 撤回（BUTTON_TYPE_PROCESS_DRAW_BACK = 29）

发起人在流程未完成前撤回流程。

```json
{
  "operationType": 29,
  "processNumber": "LEAVE20260726001",
  "approvalComment": "撤回申请"
}
```

## 撤销同意（BUTTON_TYPE_DRAW_BACK_AGREE = 32）

审批人在同意后反悔，撤销同意重新审批。

```json
{
  "operationType": 32,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "approvalComment": "撤销同意，重新审核"
}
```

## 终止（BUTTON_TYPE_STOP = 12）

管理员强制终止流程。

```json
{
  "operationType": 12,
  "processNumber": "LEAVE20260726001",
  "approvalComment": "流程终止"
}
```

## 作废（BUTTON_TYPE_ABANDON = 7）

将已完成的流程标记为作废。

```json
{
  "operationType": 7,
  "processNumber": "LEAVE20260726001",
  "approvalComment": "流程作废"
}
```

## 流程推进（BUTTON_TYPE_PROCESS_MOVE_AHEAD = 33）

管理员手动推进流程进度，跳过卡住的节点。

```json
{
  "operationType": 33,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "approvalComment": "流程推进"
}
```

## 承办（BUTTON_TYPE_UNDERTAKE = 10）

审批人将当前任务委托给承办人处理。

```json
{
  "operationType": 10,
  "processNumber": "LEAVE20260726001",
  "taskId": "task_001",
  "userIds": ["emp_060"],
  "approvalComment": "由钱九承办"
}
```

## 节点操作类型配置

每个节点可以配置允许的操作类型：

```json
{
  "nodeId": "approve_001",
  "operationTypes": [3, 4, 18, 19, 21, 25]
}
```

## 审批按钮类型

前端页面根据权限显示不同的按钮组：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/ProcessButtonEnum.cs
public class ProcessButtonEnum
{
    public static readonly ProcessButtonEnum DISAGREE_TYPE = new(1, "不同意");
    public static readonly ProcessButtonEnum AGREE_TYPE = new(2, "同意");
    public static readonly ProcessButtonEnum WITHDRAW_TYPE = new(3, "撤回");
    public static readonly ProcessButtonEnum END_TYPE = new(4, "终止");
    public static readonly ProcessButtonEnum DELETE_TYPE = new(5, "作废");
    public static readonly ProcessButtonEnum CHANGE_TYPE = new(6, "变更处理人");
    public static readonly ProcessButtonEnum HANDLE_TYPE = new(7, "代审批");
    public static readonly ProcessButtonEnum PRINTRING_TYPE = new(8, "打印");
    public static readonly ProcessButtonEnum FORWARD_TYPE = new(10, "转发");
    public static readonly ProcessButtonEnum UNDERTAKE_TYPE = new(12, "承办");
    public static readonly ProcessButtonEnum JOINTLY_SIGN = new(14, "会签");
    public static readonly ProcessButtonEnum GET_BACK = new(15, "返回");
    public static readonly ProcessButtonEnum ADD_BATCH = new(16, "增加审批人");
    public static readonly ProcessButtonEnum STAFF_CONFIRM_TYPE = new(17, "代员工确认");
}
```

## 审批路径记录

每次审批操作都会记录到审批路径中，通过 `BpmVerifyInfoVo` 查询：

```http
GET /BpmnConf/getBpmVerifyInfoVos?processNumber={processNumber}
```

```csharp
// 源码位置: AntFlowCore.Base/vo/BpmVerifyInfoVo.cs
public class BpmVerifyInfoVo
{
    public string Id { get; set; }
    public string RunInfoId { get; set; }
    public string VerifyUserId { get; set; }       // 审批人ID
    public string VerifyUserName { get; set; }     // 审批人姓名
    public int VerifyStatus { get; set; }          // 审批状态
    public string VerifyDesc { get; set; }         // 审批描述
    public DateTime? VerifyDate { get; set; }      // 审批时间
    public string TaskName { get; set; }           // 任务名称
    public string ElementId { get; set; }          // 元素ID
    public int? Sort { get; set; }                 // 排序
    public int? NodeType { get; set; }             // 节点类型
}
```

## 审批操作流程图

```
┌─────────────────────────────────────────────────────────────────┐
│                      审批操作流程                                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌───────────────┐                                              │
│  │ 1. 接收请求    │  解析 operationType                          │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 2. 路由分发    │  根据 operationType 选择处理策略               │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────────────────────────────────────┐              │
│  │ 3. 策略执行                                    │              │
│  │                                               │              │
│  │  ┌─────────┐ ┌─────────┐ ┌─────────┐         │              │
│  │  │ 同意策略 │ │ 拒绝策略 │ │ 退回策略 │ ...    │              │
│  │  └────┬────┘ └────┬────┘ └────┬────┘         │              │
│  └───────┼───────────┼───────────┼───────────────┘              │
│          ▼           ▼           ▼                               │
│  ┌───────────────────────────────────────────────┐              │
│  │ 4. 更新状态                                    │              │
│  │  - 更新任务状态                                │              │
│  │  - 记录审批路径                                │              │
│  │  - 创建新任务                                  │              │
│  └───────────────────────┬───────────────────────┘              │
│                          ▼                                      │
│  ┌───────────────────────────────────────────────┐              │
│  │ 5. 通知下一节点审批人                           │              │
│  └───────────────────────────────────────────────┘              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## 最佳实践

1. **明确审批意见**：每次审批都填写清晰的审批意见
2. **及时审批**：收到待办后及时处理，避免影响流程进度
3. **加批谨慎**：加批会增加流程节点，仅在必要时使用
4. **退回说明**：退回时说明原因和修改要求
5. **转办确认**：转办前与被转办人沟通确认
