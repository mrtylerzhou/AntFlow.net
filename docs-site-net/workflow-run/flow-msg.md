# 消息通知机制

## 概述

AntFlowCore 提供完善的消息通知机制，在流程状态变更时自动通知相关人员。支持工作流流转通知、完成通知、拒绝通知、超时通知等多种通知类型。

## 消息通知类型

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/MsgNoticeTypeEnum.cs
public enum MsgNoticeTypeEnum
{
    PROCESS_FLOW = 1,               // 工作流流转通知
    RECEIVE_FLOW_PROCESS = 2,       // 收到转发工作流通知
    PROCESS_FINISH = 3,             // 工作流完成通知
    PROCESS_REJECT = 4,             // 工作流流程审批不通过通知
    PROCESS_TIME_OUT = 5,           // 工作流超时通知
    PROCESS_STOP = 6,               // 工作流被终止通知
    PROCESS_WAIR_VERIFY = 7,        // 工作流代审批通知
    PROCESS_CHANGE_ORIAL_TREATOR = 8,  // 工作流变更处理人通知(原审批节点处理人)
    PROCESS_CHANGE_NOW_TREATOR = 9,    // 工作流变更处理人通知(现审批节点处理人)
    PROCESS_SILENCE = 10            // 发送流程沉默消息通知
}
```

## 通知类型详解

### 1. 工作流流转通知（PROCESS_FLOW = 1）

当流程流转到新节点时，通知新节点的审批人处理。

**默认消息模板**：
```
您有1个{流程类型}{流程名称}{流程编号}需要处理
```

**触发时机**：
- 流程提交后，第一审批人收到通知
- 审批人同意后，下一审批人收到通知
- 条件分支确定后，对应分支审批人收到通知

### 2. 收到转发工作流通知（RECEIVE_FLOW_PROCESS = 2）

当流程被转发给某人时，通知被转发人。

**默认消息模板**：
```
您有1个{流程类型}{流程名称}{流程编号}需要查看
```

### 3. 工作流完成通知（PROCESS_FINISH = 3）

当流程全部审批通过时，通知发起人。

**默认消息模板**：
```
您的{流程类型}{流程名称}{流程编号}已完成
```

### 4. 工作流审批不通过通知（PROCESS_REJECT = 4）

当流程被拒绝时，通知发起人和所有已审批人。

**默认消息模板**：
```
您参与审批的{流程类型}{流程名称}{流程编号}已被{审批不同意者}驳回
```

### 5. 工作流超时通知（PROCESS_TIME_OUT = 5）

当审批任务超过处理期限时，通知当前审批人。

**默认消息模板**：
```
您有1个{流程类型}{流程名称}{流程编号}已超过处理期限，请立即处理
```

### 6. 工作流被终止通知（PROCESS_STOP = 6）

当流程被管理员终止时，通知所有相关人员。

**默认消息模板**：
```
您参与审批的{流程类型}{流程名称}{流程编号}已被{操作者}终止
```

### 7. 工作流代审批通知（PROCESS_WAIR_VERIFY = 7）

当审批被代审批时，通知原审批人。

**默认消息模板**：
```
您参与审批的{流程类型}{流程名称}{流程编号}已被{操作者}代审批
```

### 8. 变更处理人通知（PROCESS_CHANGE_ORIAL_TREATOR = 8）

当审批处理人变更时，通知原处理人。

**默认消息模板**：
```
您参与审批的{流程类型}{流程名称}{流程编号}已被变更为{变更后处理人}处理
```

### 9. 变更处理人通知（现处理人）（PROCESS_CHANGE_NOW_TREATOR = 9）

当审批处理人变更时，通知新处理人。

**默认消息模板**：
```
您有1个从{原审批节点处理人}转给您的{流程类型}{流程名称}{流程编号}需要处理
```

### 10. 流程沉默消息通知（PROCESS_SILENCE = 10）

当流程长时间无人处理时，通知管理员干预。

**默认消息模板**：
```
{流程类型}{流程名称}{流程编号}无人处理，请至流程管理中进行干预。
```

## 流程事件与通知映射

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/MsgProcessEventEnum.cs
public enum MsgProcessEventEnum
{
    NULL = 0,
    PROCESS_SUBMIT = 1,           // 流程提交
    PROCESS_RESUBMIT = 2,         // 重新提交
    PROCESS_APPROVE = 3,          // 同意
    PROCESS_NOT_APPROVE = 4,      // 不同意
    PROCESS_ABANDON = 7,          // 作废
    PROCESS_UNDERTAKE = 10,       // 承办
    PROCESS_CHANGE_DEALER = 11,   // 变更处理人
    PROCESS_ABORT = 12,           // 终止
    PROCESS_FORWARD = 15,         // 转发
    BUTTON_BACK_TO_MODIFY = 18,   // 打回修改
    PROCESS_JP = 19,              // 加批
    PROCESS_FINISH = 20,          // 流程完成
    HISTORY_SYNC = 100,           // 同步历史数据
    PROCESS_DATA_SYNC = 101       // 流程历史数据同步
}
```

