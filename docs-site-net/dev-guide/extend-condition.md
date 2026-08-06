# AntFlowCore .NET 条件扩展指南

## 1. 概述

条件扩展允许自定义流程分支的判断逻辑。AntFlowCore 内置了多种条件类型（账户类型、请假时长、采购费用、数值运算等），当这些不满足需求时，可以通过实现 `IConditionJudge` 接口来添加自定义条件判断器。

## 2. 条件系统架构

```
条件配置 (BpmnNodeConditionsConfBaseVo)
     │
     │ conditionType = 条件类型编码
     ▼
┌──────────────────────────────────────────────┐
│  ConditionService                            │
│  - CheckMatchCondition()                     │
│  - 遍历条件组，查找匹配的 IConditionJudge     │
└──────────────────┬───────────────────────────┘
                   │
                   ▼
┌──────────────────────────────────────────────┐
│  IConditionJudge                             │
│  - Judge(nodeId, conditionsConf,            │
│          startConditions, group, index)      │
└──────────────────┬───────────────────────────┘
                   │
         ┌─────────┴─────────┐
         │                   │
    ConditionTypeEnum    IConditionJudge
    (条件类型定义)        (条件判断实现)
         │                   │
         ▼                   ▼
  ┌─────────────┐    ┌─────────────┐
  │ConditionType│    │Judge()      │
  │Attributes   │    │返回bool     │
  │(特性配置)   │    └─────────────┘
  └─────────────┘
```

## 3. 条件类型定义

条件类型通过 `ConditionTypeEnum` 枚举定义，每个枚举值使用 `[ConditionType]` 特性配置元数据：

```csharp
public enum ConditionTypeEnum
{
    [ConditionType("三方账户",
        FieldName = "AccountType",
        FieldType = 1,
        FieldClass = typeof(int),
        AdaptorClass = typeof(BpmnNodeConditionsAccountTypeAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "AccountType",
        ConditionJudgeClass = typeof(ThirdAccountJudgeService))]
    CONDITION_THIRD_ACCOUNT_TYPE = 1,
    
    [ConditionType("请假时长",
        FieldName = "LeaveHour",
        FieldType = 2,
        FieldClass = typeof(double),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "LeaveHour",
        ConditionJudgeClass = typeof(AskLeaveJudge))]
    CONDITION_BIZ_LEAVE_TIME = 2,
    
    // ... 更多条件类型
    // 低代码条件（10000+）
    [ConditionType("无代码数字流程条件",
        FieldName = "lfConditions",
        FieldType = 2,
        FieldClass = typeof(string),
        ConditionJudgeClass = typeof(LFNumberFormatJudge))]
    CONDITION_TYPE_LF_NUM_CONDITION = 10001,
}
```

### ConditionTypeAttribute 属性说明

| 属性 | 类型 | 说明 |
|------|------|------|
| `Description` | string | 条件描述 |
| `FieldName` | string | 条件字段名称 |
| `FieldType` | int | 字段类型（1=列表，2=对象） |
| `FieldClass` | Type | 条件字段CLR类型 |
| `AdaptorClass` | Type | 条件适配器类型 |
| `AlignmentClass` | Type | 比对对象类型 |
| `AlignmentFieldName` | string | 比对对象字段名 |
| `ConditionJudgeClass` | Type | 条件判断器类型 |

## 4. 内置条件判断器

### 4.1 内置条件判断器列表

| 判断器类 | 条件类型 | 说明 |
|---------|---------|------|
| `ThirdAccountJudgeService` | `CONDITION_THIRD_ACCOUNT_TYPE` | 第三方账户类型判断 |
| `AskLeaveJudge` | `CONDITION_BIZ_LEAVE_TIME` | 请假时长判断 |
| `PurchaseTotalMoneyJudge` | `CONDITION_PURCHASE_FEE` | 采购总金额判断 |
| `NumberOperatorJudgeService` | `CONDITION_TYPE_NUMBER_OPERATOR` | 数值运算判断 |
| `BpmnTemplateMarkJudge` | `CONDITION_TEMPLATEMARK` | 模板标记判断 |
| `LFStringConditionJudge` | `CONDITION_TYPE_LF_STR_CONDITION` | 低代码字符串条件 |
| `LFNumberFormatJudge` | `CONDITION_TYPE_LF_NUM_CONDITION` | 低代码数值条件 |
| `LFDateConditionJudge` | `CONDITION_TYPE_LF_DATE_CONDITION` | 低代码日期条件 |
| `LFDateTimeConditionJudge` | `CONDITION_TYPE_LF_DATE_TIME_CONDITION` | 低代码日期时间条件 |
| `LFCollectionConditionJudge` | `CONDITION_TYPE_LF_COLLECTION_CONDITION` | 低代码集合条件 |

