# AntFlowCore 会签节点实现原理详解

## 前言

会签是流程审批中非常常见的场景，就是多个审批人共同审批一个任务，需要根据投票结果决定流程是否通过。AntFlowCore 原生支持多种会签类型：并签（无序会签）、顺序会签、或签。本文深入解析 AntFlowCore 会签的实现原理和使用方法。

## 什么是会签？不同会签类型区别

首先我们理清概念：

| 类型 | 说明 | 通过规则 | 适用场景 |
|------|------|----------|---------|
| **单人节点** | 一个人审批 | - | 普通审批 |
| **或签（一票通过）** | 多个人审批，只要有一个人通过，节点就通过 | 任意一个同意 → 通过 | 多个候选人，谁有空谁批，只要有人批了就过 |
| **并签（全票通过）** | 多个人审批，必须全部同意才通过 | 全部同意 → 通过 | 重要事项需要多人确认 |
| **百分比通过** | 指定百分比同意就通过 | 同意票数 / 总票数 ≥ 设定比例 → 通过 | 董事会投票 |
| **顺序会签** | 按顺序一个一个审批，必须都通过 | 全部按顺序审批完成都同意 → 通过 | 公文流转，需要按层级依次审批 |
| **并行会签** | 多个审批人可以同时审批，不分顺序 | 根据投票规则决定 | 多个部门同时会审 |

AntFlowCore 原生支持以上所有类型。

## 数据库设计

AntFlowCore 会签相关表设计：

### 1. `BpmVariableMultiplayer` - 会签实例表

```csharp
public class BpmVariableMultiplayer
{
    public long Id { get; set; }
    public long VariableId { get; set; }      // 关联流程变量
    public long ElementId { get; set; }      // 节点ID
    public string ElementName { get; set; }  // 节点名称
    public long NodeId { get; set; }         // BPMN节点ID
    public int SignType { get; set; }        // 会签类型：1=会签（全通过） 2=或签（一票过）
    public DateTime CreateTime { get; set; }
    public long? TenantId { get; set; }
}
```

### 2. `BpmVariableMultiplayerPersonnel` - 会签人员表

```csharp
public class BpmVariableMultiplayerPersonnel
{
    public long Id { get; set; }
    public long VariableMultiplayerId { get; set; }  // 关联会签实例
    public string Assignee { get; set; }              // 审批人ID
    public string AssigneeName { get; set; }          // 审批人姓名
    public int UndertakeStatus { get; set; }         // 处理状态：0=未处理 1=已处理 2=已通过 3=已驳回
    public DateTime CreateTime { get; set; }
}
```

设计思路：
- 一个会签节点 = 一个 `BpmVariableMultiplayer` 记录
- 每个审批人 = 一个 `BpmVariableMultiplayerPersonnel` 记录
- 通过 `UndertakeStatus` 记录每个审批人的处理结果
- 节点通过与否 = 根据会签类型统计投票结果决定

## 会签节点类型定义

我们看源码 `ElementPropertyEnum.cs` 中的定义：

```csharp
public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_SIGN =
    new ElementPropertyEnum(2, "多人会签节点", 
        typeof(BpmnAddFlowElementMultSignAdaptor), 
        typeof(BpmnInsertVariableSubsMultiplayerSignAdaptor));

public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_ORSIGN =
    new ElementPropertyEnum(3, "多人或签节点", 
        typeof(BpmnAddFlowElementMultOrSignAaptor), 
        typeof(BpmnInsertVariableSubsMultiplayerOrSignAdaptor));

public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SIGN_UP_SERIAL =
    new ElementPropertyEnum(9, "加批串行-顺序会签", 
        typeof(BpmnAddFlowElementSignUpSerialAdaptor), 
        null);

public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_SIGN_IN_ORDER =
    new ElementPropertyEnum(21, "多人顺序会签节点", 
        typeof(BpmnAddFlowElementSignUpSerialAdaptor), 
        typeof(BpmnInsertVariableSubsMultiplayerSignAdaptor));
```

不同类型对应不同的 Adaptor 处理，Adaptor 负责把流程定义保存到数据库。

## 启动流程时，会签节点初始化

当流程启动走到会签节点，AntFlowCore 会调用 `BpmnInsertVariableSubsMultiplayerSignAdaptor` 初始化会签数据：

看源码 `BpmnInsertVariableSubsMultiplayerSignAdaptor.cs`:

