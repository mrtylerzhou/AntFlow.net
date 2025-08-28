using AntFlow.Core.Adaptor.NodeTypeCondition;
using AntFlow.Core.Adaptor.NodeTypeCondition.Judge;
using AntFlow.Core.Vo;
using static AntFlow.Core.Constant.StringConstants;

namespace AntFlow.Core.Constant.Enums;

public enum ConditionTypeEnum
{
    CONDITION_THIRD_ACCOUNT_TYPE = 1, // 第三方账户类型
    CONDITION_BIZ_LEAVE_TIME = 2, // 请假时长
    CONDITION_PURCHASE_FEE = 3, // 采购费用
    CONDITION_TYPE_OUT_TOTAL_MONEY = 4, // 外出总金额
    CONDITION_JOB_LEVEL_TYPE = 5, // 职级类型
    CONDITION_PURCHASE_TYPE = 6, // 采购类型
    CONDITION_TYPE_NUMBER_OPERATOR = 7, // 数字操作符
    CONDITION_THIRD_PARK_AREA = 37, // 第三方园区
    CONDITION_TYPE_TOTAL_MONEY = 38, // 总金额


    CONDITION_TEMPLATEMARK = 9999, // 模板标记
    CONDITION_TYPE_LF_STR_CONDITION = 10000, // 低代码字符串条件
    CONDITION_TYPE_LF_NUM_CONDITION = 10001, // 低代码数字条件
    CONDITION_TYPE_LF_DATE_CONDITION = 10002, // 低代码日期条件
    CONDITION_TYPE_LF_DATE_TIME_CONDITION = 10003, // 低代码日期时间条件
    CONDITION_TYPE_LF_COLLECTION_CONDITION = 10004 // 低代码集合条件
}

public class ConditionTypeAttributes

{
    public string Description { get; set; } // 条件描述
    public string FieldName { get; set; } // 字段名称
    public int FieldType { get; set; } // 字段类型：1-字符串，2-数字
    public Type FieldClass { get; set; } // 字段类型
    public Type AdaptorClass { get; set; } // 适配器类
    public Type AlignmentClass { get; set; } // 对齐类
    public string AlignmentFieldName { get; set; } // 对齐字段名称
    public Type ConditionJudgeClass { get; set; } // 条件判断类
}