### 4.2 低代码数值条件判断器实现

```csharp
public class LFNumberFormatJudge : AbstractLFConditionJudge
{
    private readonly ILogger<LFNumberFormatJudge> _logger;

    public LFNumberFormatJudge(ILogger<LFNumberFormatJudge> logger)
    {
        _logger = logger;
    }

    public override bool Judge(
        string nodeId, 
        BpmnNodeConditionsConfBaseVo conditionsConf, 
        BpmnStartConditionsVo bpmnStartConditionsVo,
        int group, int index)
    {
        Func<object, object, int, bool> predicate = (dbValue, userValue, op) =>
        {
            var split = dbValue.ToString().Split(',');
            var valueInDbBig1 = decimal.Parse(split[0], CultureInfo.InvariantCulture);
            decimal? valueInDbBig2 = null;

            if (split.Length > 1)
            {
                valueInDbBig2 = decimal.Parse(split[1], CultureInfo.InvariantCulture);
            }

            var userVal = Math.Round(
                decimal.Parse(userValue.ToString(), CultureInfo.InvariantCulture), 
                2, MidpointRounding.AwayFromZero);

            return CompareJudge(valueInDbBig1, valueInDbBig2, userVal, op);
        };

        return base.LfCommonJudge(conditionsConf, bpmnStartConditionsVo, predicate, group, index);
    }
}
```

## 5. 条件评估流程

### 5.1 条件评估入口

```csharp
public class ConditionService : IConditionService
{
    public bool CheckMatchCondition(
        BpmnNodeVo bpmnNodeVo, 
        BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, 
        bool isDynamicConditionGateway)
    {
        string nodeId = bpmnNodeVo.NodeId;
        var groupedConditionParamTypes = conditionsConf.GroupedConditionParamTypes;
        
        bool result = true;
        int? groupRelation = conditionsConf.GroupRelation;
        
        foreach (var conditionTypeEntry in groupedConditionParamTypes)
        {
            int currentGroup = conditionTypeEntry.Key;
            bool currentGroupResult = true;
            
            var conditionParamTypeList = conditionTypeEntry.Value;
            for (var i = 0; i < conditionParamTypeList.Count; i++)
            {
                int conditionParam = conditionParamTypeList[i];
                var conditionTypeEnum = ConditionTypeEnumExtensions.GetEnumByCode(conditionParam);
                var conditionTypeAttributes = conditionTypeEnum.Value.GetAttributes();
                Type conditionJudgeClassType = conditionTypeAttributes.ConditionJudgeClass;
                
                // 从 DI 容器查找对应的 IConditionJudge
                IEnumerable conditionJudgeServices = ServiceProviderUtils
                    .GetServices(typeof(IConditionJudge));
                
                IConditionJudge conditionJudge = null;
                foreach (var service in conditionJudgeServices)
                {
                    if (service.GetType() == conditionJudgeClassType)
                    {
                        conditionJudge = (IConditionJudge)service;
                        break;
                    }
                }
                
                bool judgeResult = conditionJudge.Judge(
                    nodeId, conditionsConf, bpmnStartConditionsVo, currentGroup, i);
                
                // 根据组内关系（AND/OR）合并结果
                // ...
            }
        }
        return result;
    }
}
```

### 5.2 条件评估流程图

```
流程到达条件节点
       │
       ▼
┌──────────────────┐
│ 获取条件组配置    │
│ (GroupedCondition │
│  ParamTypes)      │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│ 遍历每个条件组    │◄─────┐
└────────┬─────────┘      │
         │                │
         ▼                │
┌──────────────────┐      │
│ 遍历组内每个条件  │◄──┐  │
└────────┬─────────┘   │  │
         │             │  │
         ▼             │  │
┌──────────────────┐   │  │
│ 查找匹配的        │   │  │
│ IConditionJudge   │   │  │
└────────┬─────────┘   │  │
         │             │  │
         ▼             │  │
┌──────────────────┐   │  │
│ 调用 Judge()     │   │  │
│ 评估条件是否满足  │   │  │
└────────┬─────────┘   │  │
         │             │  │
         ▼             │  │
┌──────────────────┐   │  │
│ 合并组内结果      │───┘  │
│ (AND/OR)          │      │
└────────┬─────────┘      │
         │                │
         ▼                │
┌──────────────────┐      │
│ 合并组间结果      │──────┘
│ (AND/OR)          │
└────────┬─────────┘
         │
         ▼
    条件评估结果
    (true/false)
```

