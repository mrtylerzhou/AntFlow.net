# AntFlowCore 流程撤回、驳回和回退 实现差异详解

## 前言

很多开发者分不清流程撤回、驳回和回退这三个操作的区别，在实际使用中经常混淆。本文详细介绍这三个操作的概念区别、适用场景和实现原理，帮你理清设计思路。

## 概念区分

先明确三个操作的定义：

| 操作 | 谁操作 | 什么时候用 | 最终效果 |
|------|--------|-----------|---------|
| **撤回** | 发起人 | 流程发起后，还没完成，发起人不想继续了，主动撤回整个流程 | 流程直接结束，状态变为已撤回 |
| **驳回** | 审批人 | 审批人不同意当前申请，打回给发起人 | 流程回到发起人，发起人修改后可以重新发起 |
| **回退** | 审批人 | 不同意当前审批，想要退回到前面某个节点重新审批 | 流程退回到指定的前面节点，从那里重新审批 |

我们一个一个来看。

## 一、撤回：发起人主动结束流程

### 适用场景

- 发起人填错了表单，不想继续了
- 需求取消了，不需要审批了
- 发起的时候选错了流程，撤回重新发起

### 核心要求

- 只能由发起人撤回
- 只有流程还在运行中（没有结束、没有驳回）才能撤回
- 撤回后流程状态变为**已撤回**，不再继续流转
- 需要记录撤回原因

### 实现源码解析

我们看撤回操作实现 `UndertakeProcessService`:

```csharp
public class UndertakeProcessService : IProcessOperationAdaptor
{
    public async Task<CustomButtonResult> ExecuteAsync(
        AFTask task, 
        AFProcessInstance instance, 
        Dictionary<string, object> param)
    {
        // 1. 校验：只有发起人才能撤回
        var currentUserId = SecurityUtils.GetLogInEmpIdStr();
        if (instance.StartUserId != currentUserId)
        {
            return CustomButtonResult.Fail("只有发起人才能撤回");
        }

        // 2. 校验：流程必须还在运行中
        if (instance.Status != ProcessStatus.Running)
        {
            return CustomButtonResult.Fail("流程已经结束，不能撤回");
        }

        // 3. 获取撤回原因
        var reason = param.TryGetValue("reason", out var r) ? r?.ToString() : "";

        // 4. 更新流程状态为已撤回
        instance.Status = ProcessStatus.Withdrawn;
        instance.WithdrawReason = reason;
        instance.WithdrawTime = DateTime.Now;
        await _processInstanceService.UpdateAsync(instance);

        // 5. 取消所有未完成的任务
        await _taskService.CancelAllTasks(instance.Id);

        // 6. 发布流程撤回事件
        await _eventBus.Publish(new ProcessWithdrawnEvent(instance));

        // 流程结束，不需要继续推进
        return CustomButtonResult.Success(needContinue: false);
    }
}
```

### 状态流转图

```
发起流程 → 运行中 → [发起人撤回] → 已撤回 → 结束
```

### 数据库变化

- `af_process_instance.Status` 更新为 `Withdrawn` (已撤回)
- 所有该流程下的未完成任务更新为 `Canceled` 状态

## 二、驳回：审批人打回给发起人

### 适用场景

- 审批人认为申请材料不全，要求发起人补充材料
- 审批人不同意申请，直接打回给发起人
- 发起人修改后可以重新提交审批

### 核心要求

- 由当前审批人操作
- 驳回后流程回到发起人，发起人可以修改后重新提交
- 原来的审批记录保留，方便追溯

### 实现源码解析

我们看驳回实现：

```csharp
public class BackToModifyService : IProcessOperationAdaptor
{
    public async Task<CustomButtonResult> ExecuteAsync(
        AFTask task, 
        AFProcessInstance instance, 
        Dictionary<string, object> param)
    {
        // 1. 获取驳回原因
        var reason = param.TryGetValue("reason", out var r) ? r?.ToString() : "";

        // 2. 完成当前任务，标记为驳回
        await _taskService.CompleteTask(task.Id, false, reason);

        // 3. 更新流程状态为驳回中
        instance.Status = ProcessStatus.Rejected;
        instance.RejectReason = reason;
        instance.RejectTime = DateTime.Now;
        await _processInstanceService.UpdateAsync(instance);

        // 4. 发布驳回事件，业务可以监听做通知
        await _eventBus.Publish(new ProcessRejectedEvent(instance, task));

        // 流程结束当前节点，等待发起人重新提交
        return CustomButtonResult.Success(needContinue: false);
    }
}
```

### 状态流转图

```
发起流程 → ... → 当前审批人审批 → [驳回] → 发起人修改 → 重新提交 → 重新开始审批
```

### 和撤回的区别

| 对比点 | 撤回 | 驳回 |
|--------|------|------|
| 操作人 | 发起人 | 审批人 |
| 结果 | 流程直接结束 | 流程回到发起人，可以重新提交 |
| 状态 | 已撤回 | 已驳回 |
| 能否重新提交 | 不能，要重新发起新流程 | 可以，修改后直接重新提交 |

## 三、回退：退回到前面某一个节点重新审批

### 适用场景

- 当前审批人认为应该由前面某个节点先重新审批
- 审批错了，想要退回去重审
- 会签中发现前面环节遗漏了审批，需要退回去加上

**回退和驳回最大的区别：**
- 驳回 → 退回到**发起人**，结束当前审批流，等待发起人修改
- 回退 → 退回到**前面任意一个节点**，继续从那个节点重新审批

### 核心要求

