# 条件规则引擎

## 概述

AntFlowCore 条件规则引擎基于策略模式实现，支持双层 AND/OR 条件组合评估。引擎在流程运行时动态判断条件是否满足，决定流程走向或节点行为。

## 核心架构

```
┌─────────────────────────────────────────────────────────────┐
│                        条件规则引擎                           │
├─────────────────────────────────────────────────────────────┤
│  ┌───────────────┐   ┌───────────────┐   ┌───────────────┐  │
│  │  条件解析器    │   │  条件评估器    │   │  条件执行器    │  │
│  │  Parser       │──▶│  Evaluator    │──▶│  Executor     │  │
│  └───────────────┘   └───────────────┘   └───────────────┘  │
│          │                   │                   │           │
│          ▼                   ▼                   ▼           │
│  ┌───────────────┐   ┌───────────────┐   ┌───────────────┐  │
│  │ ConditionType │   │ JudgeService  │   │ FlowDecision  │  │
│  │  条件类型定义  │   │  判断服务     │   │  流程决策      │  │
│  └───────────────┘   └───────────────┘   └───────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## 条件类型定义

条件类型通过 `ConditionTypeEnum` 枚举定义，支持低代码字段和业务字段两大类：

```csharp
// 源码位置: AntFlowCore.Bpmn/constants/ConditionTypeEnum.cs
public enum ConditionTypeEnum
{
    CONDITION_THIRD_ACCOUNT_TYPE = 1,      // 三方账户类型
    CONDITION_BIZ_LEAVE_TIME = 2,          // 请假时长
    CONDITION_PURCHASE_FEE = 3,            // 采购费用
    CONDITION_TYPE_NUMBER_OPERATOR = 7,    // 数字运算符
    CONDITION_TYPE_TOTAL_MONEY = 38,       // 总金额
    CONDITION_TEMPLATEMARK = 9999,         // 条件模板标识
    CONDITION_TYPE_LF_STR_CONDITION = 10000,      // 无代码字符串条件
    CONDITION_TYPE_LF_NUM_CONDITION = 10001,      // 无代码数字条件
    CONDITION_TYPE_LF_DATE_CONDITION = 10002,     // 无代码日期条件
    CONDITION_TYPE_LF_DATE_TIME_CONDITION = 10003, // 无代码日期时间条件
    CONDITION_TYPE_LF_COLLECTION_CONDITION = 10004 // 无代码集合条件
}
```

## 条件类型分类

### 业务条件类型

| 条件类型 | Code | 字段名 | 说明 |
|---------|------|--------|------|
| 三方账户类型 | 1 | AccountType | 根据账户类型分支 |
| 请假时长 | 2 | LeaveHour | 根据请假时长分支 |
| 采购费用 | 3 | PlanProcurementTotalMoney | 根据采购金额分支 |
| 数字运算符 | 7 | NumberOperator | 数值比较运算 |
| 总金额 | 38 | TotalMoney | 根据总金额分支 |
| 条件模板标识 | 9999 | TemplateMarks | 根据模板标识分支 |

### 低代码条件类型

| 条件类型 | Code | 说明 |
|---------|------|------|
| 字符串条件 | 10000 | 字符串类型字段比较 |
| 数字条件 | 10001 | 数字类型字段比较 |
| 日期条件 | 10002 | 日期类型字段比较 |
| 日期时间条件 | 10003 | 日期时间类型字段比较 |
| 集合条件 | 10004 | 集合类型字段包含判断 |

```csharp
// 源码位置: AntFlowCore.Bpmn/constants/ConditionTypeEnum.cs:242-253
private static readonly HashSet<int> LowCodeFlowCodes = new HashSet<int>
{
    (int)ConditionTypeEnum.CONDITION_TYPE_LF_STR_CONDITION,
    (int)ConditionTypeEnum.CONDITION_TYPE_LF_NUM_CONDITION,
    (int)ConditionTypeEnum.CONDITION_TYPE_LF_DATE_CONDITION,
    (int)ConditionTypeEnum.CONDITION_TYPE_LF_DATE_TIME_CONDITION,
    (int)ConditionTypeEnum.CONDITION_TYPE_LF_COLLECTION_CONDITION
};

