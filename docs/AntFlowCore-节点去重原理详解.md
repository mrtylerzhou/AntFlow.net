# AntFlowCore 节点去重原理详解

## 前言

在流程配置中，经常会出现同一个审批人在一条流程中出现多次的情况。比如：

```
申请人填写 → 部门经理审批 → 总监审批 → 总经理审批
                  ↓
            总经理同时也是部门经理，已经出现过一次了
```

这个时候需要去重，避免同一个审批人重复审批。AntFlowCore 原生支持两种去重模式：前去重和后去重。本文详细讲解去重原理和使用方法。

## 为什么需要去重？

同一个审批人重复出现在流程路径中，如果不去重：
- 同一个审批人会收到多个任务，需要审批多次，影响体验
- 如果审批人已经审批过一次，没必要再审批第二次
- 浪费审批时间，降低效率

所以需要自动去重，同一个审批人只需要审批一次。

## 去重模式

AntFlowCore 支持三种去重模式：

| 模式 | 枚举 | 说明 | 适用场景 |
|------|------|-------|---------|
| 不去重 | `DEDUPLICATION_TYPE_NULL` | 保留所有节点，不去重 | 需要同一个人审批多次 |
| 前去重 | `DEDUPLICATION_TYPE_FORWARD` | 当一个审批人重复出现，**只保留最后一次**，前面出现的跳过 | 顺序审批，后面需要审批，前面已经审批过就跳过 |
| 后去重 | `DEDUPLICATION_TYPE_BACKWARD` | 当一个审批人重复出现，**只保留第一次**，后面出现的跳过 | 并行分支，同一个人在多个分支，只需要审批一次 |

我们一个个来看。

## 前去重原理

前去重的核心逻辑：**从前往后遍历，同一个审批人已经出现过了，后面再出现就去掉**。

源码 `BpmnDeduplicationFormatService.ForwardDeduplication`:

```csharp
public BpmnConfVo ForwardDeduplication(
    BpmnConfVo bpmnConfVo, 
    BpmnStartConditionsVo bpmnStartConditions)
{
    // 1. 建立节点ID字典
    Dictionary<string, BpmnNodeVo> mapNodes = new Dictionary<string, BpmnNodeVo>();
    foreach (var vo in bpmnConfVo.Nodes)
    {
        mapNodes[vo.NodeId] = vo;
        // 记录开始节点
        if (vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
        {
            startNodeId = vo.NodeId;
        }
    }

    string initiator = mapNodes[startNodeId].Params.Assignee.Assignee;
    BpmnNodeVo bpmnNodeVo = mapNodes[startNodeId];
    List<string> approverList = new List<string> { initiator };

    // 从开始节点一直往后遍历
    while (!string.IsNullOrEmpty(bpmnNodeVo.Params.NodeTo))
    {
        bpmnNodeVo = mapNodes[bpmnNodeVo.Params.NodeTo];

        // 单人节点去重
        if (bpmnNodeVo.Params.ParamType == 1)
        {
            SinglePlayerNodeDeduplication(bpmnNodeVo, 
                new HashSet<string>(), 
                approverList);
            continue;
        }

        // 多人节点去重
        if (bpmnNodeVo.Params.ParamType == 2)
        {
            MultiPlayerNodeDeduplication(bpmnNodeVo, 
                new HashSet<string>(), 
                approverList, 
                false);
        }
    }

    return bpmnConfVo;
}
```

### 单人节点去重

```csharp
private void SinglePlayerNodeDeduplication(
    BpmnNodeVo bpmnNodeVo,
    HashSet<String> alreadyProcessedNods, 
    List<string> approverList)
{
    if (bpmnNodeVo.Params.IsNodeDeduplication == 1 ||
        alreadyProcessedNods.Contains(bpmnNodeVo.NodeId))
    {
        return;
    }

    BpmnNodeParamsAssigneeVo assignee = bpmnNodeVo.Params.Assignee;
    if (approverList.Contains(assignee.Assignee))
    {
        // 审批人已经出现过了，标记去重
        assignee.IsDeduplication = 1;
        bpmnNodeVo.Params.IsNodeDeduplication = 1;
    }
    else
    {
        approverList.Add(assignee.Assignee);
    }
    alreadyProcessedNods.Add(bpmnNodeVo.NodeId);
}
```