```csharp
public void InsertVariableSubs(BpmnConfCommonElementVo elementVo, long variableId)
{
    // 1. 创建会签实例记录
    var variableMultiplayer = new BpmVariableMultiplayer
    {
        VariableId = variableId,
        ElementId = elementVo.ElementId,
        ElementName = elementVo.ElementName,
        NodeId = elementVo.NodeId,
        CollectionName = elementVo.CollectionName,
        SignType = (int)SignTypeEnum.SIGN_TYPE_SIGN, // 会签 = 需要全部通过
        CreateTime = DateTime.Now,
        TenantId = MultiTenantUtil.GetCurrentTenantId(),
    };

    _bpmVariableMultiplayerService.baseRepo.Insert(variableMultiplayer);

    // 2. 给每个审批人创建记录
    IDictionary<string,string> assigneeMap = elementVo.AssigneeMap;
    long variableMultiplayerId = variableMultiplayer.Id;

    List<BpmVariableMultiplayerPersonnel> personnelList = elementVo.CollectionValue
        .Select(o => new BpmVariableMultiplayerPersonnel
        {
            VariableMultiplayerId = variableMultiplayerId,
            Assignee = o,
            AssigneeName = assigneeMap != null && 
                assigneeMap.TryGetValue(o, out var value) ? value : "",
            UndertakeStatus = 0, // 初始状态 = 未处理
            CreateTime = DateTime.Now,
        })
        .ToList();

    _bpmVariableMultiplayerPersonnelService.baseRepo.Insert(personnelList);
}
```

或签的初始化几乎一样，只是 `SignType` 不同：

```csharp
SignType = (int)SignTypeEnum.SIGN_TYPE_OR_SIGN, // 或签 = 一票通过
```

初始化完成后：
- 会签节点有了一条主记录
- 每个审批人都有一条未处理记录
- 接下来给每个审批人创建一个任务

## 创建任务

会签节点初始化完成后，AntFlowCore 会给每个审批人创建一个独立的任务：

```csharp
// 伪代码
foreach (var assignee in personnelList)
{
    var task = new AFTask
    {
        ProcessInstanceId = instance.Id,
        NodeId = currentNode.Id,
        Assignee = assignee.Assignee,
        // ...
    };
    await _taskService.InsertAsync(task);
}
```

所以每个审批人在自己的待办列表里都能看到这个任务。

## 审批完成后，判断是否通过

当一个审批人完成审批（同意/驳回），AntFlowCore 会更新该审批人的状态，然后根据会签类型判断节点是否通过：

### 核心判断逻辑

我们整理一下判断逻辑：

| 会签类型 | 判断通过条件 |
|---------|-------------|
| **或签** | 任意一个审批人同意 → 节点通过，直接推进流程 |
| **会签（全通过）** | 所有审批人都同意 → 节点通过；任意一个驳回 → 节点驳回 |
| **顺序会签** | 当前审批人完成，走到下一个，直到全部完成 |

### 或签处理流程

或签（一票通过）处理：

```csharp
// 审批人点击同意
UpdateUndertakeStatus(task, currentAssignee, Passed);

// 只要有一个同意，节点就通过
if (signType == 或签)
{
    if (hasAnyApproved())
    {
        // 节点通过，推进流程到下一个节点
        await ProcessToNextNode();
        // 取消其他未处理审批人的任务
        cancelOtherTasks();
    }
}
```

所以或签场景下，第一个人同意了，其他人的任务自动取消，流程继续往下走。

### 会签（全票通过）处理

```csharp
// 更新当前审批人状态
UpdateUndertakeStatus(task, currentAssignee, approveOrReject);

if (signType == 会签)
{
    if (hasAnyRejected())
    {
        // 任意一个驳回，整个节点驳回
        RejectNode();
        return;
    }
    
    if (allApproved())
    {
        // 全部同意了，推进流程
        await ProcessToNextNode();
    }
    // 否则还有人没审批，等待...
}
```

必须所有人都同意才能通过，只要有一个人驳回，整个节点就驳回。

### 顺序会签处理

顺序会签需要按顺序一个一个审批：

```csharp
// 当前序号的审批人审批完成
UpdateUndertakeStatus(currentIndex, approveOrReject);

if (approveOrReject == Rejected)
{
    // 驳回，整个节点驳回
    RejectNode();
    return;
}

if (currentIndex == lastIndex && approved)
{
    // 最后一个人也审批完了，全部通过，推进流程
    ProcessToNextNode();
}
else
{
    // 创建下一个人的任务，等待下一个人审批
    CreateNextTask(nextIndex);
}
```

顺序就是你配置审批人的顺序，一个批完了才轮到下一个。

## 源码解析：会签投票统计

我们来看实际的统计逻辑，从任务完成开始：

`ProcessApprovalService.cs` 中按钮处理完成后：