public static bool IsLowCodeFlow(this ConditionTypeEnum conditionType) =>
    LowCodeFlowCodes.Contains((int)conditionType);
```

## 条件属性结构

每个条件类型通过 `ConditionTypeAttributes` 定义其元数据：

```csharp
// 源码位置: AntFlowCore.Bpmn/constants/ConditionTypeEnum.cs
public class ConditionTypeAttributes
{
    public string Description { get; set; }        // 条件描述
    public string FieldName { get; set; }          // 条件字段名称
    public int FieldType { get; set; }             // 条件字段类型（1-列表；2-对象）
    public Type FieldClass { get; set; }           // 条件字段类型
    public Type AdaptorClass { get; set; }         // 条件字段扩展适配类型
    public Type AlignmentClass { get; set; }       // 条件比对对象类型
    public string AlignmentFieldName { get; set; } // 条件比对对象字段名称
    public Type ConditionJudgeClass { get; set; }  // 条件判断类类型
}
```

## 判断运算符

条件评估使用 `JudgeOperatorEnum` 定义的运算符：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/JudgeOperatorEnum.cs
public class JudgeOperatorEnum
{
    public static readonly JudgeOperatorEnum GTE = new(1, ">=");
    public static readonly JudgeOperatorEnum GT = new(2, ">");
    public static readonly JudgeOperatorEnum LTE = new(3, "<=");
    public static readonly JudgeOperatorEnum LT = new(4, "<");
    public static readonly JudgeOperatorEnum EQ = new(5, "=");
    public static readonly JudgeOperatorEnum GT1LT2 = new(6, "first<a<second");
    public static readonly JudgeOperatorEnum GTE1LT2 = new(7, "first<=a<second");
    public static readonly JudgeOperatorEnum GET1LE2 = new(8, "first<a<=second");
    public static readonly JudgeOperatorEnum GTE1LTE2 = new(9, "first<=a<=second");
}
```

| 运算符 | Code | 符号 | 说明 |
|--------|------|------|------|
| GTE | 1 | >= | 大于等于 |
| GT | 2 | > | 大于 |
| LTE | 3 | <= | 小于等于 |
| LT | 4 | < | 小于 |
| EQ | 5 | = | 等于 |
| GT1LT2 | 6 | first<a<second | 区间：大于first且小于second |
| GTE1LT2 | 7 | first<=a<second | 区间：大于等于first且小于second |
| GET1LE2 | 8 | first<a<=second | 区间：大于first且小于等于second |
| GTE1LTE2 | 9 | first<=a<=second | 区间：大于等于first且小于等于second |

## 条件关系（AND/OR）