### 事件与通知类型对照表

| 流程事件 | 通知类型 | 通知对象 |
|---------|---------|---------|
| PROCESS_SUBMIT | PROCESS_FLOW | 第一审批人 |
| PROCESS_APPROVE | PROCESS_FLOW | 下一审批人 |
| PROCESS_NOT_APPROVE | PROCESS_REJECT | 发起人、已审批人 |
| PROCESS_FINISH | PROCESS_FINISH | 发起人 |
| PROCESS_FORWARD | RECEIVE_FLOW_PROCESS | 被转发人 |
| PROCESS_CHANGE_DEALER | PROCESS_CHANGE_ORIAL_TREATOR | 原处理人 |
| PROCESS_CHANGE_DEALER | PROCESS_CHANGE_NOW_TREATOR | 新处理人 |
| PROCESS_ABORT | PROCESS_STOP | 所有相关人员 |
| PROCESS_ABANDON | - | - |
| BUTTON_BACK_TO_MODIFY | PROCESS_FLOW | 发起人 |
| PROCESS_JP | PROCESS_FLOW | 加批审批人 |

## 通知对象类型

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/InformEnum.cs
public enum InformEnum
{
    APPLICANT = 1,           // 申请人
    ALL_APPROVER = 2,        // 所有已审批人
    AT_APPROVER = 3,         // 当前节点审批人
    BY_TRANSPOND = 4,        // 被转发人
    ASSIGNED_USER = 5,       // 指定审批人
    ASSIGNEED_ROLES = 6      // 指定审批角色
}
```

| 通知对象 | 值 | 说明 |
|---------|---|------|
| 申请人 | 1 | 流程发起人 |
| 所有已审批人 | 2 | 流程中所有已完成审批的人员 |
| 当前节点审批人 | 3 | 当前正在审批的节点中的审批人 |
| 被转发人 | 4 | 被转发流程的人员 |
| 指定审批人 | 5 | 手动指定的审批人 |
| 指定审批角色 | 6 | 指定角色下的所有审批人 |

## 消息模板变量

通知消息支持以下模板变量：

| 变量名 | 说明 | 示例 |
|--------|------|------|
| {流程类型} | 流程类型名称 | 请假、报销 |
| {流程名称} | 流程实例名称 | 张三的请假申请 |
| {流程编号} | 流程唯一编号 | LEAVE20260726001 |
| {审批不同意者} | 拒绝审批的人员姓名 | 李四 |
| {操作者} | 执行操作的管理员姓名 | 王五 |
| {变更后处理人} | 变更后的处理人姓名 | 赵六 |
| {原审批节点处理人} | 原审批节点处理人姓名 | 孙七 |

## 通知渠道配置

流程模板支持配置通知渠道：

```json
{
  "noticeChannelTypes": [1, 2, 3]
}
```

| 渠道类型 | 说明 |
|---------|------|
| 1 | 站内消息 |
| 2 | 邮件通知 |
| 3 | 短信通知 |

## 通知发送流程

```
┌─────────────────────────────────────────────────────────────────┐
│                      通知发送流程                                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌───────────────┐                                              │
│  │ 1. 流程事件    │  审批操作触发事件                             │
│  │   触发        │  (同意/拒绝/转发等)                           │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 2. 确定通知    │  根据事件类型确定通知类型                       │
│  │   类型        │  MsgNoticeTypeEnum                            │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 3. 确定通知    │  根据通知类型确定通知对象                       │
│  │   对象        │  InformEnum                                   │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 4. 渲染消息    │  使用模板变量渲染消息内容                       │
│  │   内容        │  替换 {流程名称} 等变量                        │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 5. 发送通知    │  通过配置的渠道发送通知                         │
│  │               │  站内消息/邮件/短信                            │
│  └───────────────┘                                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## 超时通知机制

