# AntFlowCore .NET 适配器模式详解

## 1. 概述

适配器模式（Adaptor Pattern）是 AntFlowCore 的核心扩展机制。引擎通过六类适配器实现与外部系统的解耦，使业务系统可以在不修改引擎核心代码的前提下，自定义审批人分配、条件判断、通知发送等行为。

## 2. 适配器类型总览

| 适配器类型 | 接口 | 作用 | 注册数量 |
|-----------|------|------|---------|
| 审批人适配器 | `AbstractBpmnPersonnelAdaptor` | 确定节点的审批人 | 14种 |
| 条件适配器 | `IBpmnNodeConditionsAdaptor` | 设置节点条件响应 | 4种 |
| 条件判断器 | `IConditionJudge` | 评估条件是否满足 | 9种 |
| 节点元素适配器 | `IBpmnAddFlowElementAdaptor` | 添加BPMN流程元素 | 5种 |
| 节点属性适配器 | `IAdaptorService` (NodeProperty*) | 处理节点属性 | 14种 |
| 流程操作适配器 | `IProcessOperationAdaptor` | 处理流程操作 | 18种 |
| 通知适配器 | `IProcessNoticeAdaptor` | 发送审批通知 | 3种 |
| 回调适配器 | `ICallbackAdaptor` | 处理第三方回调 | 2种 |
| 表单操作适配器 | `IFormOperationAdaptor<T>` | 与业务表单交互 | 2种 |

## 3. 核心接口 IAdaptorService

所有适配器的基接口，提供业务对象匹配能力：

```csharp
public interface IAdaptorService
{
    // 存储支持的类型（线程安全字典）
    static readonly ConcurrentDictionary<string, List<Enum>> SupportedBusiness = new();

    /// <summary>
    /// 设置支持的子业务类型（子类重写）
    /// </summary>
    void SetSupportBusinessObjects();

    /// <summary>
    /// 添加支持的子业务类型
    /// </summary>
    void AddSupportBusinessObjects(params Enum[] businessObjects);

    /// <summary>
    /// 检查是否支持指定的子业务类型
    /// </summary>
    bool IsSupportBusinessObject(Enum businessObject);
}
```

**工作原理**：每个适配器通过 `SetSupportBusinessObjects` 声明自己支持的枚举值，引擎运行时通过 `IsSupportBusinessObject` 查找匹配的适配器。

## 4. 审批人适配器（Personnel Adaptor）

### 4.1 架构

```
                    ┌─────────────────────────────┐
                    │  AbstractBpmnPersonnelAdaptor│
                    │  (抽象基类)                   │
                    │  - SetNodeParams()           │
                    │  - SetEmployeeName()         │
                    │  - AssigneeListUniq()        │
                    └──────────────┬──────────────┘
                                   │ 继承
              ┌────────────────────┼────────────────────┐
              │                    │                    │
    ┌─────────┴────────┐ ┌────────┴────────┐ ┌────────┴────────┐
    │Customizable      │ │DirectLeader     │ │Role            │
    │PersonnelAdaptor  │ │PersonnelAdaptor │ │PersonnelAdaptor │
    │(指定人员)        │ │(直属领导)        │ │(角色)          │
    └──────────────────┘ └─────────────────┘ └─────────────────┘
```

### 4.2 抽象基类 AbstractBpmnPersonnelAdaptor