条件组之间支持 AND 和 OR 两种逻辑关系：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/ConditionRelationShipEnum.cs
public class ConditionRelationShipEnum
{
    public static readonly ConditionRelationShipEnum AND = new(0, false, "and");
    public static readonly ConditionRelationShipEnum OR = new(1, true, "or");
}
```

## 双层条件评估

AntFlowCore 支持嵌套的条件组结构，实现复杂的条件组合：

```
条件组1 (OR)
├── 条件A: 金额 >= 10000
├── 条件B: 天数 >= 5
│
条件组2 (AND)
├── 条件C: 类型 = "年假"
├── 条件D: 余额 > 0
```

```json
{
  "conditionList": [
    [
      { "columnId": "38", "operator": 1, "value": "10000" },
      { "columnId": "2", "operator": 1, "value": "5" }
    ],
    [
      { "columnId": "1", "operator": 5, "value": "1" },
      { "columnId": "1", "operator": 5, "value": "1" }
    ]
  ],
  "groupRelation": false
}
```

## 条件配置数据结构

### 前端提交格式（Vue3 Model）

```json
{
  "conditionList": [
    [
      {
        "columnId": "38",
        "operator": 1,
        "value": "5000",
        "condGroup": 1
      }
    ],
    [
      {
        "columnId": "2",
        "operator": 1,
        "value": "3",
        "condGroup": 2
      }
    ]
  ],
  "groupRelation": false,
  "isDefault": 0,
  "sort": 1
}
```

### 后端处理格式

```csharp
// 源码位置: AntFlowCore.Base/vo/BpmnNodeConditionsConfBaseVo.cs
public class BpmnNodeConditionsConfBaseVo
{
    public List<int> ConditionParamTypes { get; set; }
    public IDictionary<int, List<int>> GroupedConditionParamTypes { get; set; }
    public int? IsDefault { get; set; }
    public int? Sort { get; set; }
    public int? GroupRelation { get; set; }
    public string TotalMoney { get; set; }
    public string LeaveHour { get; set; }
    public int? NumberOperator { get; set; }
    public List<int> NumberOperatorList { get; set; }
    public IDictionary<int, List<int>> GroupedNumberOperatorListMap { get; set; }
    public IDictionary<int, int> GroupedCondRelations { get; set; }
    public string ExtJson { get; set; }
    public string OutSideConditionsJson { get; set; }
    public string OutSideConditionsId { get; set; }
    public string OutSideConditionsUrl { get; set; }
    public bool? OutSideMatched { get; set; }
    public List<int> TemplateMarks { get; set; }
    public IDictionary<string, object> LfConditions { get; set; }
    public IDictionary<int, IDictionary<string, object>>? GroupedLfConditionsMap { get; set; }
}
```

## 条件判断服务

每种条件类型对应一个判断服务（`ConditionJudgeClass`）：

```csharp
// 条件类型 → 判断服务映射
CONDITION_THIRD_ACCOUNT_TYPE (1) → ThirdAccountJudgeService
CONDITION_BIZ_LEAVE_TIME (2)     → AskLeaveJudge
CONDITION_PURCHASE_FEE (3)       → PurchaseTotalMoneyJudge
CONDITION_TYPE_NUMBER_OPERATOR (7) → NumberOperatorJudgeService
CONDITION_TYPE_TOTAL_MONEY (38)  → NumberOperatorJudgeService
CONDITION_TEMPLATEMARK (9999)    → BpmnTemplateMarkJudge
CONDITION_TYPE_LF_STR_CONDITION (10000) → LFStringConditionJudge
CONDITION_TYPE_LF_NUM_CONDITION (10001) → LFNumberFormatJudge
CONDITION_TYPE_LF_DATE_CONDITION (10002) → LFDateConditionJudge
CONDITION_TYPE_LF_DATE_TIME_CONDITION (10003) → AbstractLFDateTimeConditionJudge
CONDITION_TYPE_LF_COLLECTION_CONDITION (10004) → LFCollectionConditionJudge
```

## 条件配置转换

前端提交的 Vue3 Model 通过 `BpmnConfNodePropertyConverter.FromVue3Model` 转换为后端格式：

```csharp
// 源码位置: AntFlowCore.Bpmn/util/BpmnConfNodePropertyConverter.cs:16-80
public static BpmnNodeConditionsConfBaseVo FromVue3Model(BpmnNodePropertysVo propertysVo)
{
    int? isDefault = propertysVo.IsDefault;
    var groupedLfConditionsMap = new Dictionary<int, IDictionary<string, object>>();
    
    var result = new BpmnNodeConditionsConfBaseVo
    {
        IsDefault = propertysVo.IsDefault,
        Sort = propertysVo.Sort,
        GroupRelation = ConditionRelationShipEnum.GetCodeByValue(propertysVo.GroupRelation),
    };
    
    var conditionTypes = new List<int>();
    var groupedConditionTypes = new Dictionary<int, int>>();
    
    var groupedNewModels = propertysVo.ConditionList ?? new List<List<BpmnNodeConditionsConfVueVo>>();
    
    int index = 0;
    foreach (var newModels in groupedNewModels)
    {
        index++;
        var currentGroupConditionTypes = new List<int>();
        
        foreach (var newModel in newModels)
        {
            newModel.CondGroup = index;
            string columnId = newModel.ColumnId;
            
            // 处理低代码集合条件
            int columnIdInt = int.Parse(columnId);
            if (strEnumCode == columnIdInt && newModel.Multiple != null && newModel.Multiple.Value)
            {
                columnIdInt = (int)ConditionTypeEnum.CONDITION_TYPE_LF_COLLECTION_CONDITION;
            }
            
            conditionTypes.Add(columnIdInt);
            currentGroupConditionTypes.Add(columnIdInt);
        }
    }
    
    return result;
}
```

## 低代码条件配置

低代码流程使用 `LfConditions` 字段存储表单字段条件：

```json
{
  "lfConditions": {
    "amount": { "operator": 1, "value": 5000 },
    "days": { "operator": 1, "value": 3 }
  },
  "groupedLfConditionsMap": {
    "1": { "amount": { "operator": 1, "value": 5000 } },
    "2": { "days": { "operator": 1, "value": 3 } }
  }
}
```

## 外部条件节点

接入方条件节点支持外部系统通过 API 传入条件结果：

```json
{
  "outSideConditionsId": "ext_condition_001",
  "outSideConditionsUrl": "https://api.example.com/conditions/check",
  "outSideConditionsJson": "{\"result\": true}"
}
```

## 条件审批节点配置

条件审批节点（NODE_TYPE_CONDITION_APPROVE）使用 `autoNodeConf` 存储条件配置：

```json
{
  "nodeType": 12,
  "autoNodeConf": {
    "conditionList": [
      [
        {
          "columnId": "38",
          "operator": 1,
          "value": "10000"
        }
      ]
    ],
    "groupRelation": false
  }
}
```

```csharp
// 源码位置: AntFlowCore.Base/vo/BpmnNodeVo.cs:281-288
public class AutoNodeConfVo
{
    public List<List<BpmnNodeConditionsConfVueVo>>? ConditionList { get; set; }
    public bool? GroupRelation { get; set; }
}
```

## 条件模板标识

条件模板标识（TemplateMark）用于按模板类型分支：

```json
{
  "templateMarks": [1, 2, 3],
  "templateMarksList": [
    { "id": "1", "name": "年假模板" },
    { "id": "2", "name": "病假模板" },
    { "id": "3", "name": "调休模板" }
  ]
}
```

## 条件评估流程

```
┌─────────────────┐
│  1. 获取表单数据  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  2. 解析条件配置  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  3. 遍历条件组   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐     ┌─────────────────┐
│  4. 评估单个条件  │────▶│  获取Judge服务   │
└────────┬────────┘     └─────────────────┘
         │
         ▼
