using antflowcore.adaptor.nodetypecondition;
using antflowcore.adaptor.nodetypecondition.judge;
using AntFlowCore.Constants;
using antflowcore.service.processor.filter;
using AntFlowCore.Vo;

namespace antflowcore.constant.enus;
using System;
using System.Collections.Generic;
using System.Linq;
using static StringConstants;
public enum ConditionTypeEnum
{
    CONDITION_THIRD_ACCOUNT_TYPE = 1,         // 三方账户
    CONDITION_BIZ_LEAVE_TIME = 2,             // 请假时长
    CONDITION_PURCHASE_FEE = 3,              // 采购费用
    CONDITION_TYPE_OUT_TOTAL_MONEY = 4,            // 支出费用
    CONDITION_JOB_LEVEL_TYPE = 5,             // 职级列表
    CONDITION_PURCHASE_TYPE = 6,             // 采购类型
    CONDITION_TYPE_NUMBER_OPERATOR = 7,           // 数字运算符
    CONDITION_THIRD_PARK_AREA = 37,           // 园区面积
    CONDITION_TYPE_TOTAL_MONEY = 38,              // 总金额
    
    
    CONDITION_TEMPLATEMARK = 9999,          // 条件模板标识
    CONDITION_TYPE_LF_STR_CONDITION = 10000,       // 无代码字符串流程条件
    CONDITION_TYPE_LF_NUM_CONDITION = 10001,       // 无代码数字流程条件
    CONDITION_TYPE_LF_DATE_CONDITION = 10002,      // 无代码日期流程条件
    CONDITION_TYPE_LF_DATE_TIME_CONDITION = 10003,  // 无代码日期时间流程条件
    CONDITION_TYPE_LF_COLLECTION_CONDITION = 10004 // 无代码集合流程条件
}

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

public static class ConditionTypeEnumExtensions
{
    private static readonly Dictionary<ConditionTypeEnum, ConditionTypeAttributes> _attributes = 
        new Dictionary<ConditionTypeEnum, ConditionTypeAttributes>()
    {
        {
            ConditionTypeEnum.CONDITION_THIRD_ACCOUNT_TYPE,
            new ConditionTypeAttributes
            {
                Description = "三方账户",
                FieldName = "AccountType",
                FieldType = 1,
                AdaptorClass = typeof(BpmnNodeConditionsAccountTypeAdaptor),
                FieldClass = typeof(int),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = "AccountType",
                ConditionJudgeClass = typeof(ThirdAccountJudgeService),
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
                ConditionJudgeClass = typeof(PurchaseTotalMoneyJudge),
            }
        },
        {
            ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR,
            new ConditionTypeAttributes
            {
                Description = "数字运算符",
                FieldName = "NumberOperator",
                FieldType = 2,
                FieldClass = typeof(int),
                AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = "NumberOperator",
                ConditionJudgeClass = typeof(NumberOperatorJudgeService),
            }
        },
        {//为了兼容已有
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
                ConditionJudgeClass = typeof(NumberOperatorJudgeService),
            }
        },
        {
            ConditionTypeEnum.CONDITION_TEMPLATEMARK,
            new ConditionTypeAttributes
            {
                Description = "条件模板标识",
                FieldName = "TemplateMarks",
                FieldType = 1,
                FieldClass = typeof(string),
                AdaptorClass = typeof(BpmnTemplateMarkAdaptor),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = "TemplateMarks",
                ConditionJudgeClass = typeof(BpmnTemplateMarkJudge),
            }
        },
        {
            ConditionTypeEnum.CONDITION_TYPE_LF_STR_CONDITION,
            new ConditionTypeAttributes
            {
                Description = "无代码字符串流程条件",
                FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                FieldType = 2,
                FieldClass = typeof(string),
                AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                ConditionJudgeClass = typeof(LFStringConditionJudge),
            }
        },
        {
            ConditionTypeEnum.CONDITION_TYPE_LF_NUM_CONDITION,
            new ConditionTypeAttributes
            {
                Description = "无代码数字流程条件",
                FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                FieldType = 2,
                FieldClass = typeof(string),
                AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                ConditionJudgeClass = typeof(LFNumberFormatJudge),
            }
        },
        {
            ConditionTypeEnum.CONDITION_TYPE_LF_DATE_CONDITION,
            new ConditionTypeAttributes
            {
                Description = "无代码日期流程条件",
                FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                FieldType = 2,
                FieldClass = typeof(string),
                AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                ConditionJudgeClass = typeof(LFDateConditionJudge),
            }
        },
        {
            ConditionTypeEnum.CONDITION_TYPE_LF_DATE_TIME_CONDITION,
            new ConditionTypeAttributes
            {
                Description = "无代码日期时间流程条件",
                FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                FieldType = 2,
                FieldClass = typeof(string),
                AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                ConditionJudgeClass = typeof(AbstractLFDateTimeConditionJudge),
            }
        },
        {
            ConditionTypeEnum.CONDITION_TYPE_LF_COLLECTION_CONDITION,
            new ConditionTypeAttributes
            {
                Description = "无代码集合流程条件",
                FieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                FieldType = 1,
                FieldClass = typeof(string),
                AdaptorClass = typeof(BpmnNodeConditionsEmptyAdaptor),
                AlignmentClass = typeof(BpmnStartConditionsVo),
                AlignmentFieldName = LOWFLOW_CONDITION_CONTAINER_FIELD_NAME,
                ConditionJudgeClass = typeof(LFCollectionConditionJudge),
            }
        }
    };

    public static ConditionTypeAttributes GetAttributes(this ConditionTypeEnum conditionType)
    {
        if (_attributes.TryGetValue(conditionType, out var attributes))
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
        var lowFlowCodes = new List<int>
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