```csharp
// 完成当前任务
_taskService.CompleteTask(taskId);

// 更新会签人员状态
_bpmVariableMultiplayerPersonnelService.UpdateStatus(
    personnelId, 
    vo.Flag ? UndertakeStatus.Passed : UndertakeStatus.Rejected);

// 根据会签类型判断是否通过
var multiplayer = _bpmVariableMultiplayerService.GetById(variableMultiplayerId);

if (multiplayer.SignType == SIGN_TYPE_OR_SIGN) // 或签
{
    if (vo.Flag) // 当前人同意了
    {
        // 直接通过，推进流程
        await _processFlowService.ProcessToNextNode(...);
    }
    else
    {
        // 当前人驳回，如果还有其他人，等待；如果是最后一个驳回，节点驳回
        // ...
    }
}
else if (multiplayer.SignType == SIGN_TYPE_SIGN) // 会签（全通过）
{
    var statistics = _bpmVariableMultiplayerPersonnelService.Statistics(multiplayer.Id);
    
    // statistics.ApprovedCount = 同意票数
    // statistics.RejectedCount = 驳回票数
    // statistics.TotalCount = 总票数
    
    if (statistics.RejectedCount > 0)
    {
        // 有一个驳回，整个节点驳回
        await RejectProcess(...);
        return;
    }
    
    if (statistics.ApprovedCount == statistics.TotalCount)
    {
        // 全部同意，推进
        await _processFlowService.ProcessToNextNode(...);
    }
    
    // 否则还有人没审批，等待
}
```

非常清晰，就是统计投票，根据规则判断。

## 百分比通过规则

AntFlowCore 也支持百分比通过规则，比如超过 50% 同意就通过：

你可以自己扩展判断逻辑：

```csharp
public bool IsPass(BpmVariableMultiplayer multiplayer)
{
    var statistics = _personnelService.Statistics(multiplayer.Id);
    
    double passPercent = (double)statistics.ApprovedCount / statistics.TotalCount * 100;
    
    // 配置的百分比阈值，比如 50%
    double threshold = multiplayer.ThresholdPercent ?? 50;
    
    return passPercent >= threshold;
}
```

AntFlowCore 核心已经做好了基础设施，你只需要扩展判断逻辑就行了。

## 加签（额外加签）

AntFlowCore 还支持**加签**，就是在流程运行过程中，额外增加审批人。

使用场景：审批人觉得需要让某人也看看，点击"加签"按钮，添加一个额外的审批人。

AntFlowCore 支持两种加签：

| 加签方式 | 说明 |
|---------|------|
| **串行加签** | 加签人在当前节点之后，按顺序审批 |
| **并行加签** | 加签人和当前节点一起会签 |

加签本质上就是动态添加会签审批人，核心逻辑和上面的会签一样。

## 使用方式：在流程设计器中配置会签

在流程设计器中使用会签非常简单：

1. 拖拽一个"会签节点"到画布
2. 选择会签类型：
   - 多人会签（全部通过）
   - 多人或签（一票通过）
   - 顺序会签
3. 选择审批人（可以用任何审批人查找策略：直接指定、角色、部门负责人、动态查找...）
4. 保存流程定义

就是这么简单，剩下的交给 AntFlowCore 处理。

## 常见问题

### 1. 会签节点支持中途加人吗？

支持，就是上面说的"加签"功能，审批过程中可以动态添加审批人。

### 2. 会签可以驳回吗？驳回了怎么处理？

可以驳回。对于会签（全通过），只要有一个人驳回，整个节点就驳回，流程走驳回分支。对于或签，驳回不影响，只要有人同意就能过。

### 3. 可以看到每个人的审批意见吗？

可以，每个审批人的审批意见都存在任务表中，可以随时查询。

### 4. 顺序会签可以跳转到指定人吗？

可以，你可以在流程设计时配置顺序，也可以在运行时动态调整。

### 5. 会签支持撤销吗？

支持，审批人在审批完成后，如果还没走完，可以撤回重新审批。

## 总结

AntFlowCore 会签实现非常清晰：

1. **数据库分层**：会签实例 + 审批人记录，清晰分离
2 **初始化**：流程走到会签节点，创建会签实例和每个审批人记录，给每个审批人创建任务
3. **审批投票**：每个审批人审批后更新状态，根据会签类型统计投票
4. **判断规则**：或签一票过，会签全过，顺序会签依次来
5. **推进流程**：满足通过条件就推进到下一个节点，否则驳回或等待

整个设计思路清晰，代码分层合理，支持各种会签场景，扩展也非常方便。

---

**相关链接：**
- [AntFlowCore 审批人查找策略详解](./AntFlowCore-审批人查找策略详解.md)
- [AntFlowCore 自定义按钮开发实战](./AntFlowCore-自定义按钮开发实战.md)
- [上一篇：审批人查找策略详解](./AntFlowCore-审批人查找策略详解.md)