┌─────────────────┐
│  5. 组内AND/OR  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  6. 组间AND/OR  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  7. 返回最终结果  │
└─────────────────┘
```

## 条件节点属性表

条件节点（NODE_TYPE_CONDITIONS）的属性表配置：

```json
{
  "nodeType": 3,
  "property": {
    "conditionsConf": {
      "isDefault": 0,
      "sort": 1,
      "groupRelation": 1,
      "numberOperatorList": [1],
      "groupedNumberOperatorListMap": { "1": [1] },
      "groupedCondRelations": { "1": 1 }
    },
    "conditionList": [[{ "columnId": "38", "operator": 1, "value": "5000" }]],
    "isDefault": 0,
    "sort": 1,
    "groupRelation": true
  }
}
```

## 最佳实践

1. **条件优先级**：将最可能命中的条件放在前面，减少评估次数
2. **默认分支**：条件节点应设置默认分支，处理未命中任何条件的情况
3. **条件简化**：避免过于复杂的条件组合，保持流程清晰可读
4. **使用模板标识**：按流程类型分支时使用模板标识，提高可维护性
5. **低代码优先**：低代码流程优先使用 LF 条件类型，保持配置统一
6. **区间判断**：数值范围判断使用区间运算符（如 GTE1LTE2），避免嵌套条件