## 6. 自定义条件判断器步骤

### Step 1: 定义条件类型枚举

```csharp
public enum ConditionTypeEnum
{
    // ... 现有条件类型
    
    [ConditionType("项目规模",
        FieldName = "ProjectScale",
        FieldType = 2,
        FieldClass = typeof(int),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "ProjectScale",
        ConditionJudgeClass = typeof(ProjectScaleJudge))]
    CONDITION_PROJECT_SCALE = 200,
    
    [ConditionType("客户等级",
        FieldName = "CustomerLevel",
        FieldType = 1,
        FieldClass = typeof(string),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "CustomerLevel",
        ConditionJudgeClass = typeof(CustomerLevelJudge))]
    CONDITION_CUSTOMER_LEVEL = 201
}
```

### Step 2: 实现条件判断器

#### 简单条件判断器

```csharp
/// <summary>
/// 项目规模条件判断器
/// 判断项目规模是否超过指定阈值
/// </summary>
public class ProjectScaleJudge : IConditionJudge
{
    private readonly ILogger<ProjectScaleJudge> _logger;

    public ProjectScaleJudge(ILogger<ProjectScaleJudge> logger)
    {
        _logger = logger;
    }

    public bool Judge(
        string nodeId, 
        BpmnNodeConditionsConfBaseVo conditionsConf, 
        BpmnStartConditionsVo bpmnStartConditionsVo,
        int coundGroup, int index)
    {
        // 从条件配置中获取阈值
        var conditionValue = conditionsConf.Conditions?
            .FirstOrDefault(c => c.Key == nodeId).Value;
            
        if (conditionValue == null || !int.TryParse(conditionValue.ToString(), out var threshold))
        {
            _logger.LogWarning("项目规模条件配置无效，节点：{NodeId}", nodeId);
            return false;
        }

        // 从启动条件中获取实际项目规模
        if (!bpmnStartConditionsVo.Conditions.TryGetValue("ProjectScale", out var scaleObj) 
            || !int.TryParse(scaleObj?.ToString(), out var actualScale))
        {
            _logger.LogWarning("缺少项目规模参数，节点：{NodeId}", nodeId);
            return false;
        }

        // 判断项目规模是否超过阈值
        bool result = actualScale >= threshold;
        
        _logger.LogInformation(
            "项目规模条件判断 - 节点:{NodeId}, 阈值:{Threshold}, 实际值:{Actual}, 结果:{Result}",
            nodeId, threshold, actualScale, result);

        return result;
    }
}
```

#### 复杂条件判断器（多字段比较）

```csharp
/// <summary>
/// 客户等级条件判断器
/// 根据客户等级和订单金额综合判断
/// </summary>
public class CustomerLevelJudge : IConditionJudge
{
    public bool Judge(
        string nodeId, 
        BpmnNodeConditionsConfBaseVo conditionsConf, 
        BpmnStartConditionsVo bpmnStartConditionsVo,
        int coundGroup, int index)
    {
        // 从条件配置获取配置值
        var levelConfig = conditionsConf.Conditions?
            .FirstOrDefault(c => c.Key == nodeId).Value?.ToString();
            
        if (string.IsNullOrEmpty(levelConfig))
            return false;

        // 解析配置（格式：VIP,10000 表示VIP等级且金额>=10000）
        var parts = levelConfig.Split(',');
        string requiredLevel = parts[0];
        decimal minAmount = parts.Length > 1 ? decimal.Parse(parts[1]) : 0;

        // 从启动条件获取实际值
        var actualLevel = bpmnStartConditionsVo.Conditions?
            .TryGetValue("CustomerLevel", out var level) == true ? level?.ToString() : null;
            
        var actualAmount = bpmnStartConditionsVo.Conditions?
            .TryGetValue("OrderAmount", out var amount) == true 
            && decimal.TryParse(amount?.ToString(), out var amt) ? amt : 0m;

        // 综合判断
        return string.Equals(actualLevel, requiredLevel, StringComparison.OrdinalIgnoreCase) 
               && actualAmount >= minAmount;
    }
}
```

### Step 3: 注册到 DI 容器

