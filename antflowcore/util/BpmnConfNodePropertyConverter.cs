using System.Reflection;
using System.Text.Json;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.exception;
using antflowcore.vo;
using Antflowcore.Vo;

namespace antflowcore.util;

public class BpmnConfNodePropertyConverter
{
    public static BpmnNodeConditionsConfBaseVo FromVue3Model(BpmnNodePropertysVo propertysVo)
    {
        if (propertysVo == null)
        {
            throw new AFBizException("node has no property!");
        }

        var newModels = propertysVo.ConditionList;
        var isDefault = propertysVo.IsDefault;

        if (newModels == null || !newModels.Any() && isDefault == 0)
        {
            throw new AFBizException("input nodes is empty");
        }

        var result = new BpmnNodeConditionsConfBaseVo
        {
            IsDefault = propertysVo.IsDefault,
            Sort = propertysVo.Sort
        };

        var conditionTypes = new List<int>(newModels.Count);
        foreach (var newModel in newModels)
        {
            var columnId = newModel.ColumnId;
            if (string.IsNullOrEmpty(columnId))
            {
                throw new AFBizException("each and every node must have a columnId value");
            }

            int columnIdInt = int.Parse(columnId);
            var enumByCode = ConditionTypeEnumExtensions.GetEnumByCode(columnIdInt);
            if (enumByCode == null)
            {
                throw new AFBizException("node has no condition type");
            }
            ConditionTypeAttributes conditionTypeAttributes = ConditionTypeEnumExtensions.GetAttributes(enumByCode.Value);
            if (enumByCode == null)
            {
                throw new AFBizException($"columnId of value:{columnId} is not a valid value");
            }

            conditionTypes.Add(columnIdInt);
            var fieldName = conditionTypeAttributes.FieldName;
            var columnDbname = newModel.ColumnDbname;

            if (!fieldName.Equals(columnDbname) && !string.IsNullOrEmpty(columnDbname))
            {
                if (!StringConstants.LOWFLOW_CONDITION_CONTAINER_FIELD_NAME.Equals(fieldName))
                {
                    throw new AFBizException($"columnDbname:{columnDbname} is not a valid name");
                }
            }

            var fieldType = conditionTypeAttributes.FieldType;
            var fieldCls = conditionTypeAttributes.FieldClass;

            if (fieldType == 1) // list
            {
                var fixedDownBoxValue = newModel.FixedDownBoxValue;
                var valueStruVoList = JsonSerializer.Deserialize<List<BaseKeyValueStruVo>>(fixedDownBoxValue);
                var zdy1 = newModel.Zdy1;
                var keys = zdy1.Split(',');

                var values = new List<object>(keys.Length);
                foreach (var key in keys)
                {
                    var baseKeyValueStruVo = valueStruVoList.FirstOrDefault(v => v.Key.Equals(key));
                    if (fieldCls == typeof(string))
                    {
                        values.Add(baseKeyValueStruVo?.Key);
                    }
                    else
                    {
                        var parsedObject = JsonSerializer.Deserialize(baseKeyValueStruVo?.Key, fieldCls);
                        values.Add(parsedObject);
                    }
                }

                var field = typeof(BpmnNodeConditionsConfBaseVo).GetField(conditionTypeAttributes.FieldName,
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                field.SetValue(result, values);
            }
            else
            {
                var zdy1 = newModel.Zdy1;
                var field = typeof(BpmnNodeConditionsConfBaseVo).GetField(conditionTypeAttributes.FieldName,
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                var opt1 = newModel.Opt1;
                var optType = newModel.OptType;

                if (optType != null)
                {
                    var symbol = JudgeOperatorEnum.GetByOpType(optType.Value);
                    if (symbol == null)
                    {
                        throw new AFBizException($"condition optype of {optType} is undefined!");
                    }

                    var opField = typeof(BpmnNodeConditionsConfBaseVo).GetField(StringConstants.NUM_OPERATOR,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    opField.SetValue(result, symbol.Code);
                }

                object valueOrWrapper = null;
                if (fieldCls == typeof(string))
                {
                    if (ConditionTypeEnumExtensions.IsLowCodeFlow(enumByCode.Value))
                    {
                        var wrapperResult = new Dictionary<string, object>
                        {
                            { fieldName, zdy1 }
                        };
                        valueOrWrapper = wrapperResult;
                    }
                    else
                    {
                        valueOrWrapper = zdy1;
                    }
                }
                else
                {
                    object actualValue = null;
                    if (enumByCode == ConditionTypeEnum.CONDITION_TYPE_LF_DATE_CONDITION)
                    {
                        actualValue = DateUtil.ParseStandard(zdy1);
                    }
                    else if (enumByCode == ConditionTypeEnum.CONDITION_TYPE_LF_DATE_TIME_CONDITION)
                    {
                        actualValue = DateUtil.ParseStandard(zdy1);
                    }
                    else
                    {
                        actualValue = JsonSerializer.Deserialize(zdy1, fieldCls);
                    }

                    if (ConditionTypeEnumExtensions.IsLowCodeFlow(enumByCode.Value))
                    {
                        var wrapperResult = new Dictionary<string, object>
                        {
                            { fieldName, actualValue }
                        };
                        valueOrWrapper = wrapperResult;
                    }
                    else
                    {
                        valueOrWrapper = actualValue;
                    }
                }

                field.SetValue(result, valueOrWrapper ?? field.GetValue(result));
            }
        }

        newModels.ForEach(a => a.FixedDownBoxValue = null);
        var extJson = JsonSerializer.Serialize(newModels);
        result.ExtJson = extJson;
        result.ConditionParamTypes = conditionTypes;

        return result;
    }

    public static List<BpmnNodeConditionsConfVueVo> ToVue3Model(BpmnNodeConditionsConfBaseVo baseVo)
    {
        if (baseVo == null)
        {
            throw new AFBizException("baseVo to convert is null");
        }

        if (baseVo.IsDefault == 1)
        {
            return new List<BpmnNodeConditionsConfVueVo>();
        }

        var results = new List<BpmnNodeConditionsConfVueVo>();
        var conditionParamTypes = baseVo.ConditionParamTypes;

        foreach (var conditionParamType in conditionParamTypes)
        {
            var vueVo = new BpmnNodeConditionsConfVueVo();
            var enumByCode = ConditionTypeEnumExtensions.GetEnumByCode(conditionParamType);
            if (enumByCode == null)
            {
                throw new AFBizException("node has no condition type");
            }
            ConditionTypeAttributes conditionTypeAttributes = ConditionTypeEnumExtensions.GetAttributes(enumByCode.Value);
            vueVo.ColumnDbname = conditionTypeAttributes.FieldName;
            var fieldType = conditionTypeAttributes.FieldType;
            vueVo.ShowName = conditionTypeAttributes.Description;

            if (fieldType == 1)
            {
                var field = typeof(BpmnNodeConditionsConfBaseVo).GetField(conditionTypeAttributes.FieldName,
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                var objects = field.GetValue(baseVo) as IList<object>;
                var join = string.Join(",", objects);
                vueVo.Zdy1 = join;

                var extField = typeof(BpmnNodeConditionsConfBaseVo).GetField(conditionTypeAttributes.FieldName + "List",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                var extFields = extField.GetValue(baseVo) as List<BaseIdTranStruVo>;

                if (extFields == null || !extFields.Any())
                {
                    continue;
                }

                var keyValuePairVos = new List<BaseKeyValueStruVo>();
                foreach (var baseIdTranStruVo in extFields)
                {
                    var keyValuePairVo = new BaseKeyValueStruVo
                    {
                        Key = baseIdTranStruVo.Id,
                        Value = baseIdTranStruVo.Name
                    };
                    keyValuePairVos.Add(keyValuePairVo);
                }

                var extJson = JsonSerializer.Serialize(keyValuePairVos);
                vueVo.FixedDownBoxValue = extJson;
            }
            else
            {
                // todo
            }

            results.Add(vueVo);
        }

        return results;
    }
}