```csharp
public abstract class AbstractBpmnPersonnelAdaptor : IAdaptorService
{
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
    private readonly IBpmnPersonnelProviderService _bpmnPersonnelProviderService;

    public AbstractBpmnPersonnelAdaptor(
        IBpmnPersonnelProviderService bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService)
    {
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
        _bpmnPersonnelProviderService = bpmnPersonnelProviderService;
    }

    /// <summary>
    /// 核心方法：设置节点审批人参数
    /// </summary>
    public void SetNodeParams(
        BpmnNodeVo nodeVo, 
        BpmnStartConditionsVo startConditionsVo, 
        BpmnNodeParamTypeEnum nodeParamTypeEnum, 
        string nextId, 
        Dictionary<string, BpmnNodeVo> mapPreNodes, 
        HashSet<BpmnNodeVo> setAddNodes)
    {
        // 1. 处理有序签名节点（会签场景）
        var orderedNodeType = nodeVo.OrderedNodeType;
        if (orderedNodeType.HasValue)
        {
            var orderNodeTypeEnum = OrderNodeTypeEnumExtensions.GetByCode(orderedNodeType.Value);
            var abstractOrderedSignNodeAdps = ServiceProviderUtils
                .GetServices<AbstractOrderedSignNodeAdp>();
            // 查找匹配的有序签名适配器并格式化节点
            // ...
            return;
        }

        // 2. 处理上一节点指定审批人场景
        // ...

        // 3. 获取审批人列表
        List<BpmnNodeParamsAssigneeVo> assigneeList = AssigneeListUniq(
            _bpmnPersonnelProviderService.GetAssigneeList(nodeVo, startConditionsVo));
        
        // 4. 设置审批人到节点参数
        SetAssigneeOrList(paramsVo, assigneeList, nodeParamTypeEnum);
        
        // 5. 设置审批人姓名
        SetEmployeeName(assigneeList, nodeName);
    }

    public abstract void SetSupportBusinessObjects();
}
```

### 4.3 审批人提供者接口

```csharp
public interface IBpmnPersonnelProviderService
{
    /// <summary>
    /// 获取审批人列表（核心方法）
    /// </summary>
    List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo);
}
```

### 4.4 内置审批人适配器

| 适配器类 | 枚举值 | 说明 |
|---------|--------|------|
| `CustomizablePersonnelAdaptor` | `CUSTOMIZABLE_PERSONNEL` | 指定人员审批 |
| `DirectLeaderPersonnelAdaptor` | `DIRECT_LEADER` | 直属领导审批 |
| `HrbpPersonnelAdaptor` | `HRBP` | HRBP 审批 |
| `LevelPersonnelAdaptor` | `LEVEL` | 按级别审批 |
| `LoopPersonnelAdaptor` | `LOOP` | 循环审批 |
| `OutSidePersonnelAdaptor` | `OUTSIDE` | 外部系统审批 |
| `RolePersonnelAdaptor` | `ROLE` | 角色审批 |
| `StartUserPersonnelAdaptor` | `START_USER` | 发起人审批 |
| `UserPointedPersonnelAdaptor` | `USER_POINTED` | 用户指定审批 |
| `BusinessTablePersonnelAdaptor` | `BUSINESS_TABLE` | 业务表字段审批 |
| `FormRelatedPersonnelAdaptor` | `FORM_RELATED` | 表单关联审批 |
| `UDRPersonnelAdaptor` | `UDR` | 用户自定义审批 |
| `PrevNodeRelatedPersonnelAdaptor` | `PREV_NODE_RELATED` | 上一节点关联审批 |
| `ApprovedUsersPersonnelAdaptor` | `APPROVED_USERS` | 已审批人审批 |

### 4.5 自定义审批人适配器示例

```csharp
// 1. 定义枚举
public enum PersonnelEnum
{
    CUSTOMIZABLE_PERSONNEL = 1,
    DIRECT_LEADER = 2,
    // ... 添加自定义枚举值
    MY_CUSTOM_PERSONNEL = 100
}

// 2. 实现审批人提供者
public class MyCustomPersonnelProvider : IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(
        BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        // 自定义逻辑：根据业务数据计算审批人
        var assigneeList = new List<BpmnNodeParamsAssigneeVo>();
        
        // 示例：从业务条件中获取部门经理ID
        if (startConditionsVo.Conditions.TryGetValue("deptManagerId", out var managerId))
        {
            assigneeList.Add(new BpmnNodeParamsAssigneeVo 
            { 
                Assignee = managerId.ToString(),
                AssigneeName = "部门经理"
            });
        }
        
        return assigneeList;
    }
}

// 3. 实现适配器
public class MyCustomPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public MyCustomPersonnelAdaptor(
        MyCustomPersonnelProvider provider,
        IBpmnEmployeeInfoProviderService employeeService) 
        : base(provider, employeeService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        AddSupportBusinessObjects(PersonnelEnum.MY_CUSTOM_PERSONNEL);
    }
}

// 4. 注册到 DI 容器
services.AddSingleton<MyCustomPersonnelProvider>();
services.AddSingleton<IBpmnPersonnelProviderService, MyCustomPersonnelProvider>();
services.AddSingleton<AbstractBpmnPersonnelAdaptor, MyCustomPersonnelAdaptor>();
```