- 由当前审批人操作
- 可以指定退回到哪一个节点
- 原来的审批记录保留
- 从目标节点开始重新往后审批

### 实现源码解析

核心逻辑在 `ProcessNodeJumpService`:

```csharp
public async Task<CustomButtonResult> BackToNodeAsync(
    AFTask currentTask, 
    AFProcessInstance instance, 
    string targetNodeId,
    string reason)
{
    // 1. 完成当前任务
    await _taskService.CompleteTask(currentTask.Id, false, reason);

    // 2. 清除从目标节点到当前节点之间所有未完成任务
    var tasksToCancel = _taskService.FindTasksBetween(
        instance.Id, 
        targetNodeId, 
        currentTask.NodeId);
    
    foreach (var task in tasksToCancel)
    {
        await _taskService.CancelTask(task.Id);
    }

    // 3. 更新流程变量，记录当前位置退回到目标节点
    instance.CurrentNodeId = targetNodeId;
    await _processInstanceService.UpdateAsync(instance);

    // 4. 从目标节点重新开始推进流程
    await _processFlowService.ProcessToNextNode(instance, targetNodeId);

    // 5. 发布回退事件
    await _eventBus.Publish(new ProcessRollbackEvent(instance, currentTask.NodeId, targetNodeId));

    return CustomButtonResult.Success(needContinue: false);
}
```

### 状态流转图

```
节点A → 节点B → 节点C → [当前审批人在C回退到A] → 节点A → 重新A→B→C
```

### 回退的几种用法

| 用法 | 说明 |
|------|------|
| **回退到发起人** | 效果和驳回类似，但是流程还是继续走，只是重新来一遍 |
| **回退到上一级审批** | 退回给上级重新审批 |
| **回退到任意节点** | 跳过中间环节，直接回到指定节点 |

## 三者对比总结

让我们用表格对比一下：

| 对比项 | 撤回 (Withdraw) | 驳回 (Reject) | 回退 (Rollback) |
|--------|----------------|---------------|-----------------|
| **操作人** | 发起人 | 当前审批人 | 当前审批人 |
| **操作时机** | 流程发起后，任意运行阶段 | 当前节点审批时 | 当前节点审批时 |
| **目标** | 整个流程不要了，直接结束 | 不同意，打回给发起人修改 | 退回到前面节点重新审批 |
| **终点** | 流程结束，状态变为已撤回 | 流程暂停，等待发起人修改重提 | 流程继续，从目标节点重新走 |
| **能否重新发起** | 不能，要发起新流程 | 可以，修改后重提 | 不需要重新发起，继续走 |
| **指定目标节点** | - | 固定回到发起人 | 可以指定任意前面节点 |

## 数据库状态说明

AntFlowCore 定义了这些流程状态：

```csharp
public enum ProcessStatus
{
    Running = 1,       // 运行中
    Ended = 2,         // 正常结束
    Rejected = 3,      // 驳回（等待发起人重提）
    Withdrawn = 4,     // 发起人撤回
    Suspended = 5      // 挂起
}
```

状态对应：

| 操作 | 最终状态 |
|------|---------|
| 撤回 | `Withdrawn` |
| 驳回 | `Rejected` |
| 回退 | 还是 `Running` |

## 使用场景建议

### 什么时候用撤回？

- 发起人自己填错了
- 需求取消了，不需要继续了
- 发起错了流程

### 什么时候用驳回？

- 审批人不同意，需要发起人修改后重新提交
- 材料不全，要求补充材料

### 什么时候用回退？

- 当前审批人发现前面环节有问题，需要回去重审
- 流程走错了分支，退回去重新走
- 需要增加前面环节审批

## 常见问题

### 1. 驳回后发起人重新提交，是新建流程还是继续原流程？

AntFlowCore 设计是继续原流程，驳回后发起人修改重新提交，原流程继续走，这样保留完整的审批记录。如果你想要全新流程，发起人撤回重新发起就行。

### 2. 回退可以跨多个节点吗？

可以，回退可以指定任意前面的节点，不管隔了多少个节点。

### 3. 回退后审批记录会清除吗？

不会，原来的审批记录都保留，方便追溯，只是取消未完成的任务。

### 4. 撤回后还能恢复吗？

不能，撤回就是彻底结束了，要重新发起新流程。如果想要可恢复，用挂起。

### 5. 驳回后可以不重新提交直接作废吗？

可以，发起人可以选择直接撤回，流程就结束了。

## 源码中自定义按钮注册

这三个操作都是作为自定义按钮实现的，我们看注册：

```csharp
// 撤回
services.AddSingleton<IProcessOperationAdaptor, UndertakeProcessService>();
// 驳回重填
services.AddSingleton<IProcessOperationAdaptor, BackToModifyService>();
// 回退到指定节点
services.AddSingleton<IProcessOperationAdaptor, BackToNodeService>();
```

在流程设计器中，给需要的节点加上这些按钮就行，不需要写代码。

## 总结

AntFlowCore 中这三个操作设计区分非常清晰：

- **撤回** → 发起人主动结束流程 → 流程终止
- **驳回** → 审批人打回给发起人修改 → 流程暂停等待重提
- **回退** → 退回到前面节点重新审批 → 流程继续从目标节点走

根据你的场景选择正确的操作，就能满足各种审批需求。

---

**相关链接：**
- [AntFlowCore 自定义按钮开发实战](./AntFlowCore-自定义按钮开发实战.md)
- [AntFlowCore 事件系统详解](./AntFlowCore-事件系统详解-监听流程状态变化.md)
- [上一篇：会签节点实现原理详解](./AntFlowCore-会签节点实现原理详解.md)