核心逻辑：
- 检查审批人是否已经在 `approverList` 中
- 如果已经在，标记这个节点为去重，不会创建任务
- 如果不在，添加到列表，保留这个节点

### 多人节点（会签）去重

```csharp
private void MultiPlayerNodeDeduplication(
    BpmnNodeVo bpmnNodeVo,
    HashSet<String> alreadyProcessedNods, 
    List<string> approverList, 
    bool flag)
{
    if (bpmnNodeVo.DeduplicationExclude ||
        bpmnNodeVo.Params.IsNodeDeduplication == 1 ||
        alreadyProcessedNods.Contains(bpmnNodeVo.NodeId))
    {
        return;
    }

    List<BpmnNodeParamsAssigneeVo> assigneeList = bpmnNodeVo.Params.AssigneeList;
    int isNodeDeduplication = 1;
    
    foreach (var assignee in assigneeList)
    {
        if (assignee.IsDeduplication == 1)
        {
            continue;
        }

        if (approverList.Contains(assignee.Assignee))
        {
            // 审批人已经出现过，标记去重
            assignee.IsDeduplication = 1;
        }
        else
        {
            if (flag)
            {
                approverList.Add(assignee.Assignee);
            }
            isNodeDeduplication = 0; // 至少还有一个人需要审批，节点不去重
        }
    }
    
    bpmnNodeVo.Params.IsNodeDeduplication = isNodeDeduplication;
    alreadyProcessedNods.Add(bpmnNodeVo.NodeId);
}
```

多人节点去重：
- 遍历所有审批人，挨个检查是否已经出现过
- 已经出现过的标记去重
- 如果至少还有一个审批人需要审批，节点保留
- 如果全部都去重了，整个节点标记去重，不会创建任务

## 前去重示例

我们来看一个实际例子：

```
发起人 → 部门经理审批 → 总监审批 → 总经理审批
```

总经理同时也是总监，所以路径上：

| 节点 | 审批人 | 是否已经存在 | 结果 |
|------|---------|--------------|------|
| 发起人 | 张三 | 否 | 保留，添加到列表 |
| 部门经理 | 李四 | 否 | 保留，添加到列表 |
| 总监 | 王五 | 否 | 保留，添加到列表 |
| 总经理 | 王五 | **是** | 标记去重，不创建任务 |

最后实际需要审批：发起人 → 部门经理 → 总监 → （总经理去重）流程结束。

因为总经理已经在总监节点审批过了，所以不需要再审批一次。

## 后去重原理

后去重的核心逻辑：**从前往后遍历，同一个审批人已经出现过了，后面再出现就去掉，只保留第一次**。

源码：

```csharp
public BpmnConfVo BackwardDeduplication(
    BpmnConfVo bpmnConfVo, 
    BpmnStartConditionsVo bpmnStartConditions)
{
    List<string> approverList = new List<string>();
    string startNodeId = null;
    Dictionary<string, BpmnNodeVo> mapNodes = new Dictionary<string, BpmnNodeVo>();
    
    foreach (var vo in bpmnConfVo.Nodes)
    {
        mapNodes[vo.NodeId] = vo;
        if (vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
        {
            startNodeId = vo.NodeId;
        }
    }

    string initiator = mapNodes[startNodeId].Params.Assignee.Assignee;
    approverList.Add(initiator);
    BpmnNodeVo bpmnNodeVo = mapNodes[startNodeId];

    // 递归处理所有节点，包括并行网关
    ProcessNodeRecursively(bpmnNodeVo, 
        new HashSet<string>(), 
        mapNodes, 
        approverList);

    return bpmnConfVo;
}
```

`ProcessNodeRecursively` 递归处理所有节点，包括并行网关：