## 5. 条件适配器（Condition Adaptor）

### 5.1 接口定义

```csharp
public interface IBpmnNodeConditionsAdaptor
{
    /// <summary>
    /// 设置条件响应配置
    /// </summary>
    void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo);
}
```

### 5.2 内置条件适配器

| 适配器类 | 说明 |
|---------|------|
| `BpmnNodeConditionsAccountTypeAdaptor` | 账户类型条件 |
| `BpmnNodeConditionsEmptyAdaptor` | 空条件（默认） |
| `BpmnNodeConditionsPurchaseTypeAdaptor` | 采购类型条件 |
| `BpmnTemplateMarkAdaptor` | 模板标记条件 |

### 5.3 条件判断器接口

```csharp
public interface IConditionJudge
{
    /// <summary>
    /// 判断条件是否满足
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="conditionsConf">条件配置</param>
    /// <param name="bpmnStartConditions">启动条件</param>
    /// <param name="coundGroup">条件组</param>
    /// <param name="index">条件索引</param>
    /// <returns>条件是否满足</returns>
    bool Judge(string nodeId, 
               BpmnNodeConditionsConfBaseVo conditionsConf, 
               BpmnStartConditionsVo bpmnStartConditions,
               int coundGroup, int index);
}
```

### 5.4 内置条件判断器

| 判断器类 | 说明 |
|---------|------|
| `ThirdAccountJudgeService` | 第三方账户判断 |
| `AskLeaveJudge` | 请假判断 |
| `PurchaseTotalMoneyJudge` | 采购总金额判断 |
| `NumberOperatorJudgeService` | 数值运算判断 |
| `BpmnTemplateMarkJudge` | 模板标记判断 |
| `LFStringConditionJudge` | 低代码字符串条件 |
| `LFNumberFormatJudge` | 低代码数值条件 |
| `LFDateConditionJudge` | 低代码日期条件 |
| `LFDateTimeConditionJudge` | 低代码日期时间条件 |
| `LFCollectionConditionJudge` | 低代码集合条件 |

## 6. 节点元素适配器（Element Adaptor）

### 6.1 接口定义

```csharp
public interface IBpmnAddFlowElementAdaptor
{
    /// <summary>
    /// 向流程中添加BPMN元素
    /// </summary>
    void AddFlowElement(
        BpmnConfCommonElementVo elementVo,
        AFProcess process,
        Dictionary<string, object> startParamMap,
        BpmnStartConditionsVo bpmnStartConditions);
}
```

### 6.2 内置元素适配器

| 适配器类 | 说明 |
|---------|------|
| `BpmnAddFlowElementSingleAdaptor` | 单人审批节点 |
| `BpmnAddFlowElementLoopAdaptor` | 循环审批节点 |
| `BpmnAddFlowElementMultOrSignAaptor` | 多人或签节点 |
| `BpmnAddFlowElementSignUpSerialAdaptor` | 依次加签节点 |
| `BpmnAddFlowElementMultSignAdaptor` | 多人会签节点 |

### 6.3 单人审批节点适配器实现

```csharp
public class BpmnAddFlowElementSingleAdaptor : IBpmnAddFlowElementAdaptor
{
    private readonly ILogger<BpmnAddFlowElementSingleAdaptor> _logger;

    public BpmnAddFlowElementSingleAdaptor(ILogger<BpmnAddFlowElementSingleAdaptor> logger)
    {
        _logger = logger;
    }

    public void AddFlowElement(
        BpmnConfCommonElementVo elementVo,
        AFProcess process,
        Dictionary<string, object> startParamMap,
        BpmnStartConditionsVo bpmnStartConditions)
    {
        // 创建用户任务元素
        var userTask = BpmnBuildUtils.CreateUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            elementVo.AssigneeParamName
        );
        process.AddFlowElement(userTask);

        // 添加到开始参数Map
        if (!string.IsNullOrEmpty(elementVo.AssigneeParamName))
        {
            startParamMap[elementVo.AssigneeParamName] = elementVo.AssigneeParamValue;
        }

        _logger.LogInformation(
            $"Added user task: {elementVo.ElementId}, Assignee: {elementVo.AssigneeParamName}");
    }
}
```

