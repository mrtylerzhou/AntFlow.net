using AntFlowCore.Bpmn.adaptor.nodetypecondition;
using AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;
using AntFlowCore.Core.util;
using AntFlowCore.Vo;

namespace AntFlowCore.Core.constant.enus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static StringConstants;

public class ConditionTypeAttributes
{
    public string Description { get; set; }       // 条件描述
    public string FieldName { get; set; }         // 条件字段名称
    public int FieldType { get; set; }            // 条件字段类型（1-列表；2-对象）
    public Type FieldClass { get; set; }          // 条件字段类型
    public Type AdaptorClass { get; set; }      // 条件字段扩展适配类型
    public Type AlignmentClass { get; set; }      // 条件比对对象类型
    public string AlignmentFieldName { get; set; }// 条件比对对象字段名称
    public Type ConditionJudgeClass { get; set; }  // 条件判断类类型
}

[AttributeUsage(AttributeTargets.Field)]
public class ConditionTypeAttribute : Attribute
{
    /// <summary>
    /// 条件描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 条件字段名称
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// 条件字段类型（1-列表；2-对象）
    /// </summary>
    public int FieldType { get; set; }
    /// <summary>
    /// 条件字段类型
    /// </summary>
    public Type FieldClass { get; set; }
    /// <summary>
    /// 条件字段扩展适配类型
    /// </summary>
    public Type AdaptorClass { get; set; }
    /// <summary>
    /// 条件比对对象类型
    /// </summary>
    public Type AlignmentClass { get; set; }
    /// <summary>
    /// 条件比对对象字段名称
    /// </summary>
    public string AlignmentFieldName { get; set; }
    /// <summary>
    /// 条件判断类类型
    /// </summary>
    public Type ConditionJudgeClass { get; set; }

    public ConditionTypeAttribute(string description)
    {
        Description = description;
    }
}

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

    [ConditionType("采购费用",
        FieldName = "PlanProcurementTotalMoney",
        FieldType = 2,
        FieldClass = typeof(double),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "PlanProcurementTotalMoney",
        ConditionJudgeClass = typeof(PurchaseTotalMoneyJudge))]
    CONDITION_PURCHASE_FEE = 3,



    [ConditionType("数字运算符",
        FieldName = "NumberOperator",
        FieldType = 2,
        FieldClass = typeof(int),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "NumberOperator",
        ConditionJudgeClass = typeof(NumberOperatorJudgeService))]
    CONDITION_TYPE_NUMBER_OPERATOR = 7,



    [ConditionType("总金额",
        FieldName = "TotalMoney",
        FieldType = 2,
        FieldClass = typeof(double),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "TotalMoney",
        ConditionJudgeClass = typeof(NumberOperatorJudgeService))]
    CONDITION_TYPE_TOTAL_MONEY = 38,

    [ConditionType("条件模板标识",
        FieldName = "TemplateMarks",
        FieldType = 1,
        FieldClass = typeof(string),
        AdaptorClass = typeof(BpmnTemplateMarkAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = "TemplateMarks",
        ConditionJudgeClass = typeof(BpmnTemplateMarkJudge))]
    CONDITION_TEMPLATEMARK = 9999,

    [ConditionType("无代码字符串流程条件",
        FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        FieldType = 2,
        FieldClass = typeof(string),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        ConditionJudgeClass = typeof(LFStringConditionJudge))]
    CONDITION_TYPE_LF_STR_CONDITION = 10000,

    [ConditionType("无代码数字流程条件",
        FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        FieldType = 2,
        FieldClass = typeof(double),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        ConditionJudgeClass = typeof(LFNumberFormatJudge))]
    CONDITION_TYPE_LF_NUM_CONDITION = 10001,

    [ConditionType("无代码日期流程条件",
        FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        FieldType = 2,
        FieldClass = typeof(DateTime),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        ConditionJudgeClass = typeof(LFDateConditionJudge))]
    CONDITION_TYPE_LF_DATE_CONDITION = 10002,

    [ConditionType("无代码日期时间流程条件",
        FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        FieldType = 2,
        FieldClass = typeof(DateTime),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        ConditionJudgeClass = typeof(AbstractLFDateTimeConditionJudge))]
    CONDITION_TYPE_LF_DATE_TIME_CONDITION = 10003,

    [ConditionType("无代码集合流程条件",
        FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        FieldType = 1,
        FieldClass = typeof(string),
        AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
        AlignmentClass = typeof(BpmnStartConditionsVo),
        AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
        ConditionJudgeClass = typeof(LFCollectionConditionJudge))]
    CONDITION_TYPE_LF_COLLECTION_CONDITION = 10004
}

public static class ConditionTypeEnumExtensions
{
    private static readonly Lazy<Dictionary<int, ConditionTypeAttributes>> _attributesCache =
        new Lazy<Dictionary<int, ConditionTypeAttributes>>(InitializeAttributes);

    private static Dictionary<int, ConditionTypeAttributes> InitializeAttributes()
    {
        return Enum.GetValues<ConditionTypeEnum>()
            .ToDictionary(
                e => (int)e,
                e => CreateAttributesFromEnum(e)
            );
    }

    private static ConditionTypeAttributes CreateAttributesFromEnum(ConditionTypeEnum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = fieldInfo?.GetCustomAttribute<ConditionTypeAttribute>();

        if (attribute == null)
        {
            return new ConditionTypeAttributes
            {
                Description = enumValue.ToString()
            };
        }

        return new ConditionTypeAttributes
        {
            Description = attribute.Description,
            FieldName = attribute.FieldName,
            FieldType = attribute.FieldType,
            FieldClass = attribute.FieldClass,
            AdaptorClass = attribute.AdaptorClass,
            AlignmentClass = attribute.AlignmentClass,
            AlignmentFieldName = attribute.AlignmentFieldName,
            ConditionJudgeClass = attribute.ConditionJudgeClass
        };
    }

    public static ConditionTypeAttributes GetAttributes(this ConditionTypeEnum conditionType)
    {
        var cache = _attributesCache.Value;
        return cache.TryGetValue((int)conditionType, out var attributes)
            ? attributes
            : throw new ArgumentException($"Attributes for {conditionType} not found.");
    }

    public static ConditionTypeEnum? GetEnumByCode(int code) =>
        Enum.IsDefined(typeof(ConditionTypeEnum), code) ? (ConditionTypeEnum?)code : null;

    public static ConditionTypeEnum? GetEnumByFieldName(string fieldName)
    {
        return _attributesCache.Value
            .FirstOrDefault(kv => kv.Value.FieldName == fieldName)
            .Key is int key
            ? (ConditionTypeEnum)key
            : null;
    }

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
}