```csharp
private void ProcessNodeRecursively(
    BpmnNodeVo bpmnNodeVo,
    HashSet<String> alreadyProcessedNodes, 
    Dictionary<String, BpmnNodeVo> mapNodes, 
    List<String> approverList) {

    // 处理并行网关，所有分支都要处理
    if((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == bpmnNodeVo.NodeType){
        List<String> parallelNodeToIds = bpmnNodeVo.NodeTo;
        foreach (String parallelNodeToId in parallelNodeToIds) {
            if (mapNodes.TryGetValue(parallelNodeToId, out var parallelNodeTo)) {
                // 递归处理每个并行分支
                ProcessNodeRecursively(parallelNodeTo, 
                    alreadyProcessedNodes, 
                    mapNodes, 
                    approver);
            }
        }
    }

    // 处理当前节点去重
    if (bpmnNodeVo.Params.ParamType == 1) {
        SinglePlayerNodeDeduplication(bpmnNodeVo, alreadyProcessedNodes, approverList);
    }else if (bpmnNodeVo.Params.ParamType == 2) {
        MultiPlayerNodeDeduplication(bpmnNodeVo, alreadyProcessedNodes, approverList, true);
    }

    // 继续处理下一个节点
    String nodeTo = GetNodeTo(bpmnNodeVo);
    if (string.IsNullOrEmpty(nodeTo)) {
        return;
    }
    bpmnNodeVo = GetNextNodeVo(mapNodes.Values, nodeTo);
    ProcessNodeRecursively(bpmnNodeVo, alreadyProcessedNodes, mapNodes, approverList);
}
```

后去重支持并行网关，会递归处理所有并行分支，每个分支都做去重。

## 后去重示例

并行网关场景，多个分支，同一个人在不同分支都需要审批：

```
┌───────────┐         ┌───────────┐
│ 分支1：部门经理  │         │ 分支2：财务   │
│    王五       │         │    王五      │
└───────────┘         └───────────┘
             ↓
          汇总节点
```

开启后去重，同一个王五在两个分支都出现：

- 处理第一个分支：王五不在列表，添加进去，保留任务
- 处理第二个分支：王五已经在列表，标记去重，不创建任务

最终王五只需要审批一次，在第一个分支审批就可以了，第二个分支自动去重。

如果是前去重，结果会是：
- 第一个分支保留，第二个分支也保留
- 王五需要审批两次

所以并行网关同一个人多分支场景，适合用后去重，只审批一次。

## 前去重 vs 后去重 怎么选

| 场景 | 推荐模式 | 原因 |
|------|----------|------|
| 顺序流程，同一个人重复出现 | 前去重 | 前面已经审批过了，后面不需要 |
| 并行网关，同一个人在多个分支 | 后去重 | 只保留第一次，只需要审批一次 |
| 需要同一个人审批多次 | 不去重 | 每个节点都保留，需要审批多次 |

## 什么时候触发去重

去重是在**流程启动**的时候触发的：

```csharp
// 启动流程前，先去重
var deduplicated = _deduplicationFormatService
    .ForwardDeduplication(bpmnConf, startParams);

// 然后启动流程
await _processStarter.StartProcess(...);
```

启动流程的时候，根据配置对节点去重，然后创建任务。所以去重只发生一次，运行时不会重复计算，性能很好。

## 跳过去重

如果你配置了"不去重"，所有节点都保留，同一个审批人会收到多个任务，需要审批多次。适用于：
- 业务确实需要同一个人审批多次
- 每个节点审批内容不一样

## 已去重的节点会创建任务吗？

不会。节点被标记去重后，AntFlowCore 跳过创建任务，直接推进到下一个节点。所以审批人不会收到重复任务。

## 总结

AntFlowCore 节点去重设计非常简洁：

1. **三种模式**：不去重、前去重、后去重，覆盖绝大多数场景
2. **递归处理**：支持并行网关，所有分支都正确去重
3. **启动时一次计算**：运行时没有开销，性能很好
4. **精细粒度**：支持单人节点单个审批人去重，支持多人节点多个审批人逐个去重

根据你的业务场景选择合适的去重模式，可以避免审批人重复审批，提升审批体验。

---

**相关链接：**
- [AntFlowCore 会签节点实现原理详解](./AntFlowCore-会签节点实现原理详解.md)
- [AntFlowCore 审批人查找策略详解](./AntFlowCore-审批人查找策略详解.md)
- [上一篇：自定义 JSON 转换器详解](./AntFlowCore-自定义-JSON-转换器详解.md)