## 7. 流程操作适配器（Process Operation Adaptor）

### 7.1 接口定义

```csharp
public interface IProcessOperationAdaptor : IAdaptorService
{
    /// <summary>
    /// 执行流程按钮操作
    /// </summary>
    void DoProcessButton(BusinessDataVo vo);
}
```

### 7.2 内置流程操作适配器

| 适配器类 | 说明 |
|---------|------|
| `SubmitProcessService` | 提交流程 |
| `ResubmitProcessService` | 重新提交 |
| `EndProcessService` | 结束流程 |
| `OutSideAccessSubmitProcessService` | 外部系统提交 |
| `ChangeAssigneeProcessService` | 变更处理人 |
| `TransferAssigneeProcessService` | 转交处理人 |
| `UndertakeProcessService` | 承办处理 |
| `BackToModifyService` | 退回修改 |
| `ProcessForwardService` | 转发流程 |
| `RemoveAssigneeProcessService` | 移除处理人 |
| `AddAssigneeProcessService` | 添加处理人 |
| `RemoveFutureAssigneeProcessService` | 移除未来处理人 |
| `AddFutureAssigneeProcessService` | 添加未来处理人 |
| `ChangeFutureAssigneeProcessService` | 变更未来处理人 |
| `TaskRecoverProcessSerivce` | 任务恢复 |
| `FastForwardProcessService` | 快速前进 |
| `RemoveCurrentNodeProcessService` | 移除当前节点 |
| `RemoveFutureNodeProcessService` | 移除未来节点 |
| `InsertNodeAfterCurrentOrFutureService` | 插入节点 |
| `SaveDraftProcessService` | 保存草稿 |

## 8. 通知适配器（Notice Adaptor）

### 8.1 接口定义

```csharp
public interface IProcessNoticeAdaptor
{
    /// <summary>
    /// 批量发送消息
    /// </summary>
    void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
    
    /// <summary>
    /// 获取支持的通知类型编码
    /// </summary>
    int GetSupportCode();
}
```

### 8.2 抽象基类

```csharp
public abstract class AbstractMessageSendAdaptor<T> : IProcessNoticeAdaptor
{
    protected readonly IMessageService _messageService;
    private readonly ILogger _logger;

    protected Dictionary<string, T> MessageProcessing(
        List<UserMsgVo> userMsgVos, Func<UserMsgVo, T> fun)
    {
        if (userMsgVos.IsEmpty())
        {
            _logger.LogInformation("发送的消息内容不能为空!");
            return null;
        }

        Dictionary<string, T> dic = new Dictionary<string, T>();
        foreach (UserMsgVo userMsgVo in userMsgVos)
        {
            T result = fun(userMsgVo);
            dic[userMsgVo.UserId] = result;
        }
        return dic;
    }

    public abstract void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
    public abstract int GetSupportCode();
}
```

### 8.3 内置通知适配器

| 适配器类 | 通知类型 | 说明 |
|---------|---------|------|
| `EmailSendAdaptor` | `EMAIL_TYPE` | 邮件通知 |
| `AppPushAdaptor` | `APP_PUSH_TYPE` | App 推送通知 |
| `SMSSendAdaptor` | `SMS_TYPE` | 短信通知 |

### 8.4 邮件通知适配器实现

```csharp
public class EmailSendAdaptor : AbstractMessageSendAdaptor<MailInfo>
{
    public EmailSendAdaptor(
        IMessageService messageService, 
        ILogger<EmailSendAdaptor> logger) : base(messageService, logger)
    {
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        Dictionary<string, MailInfo> stringMailInfoMap = base.MessageProcessing(
            userMsgVos, UserMsgUtils.BuildMailInfo);
        _messageService.SendMailBatch(stringMailInfoMap);
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.EMAIL_TYPE.Code;
    }
}
```