public static class ConditionTypeEnumExtensions
{
    private static readonly Dictionary<ConditionTypeEnum, ConditionTypeAttributes> _attributes =
        new()
        {
            {
                ConditionTypeEnum.CONDITION_THIRD_ACCOUNT_TYPE,
                new ConditionTypeAttributes
                {
                    Description = "第三方账户类型",
                    FieldName = "AccountType",
                    FieldType = 1,
                    AdaptorClass = typeof(BpmnNodeConditionsAccountTypeAdaptor),
                    FieldClass = typeof(int),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = "AccountType",
                    ConditionJudgeClass = typeof(ThirdAccountJudgeService)
                }
            },
            {
                ConditionTypeEnum.CONDITION_BIZ_LEAVE_TIME,
                new ConditionTypeAttributes
                {
                    Description = "请假时长",
                    FieldName = "LeaveHour",
                    FieldType = 2,
                    FieldClass = typeof(double),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = "LeaveHour",
                    ConditionJudgeClass = typeof(AskLeaveJudge)
                }
            },
            {
                ConditionTypeEnum.CONDITION_PURCHASE_FEE,
                new ConditionTypeAttributes
                {
                    Description = "采购费用",
                    FieldName = "PlanProcurementTotalMoney",
                    FieldType = 2,
                    FieldClass = typeof(double),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = "PlanProcurementTotalMoney",
                    ConditionJudgeClass = typeof(PurchaseTotalMoneyJudge)
                }
            },
            {
                ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR,
                new ConditionTypeAttributes
                {
                    Description = "数字操作符",
                    FieldName = "NumberOperator",
                    FieldType = 2,
                    FieldClass = typeof(int),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = "NumberOperator",
                    ConditionJudgeClass = typeof(NumberOperatorJudgeService)
                }
            },
            {
                //总金额条件
                ConditionTypeEnum.CONDITION_TYPE_TOTAL_MONEY,
                new ConditionTypeAttributes
                {
                    Description = "总金额",
                    FieldName = "TotalMoney",
                    FieldType = 2,
                    FieldClass = typeof(double),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = "TotalMoney",
                    ConditionJudgeClass = typeof(NumberOperatorJudgeService)
                }
            },
            {
                ConditionTypeEnum.CONDITION_TEMPLATEMARK,
                new ConditionTypeAttributes
                {
                    Description = "模板标记",
                    FieldName = "TemplateMarks",
                    FieldType = 1,
                    FieldClass = typeof(string),
                    AdaptorClass = typeof(BpmnTemplateMarkAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = "TemplateMarks",
                    ConditionJudgeClass = typeof(BpmnTemplateMarkJudge)
                }
            },
            {
                ConditionTypeEnum.CONDITION_TYPE_LF_STR_CONDITION,
                new ConditionTypeAttributes
                {
                    Description = "低代码字符串条件",
                    FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    FieldType = 2,
                    FieldClass = typeof(string),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    ConditionJudgeClass = typeof(LFStringConditionJudge)
                }
            },
            {
                ConditionTypeEnum.CONDITION_TYPE_LF_NUM_CONDITION,
                new ConditionTypeAttributes
                {
                    Description = "低代码数字条件",
                    FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    FieldType = 2,
                    FieldClass = typeof(double),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    ConditionJudgeClass = typeof(LFNumberFormatJudge)
                }
            },
            {
                ConditionTypeEnum.CONDITION_TYPE_LF_DATE_CONDITION,
                new ConditionTypeAttributes
                {
                    Description = "低代码日期条件",
                    FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    FieldType = 2,
                    FieldClass = typeof(DateTime),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    ConditionJudgeClass = typeof(LFDateConditionJudge)
                }
            },
            {
                ConditionTypeEnum.CONDITION_TYPE_LF_DATE_TIME_CONDITION,
                new ConditionTypeAttributes
                {
                    Description = "低代码日期时间条件",
                    FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    FieldType = 2,
                    FieldClass = typeof(DateTime),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    ConditionJudgeClass = typeof(AbstractLFDateTimeConditionJudge)
                }
            },
            {
                ConditionTypeEnum.CONDITION_TYPE_LF_COLLECTION_CONDITION,
                new ConditionTypeAttributes
                {
                    Description = "低代码集合条件",
                    FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    FieldType = 1,
                    FieldClass = typeof(string),
                    AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                    AlignmentClass = typeof(BpmnStartConditionsVo),
                    AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                    ConditionJudgeClass = typeof(LFCollectionConditionJudge)
                }
            }
        };

    public static ConditionTypeAttributes GetAttributes(this ConditionTypeEnum conditionType)
    {
        if (_attributes.TryGetValue(conditionType, out ConditionTypeAttributes? attributes))
        {
            return attributes;
        }

        throw new ArgumentException($"Attributes for {conditionType} not found.");
    }

    public static ConditionTypeEnum? GetEnumByCode(int code)
    {
        return Enum.IsDefined(typeof(ConditionTypeEnum), code)
            ? (ConditionTypeEnum?)code
            : null;
    }

    public static ConditionTypeEnum? GetEnumByFieldName(string fieldName)
    {
        return _attributes.FirstOrDefault(kv => kv.Value.FieldName == fieldName).Key;
    }

    public static bool IsLowCodeFlow(this ConditionTypeEnum conditionType)
    {
        List<int>? lowFlowCodes = new()
        {
            (int)ConditionTypeEnum.CONDITION_TYPE_LF_STR_CONDITION,
            (int)ConditionTypeEnum.CONDITION_TYPE_LF_NUM_CONDITION,
            (int)ConditionTypeEnum.CONDITION_TYPE_LF_DATE_CONDITION,
            (int)ConditionTypeEnum.CONDITION_TYPE_LF_DATE_TIME_CONDITION,
            (int)ConditionTypeEnum.CONDITION_TYPE_LF_COLLECTION_CONDITION
        };

        return lowFlowCodes.Contains((int)conditionType);
    }
}