```csharp
// 在 ServiceRegistration.AntFlowServiceSetUp 中
services.AddSingleton<IConditionJudge, ProjectScaleJudge>();
services.AddSingleton<IConditionJudge, CustomerLevelJudge>();
```

### Step 4: 在流程模板中配置条件

在前端流程设计器中，选择条件分支，配置条件类型为新添加的条件编码（如 200），并设置条件参数。

## 7. 条件配置数据结构

### 7.1 BpmnNodeConditionsConfBaseVo

```csharp
public class BpmnNodeConditionsConfBaseVo
{
    /// <summary>
    /// 条件组与条件参数类型的映射
    /// Key=组号, Value=该组内条件参数类型列表
    /// </summary>
    public IDictionary<int, List<int>> GroupedConditionParamTypes { get; set; }
    
    /// <summary>
    /// 条件组与数值运算符映射
    /// Key=组号, Value=该组内数值运算符列表
    /// </summary>
    public IDictionary<int, List<int>> GroupedNumberOperatorListMap { get; set; }
    
    /// <summary>
    /// 条件组关系（1=AND, 0=OR）
    /// </summary>
    public int? GroupRelation { get; set; }
    
    /// <summary>
    /// 组内条件关系
    /// </summary>
    public IDictionary<int, int> GroupedCondRelations { get; set; }
    
    /// <summary>
    /// 低代码条件配置
    /// </summary>
    public IDictionary<int, IDictionary<string, object>> GroupedLfConditionsMap { get; set; }
}
```

### 7.2 条件组关系说明

| 关系类型 | 值 | 说明 |
|---------|---|------|
| 组间 AND | 1 | 所有条件组都满足才为 true |
| 组间 OR | 0 | 任一组满足即为 true |
| 组内 AND | 1 | 组内所有条件都满足 |
| 组内 OR | 0 | 组内任一条件满足 |

## 8. 低代码条件

AntFlowCore 提供了低代码条件类型（编码 >= 10000），支持在流程设计器中可视化配置条件，无需编写代码：

| 条件类型 | 编码 | 说明 |
|---------|------|------|
| `CONDITION_TYPE_LF_STR_CONDITION` | 10000 | 字符串比较（等于、包含等） |
| `CONDITION_TYPE_LF_NUM_CONDITION` | 10001 | 数值比较（>、<、>=、<=、=） |
| `CONDITION_TYPE_LF_DATE_CONDITION` | 10002 | 日期比较 |
| `CONDITION_TYPE_LF_DATE_TIME_CONDITION` | 10003 | 日期时间比较 |
| `CONDITION_TYPE_LF_COLLECTION_CONDITION` | 10004 | 集合包含判断 |

## 9. 自动节点条件

自动节点（nodeType=9）在流程运行时自动执行条件判断和动作。自定义自动节点条件通过重写 `IFormOperationAdaptor.AutoCondition` 实现：

```csharp
public class MyFlowService : AbstractLowFlowSpyFormOperationAdaptor<MyBusinessVo>
{
    /// <summary>
    /// 自定义自动节点条件评估
    /// 返回非null值覆盖默认逻辑，返回null走默认DB评估
    /// </summary>
    public override bool? AutoCondition(MyBusinessVo vo)
    {
        // 示例：金额大于50000自动通过
        if (vo.LfFields.TryGetValue("amount", out var amountObj) 
            && decimal.TryParse(amountObj?.ToString(), out var amount))
        {
            return amount > 50000;
        }
        return null; // 返回null走默认评估逻辑
    }

    /// <summary>
    /// 自动节点动作执行
    /// </summary>
    public override void AutomaticAction(MyBusinessVo vo, bool? conditionResult)
    {
        if (conditionResult == true)
        {
            // 条件满足时的自定义动作
            _logger.LogInformation("自动节点条件满足，流程自动通过");
        }
    }
}
```

## 10. 调试技巧

```csharp
// 在条件判断器中添加详细日志
_logger.LogInformation("=== 条件判断开始 ===");
_logger.LogInformation("节点ID: {NodeId}", nodeId);
_logger.LogInformation("条件组: {Group}, 索引: {Index}", coundGroup, index);
_logger.LogInformation("条件配置: {Config}", 
    JsonSerializer.Serialize(conditionsConf));
_logger.LogInformation("启动条件: {Conditions}", 
    JsonSerializer.Serialize(bpmnStartConditionsVo.Conditions));
_logger.LogInformation("判断结果: {Result}", result);
```