## 9. 表单操作适配器（Form Operation Adaptor）

### 9.1 接口定义

```csharp
public interface IFormOperationAdaptor<in T> where T : BusinessDataVo
{
    BpmnStartConditionsVo PreviewSetCondition(T vo);    // 预览条件
    BpmnStartConditionsVo LaunchParameters(T vo);       // 启动参数
    void OnInitData(T vo);                              // 初始化数据
    void OnQueryData(T vo);                             // 查询数据
    void OnSubmitData(T vo);                            // 提交数据
    void OnConsentData(T vo);                           // 同意审批回调
    void OnBackToModifyData(T vo);                      // 退回修改回调
    void OnCancellationData(T vo);                      // 取消流程回调
    void OnFinishData(BusinessDataVo vo);               // 流程结束回调
    bool? AutoCondition(T vo) => null;                  // 自定义自动节点条件
    bool? AutomaticCondition(T vo);                     // 自动节点条件评估
    void AutomaticAction(T vo, bool? conditionResult);  // 自动节点动作执行
}
```

### 9.2 内置表单操作适配器

| 适配器类 | 泛型参数 | 说明 |
|---------|---------|------|
| `ThirdPartyAccountApplyFlowService` | `ThirdPartyAccountApplyVo` | 第三方账户申请流程 |
| `LowFlowApprovalService` | `UDLFApplyVo` | 低代码审批流程 |

## 10. 适配器匹配机制

### 10.1 匹配流程

```
节点配置 (NodeProperty=2)
       │
       ▼
┌─────────────────────────┐
│ 遍历所有注册的适配器      │
│ (AbstractBpmnPersonnelAdaptor) │
└────────────┬────────────┘
             │
             ▼
┌─────────────────────────┐
│ 调用 IsSupportBusinessObject │
│ 检查适配器是否支持该类型  │
└────────────┬────────────┘
             │
        ┌────┴────┐
        │ 匹配成功? │
        └────┬────┘
        Yes  │  No
        ▼    ▼
   使用适配器  继续遍历
   设置审批人
```

### 10.2 匹配代码示例

```csharp
// 引擎内部匹配逻辑
var adaptors = ServiceProviderUtils.GetServices<AbstractBpmnPersonnelAdaptor>();
foreach (var adaptor in adaptors)
{
    if (adaptor.IsSupportBusinessObject(nodePropertyEnum))
    {
        adaptor.SetNodeParams(nodeVo, startConditions, paramType, nextId, preNodes, addNodes);
        break;
    }
}
```

## 11. 适配器注册方式

所有适配器在 `ServiceRegistration.AntFlowServiceSetUp` 中统一注册：

```csharp
// 审批人提供者注册
services.AddSingleton<IBpmnPersonnelProviderService, DirectLeaderPersonnelProvider>();
services.AddSingleton<IBpmnPersonnelProviderService, CustomizePersonnelProvider>();
// ... 更多提供者

// 审批人适配器注册
services.AddSingleton<AbstractBpmnPersonnelAdaptor, CustomizablePersonnelAdaptor>();
services.AddSingleton<AbstractBpmnPersonnelAdaptor, DirectLeaderPersonnelAdaptor>();
// ... 更多适配器

// 条件适配器注册
services.AddSingleton<IBpmnNodeConditionsAdaptor, BpmnNodeConditionsAccountTypeAdaptor>();
// ... 更多条件适配器

// 通知适配器注册
services.AddSingleton<IProcessNoticeAdaptor, EmailSendAdaptor>();
services.AddSingleton<IProcessNoticeAdaptor, AppPushAdaptor>();
services.AddSingleton<IProcessNoticeAdaptor, SMSSendAdaptor>();
```

## 12. 设计优势

1. **开闭原则**：新增功能只需添加适配器，无需修改引擎核心
2. **单一职责**：每个适配器只负责一种业务逻辑
3. **可组合**：多个适配器可以同时注册，引擎自动匹配
4. **可测试**：每个适配器可独立进行单元测试
5. **松耦合**：通过接口和DI实现组件解耦