### 超时配置

审批任务可以配置超时时间，超时后自动发送通知：

```json
{
  "overtimeConf": {
    "enabled": true,
    "timeoutHours": 24,
    "remindType": 1
  }
}
```

### 超时处理策略

| 策略 | 说明 |
|------|------|
| 通知提醒 | 发送超时通知给审批人 |
| 自动转办 | 自动转给上级领导处理 |
| 自动跳过 | 自动跳过当前节点 |

## 沉默通知

当流程长时间无人处理时（如审批人离职、审批人为空），系统发送沉默通知给管理员：

```json
{
  "noticeType": 10,
  "message": "请假审批流程LEAVE20260726001无人处理，请至流程管理中进行干预。"
}
```

## 通知记录

所有通知发送记录都会被保存，便于追溯：

```json
{
  "id": "msg_001",
  "processNumber": "LEAVE20260726001",
  "noticeType": 1,
  "noticeTypeName": "工作流流转通知",
  "receiverId": "emp_001",
  "receiverName": "张三",
  "message": "您有1个请假审批流程LEAVE20260726001需要处理",
  "sendTime": "2026-07-26 10:00:00",
  "channelType": 1,
  "isRead": 0
}
```

## 通知订阅

用户可以订阅特定类型的通知：

```json
{
  "userId": "emp_001",
  "subscribeTypes": [1, 3, 4],
  "channels": [1, 2]
}
```

## 通知接口

### 获取通知列表

```http
POST /BpmnConf/process/listPage/{type}
Content-Type: application/json

{
  "taskMgmtVO": {
    "type": 1
  },
  "pageDto": { "pageNo": 1, "pageSize": 10 }
}
```

### 获取待办统计

```http
GET /BpmnConf/todoList
```

**源码实现**（`BpmnConfController.cs:173-177`）：

```csharp
[HttpGet("todoList")]
public Result<TaskMgmtVO> TodoList()
{
    TaskMgmtVO taskMgmtVO = _processApprovalService.ProcessStatistics();
    return ResultHelper.Success(taskMgmtVO);
}
```

## 通知时序图

```
审批人          引擎          通知服务        消息队列        接收人
  │             │             │             │             │
  │ 同意操作     │             │             │             │
  │────────────▶│             │             │             │
  │             │             │             │             │
  │             │ 触发事件     │             │             │
  │             │────────────▶│             │             │
  │             │             │             │             │
  │             │             │ 渲染消息     │             │
  │             │             │ 确定接收人   │             │
  │             │             │             │             │
  │             │             │ 发送通知     │             │
  │             │             │────────────▶│             │
  │             │             │             │             │
  │             │             │             │ 推送消息     │
  │             │             │             │────────────▶│
  │             │             │             │             │
```

## 自定义通知

AntFlowCore 支持通过扩展点自定义通知行为：

1. **自定义消息模板**：修改默认消息模板内容
2. **自定义通知渠道**：接入企业微信、钉钉等第三方通知
3. **自定义通知规则**：根据业务需求调整通知触发条件

## 最佳实践

1. **合理配置通知渠道**：重要流程使用多渠道通知，避免遗漏
2. **设置超时提醒**：为关键审批节点配置超时通知
3. **定期清理通知**：定期清理已读通知，保持通知列表整洁
4. **使用模板变量**：充分利用模板变量，使通知内容更有意义
5. **管理员关注沉默通知**：及时处理沉默通知，避免流程卡死
