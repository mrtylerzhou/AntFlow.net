using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.exception;
using antflowcore.util.Extension;
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

        int? isDefault = propertysVo.IsDefault;
        IDictionary<int, IDictionary<string, object>> groupedLfConditionsMap =
            new Dictionary<int, IDictionary<string, object>>();
        BpmnNodeConditionsConfBaseVo result = new BpmnNodeConditionsConfBaseVo
        {
            IsDefault = propertysVo.IsDefault,
            Sort = propertysVo.Sort,
            GroupRelation = ConditionRelationShipEnum.GetCodeByValue(propertysVo.GroupRelation),
        };
        List<int> conditionTypes = new List<int>();
        IDictionary<int, List<int>> groupedConditionTypes = new Dictionary<int, List<int>>();
        int strEnumCode = (int)ConditionTypeEnum.CONDITION_TYPE_LF_STR_CONDITION;
        bool isLowCodeFlow = false;
        List<List<BpmnNodeConditionsConfVueVo>> groupedNewModels = propertysVo.ConditionList;
        if (groupedNewModels.IsEmpty() && (isDefault == 0))
        {
            throw new AFBizException("input nodes is empty");
        }

        int index = 0;
        foreach (List<BpmnNodeConditionsConfVueVo> newModels in groupedNewModels)
        {
            index++;
            List<int> currentGroupConditionTypes = new List<int>();
            Dictionary<String, Object> wrapperResult = new Dictionary<string, object>();
            foreach (BpmnNodeConditionsConfVueVo newModel in newModels)
            {
                newModel.CondGroup = index;
                string columnId = newModel.ColumnId;
                if (string.IsNullOrEmpty(columnId))
                {
                    throw new AFBizException("each and every node must have a columnId value");
                }

                int columnIdInt = int.Parse(columnId);
                if (strEnumCode == columnIdInt)
                {
                    if (newModel.Multiple != null && newModel.Multiple.Value)
                    {
                        columnIdInt = (int)ConditionTypeEnum.CONDITION_TYPE_LF_COLLECTION_CONDITION;
                    }
                }

                ConditionTypeEnum? enumByCode = ConditionTypeEnumExtensions.GetEnumByCode(columnIdInt);
                if (enumByCode == null)
                {
                    throw new AFBizException("node has no condition type");
                }

                ConditionTypeAttributes conditionTypeAttributes = enumByCode.Value.GetAttributes();
                if (enumByCode == null)
                {
                    throw new AFBizException($"columnId of value:{columnId} is not a valid value");
                }

                conditionTypes.Add(columnIdInt);

                string fieldName = conditionTypeAttributes.FieldName;
                string columnDbname = newModel.ColumnDbname;

                if (!fieldName.Equals(columnDbname, StringComparison.CurrentCultureIgnoreCase) &&
                    !string.IsNullOrEmpty(columnDbname))
                {
                    //if it is a lowcode flow condition,its name defined in ConditionTypeEnum is a constant,it is lfConditions,it is always not equals to the name specified
                    if (!StringConstants.LOWFLOW_CONDITION_CONTAINER_FIELD_NAME.Equals(fieldName))
                    {
                        throw new AFBizException($"columnDbname:{columnDbname} is not a valid name");
                    }
                }

                int fieldType = conditionTypeAttributes.FieldType;
                Type fieldCls = conditionTypeAttributes.FieldClass;

                if (fieldType == 1) // list
                {
                    string fixedDownBoxValue = newModel.FixedDownBoxValue;
                    List<BaseKeyValueStruVo> valueStruVoList =
                        JsonSerializer.Deserialize<List<BaseKeyValueStruVo>>(fixedDownBoxValue);
                    var zdy1 = newModel.Zdy1;
                    if (zdy1.StartsWith("[") && zdy1.EndsWith("]"))
                    {
                        zdy1 = zdy1.Substring(1, zdy1.Length - 2);
                    }

                    var keys = zdy1.Split(',');


                    var field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                    IList values = (IList)Activator.CreateInstance(field.PropertyType);
                    foreach (string key in keys)
                    {
                        BaseKeyValueStruVo baseKeyValueStruVo = valueStruVoList.First(v => v.Key == key);
                        if (fieldCls == typeof(string))
                        {
                            values.Add(baseKeyValueStruVo.Key);
                        }
                        else
                        {
                            object parsedObject = JsonSerializer.Deserialize(baseKeyValueStruVo?.Key, fieldCls);
                            values.Add(parsedObject);
                        }
                    }

                    Object valueOrWrapper = null;

                    if (enumByCode.Value.IsLowCodeFlow())
                    {
                        isLowCodeFlow = true;
                        wrapperResult.Add(columnDbname, values);
                        valueOrWrapper = wrapperResult;
                        groupedLfConditionsMap.Add(index, wrapperResult);
                    }
                    else
                    {
                        field.SetValue(result, valueOrWrapper ?? values);
                    }
                }
                else
                {
                    string zdy1 = newModel.Zdy1;
                    String zdy2 = newModel.Zdy2;

                    var field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    var opt1 = newModel.Opt1;
                    int? optType = newModel.OptType;

                    if (optType != null)
                    {
                        JudgeOperatorEnum symbol = JudgeOperatorEnum.GetByOpType(optType.Value);
                        if (symbol == null)
                        {
                            throw new AFBizException($"condition optype of {optType} is undefined!");
                        }

                        var opField = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(StringConstants.NUM_OPERATOR,
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                        opField.SetValue(result, symbol.Code);
                    }


                    if (fieldCls == typeof(string))
                    {
                        Object valueOrWrapper = null;
                        //处理多值first<b<second这种类型
                        if (optType != null && JudgeOperatorEnum.BinaryOperator().Contains(optType.Value))
                        {
                            zdy1 = zdy1 + "," + zdy2; //antflow目前只有一个自定义值,介于之间的提前定义好JudgeOperatorEnum,值用字符串拼接,使用时再分割
                        }

                        if (enumByCode.Value.IsLowCodeFlow())
                        {
                            isLowCodeFlow = true;
                            wrapperResult.Add(columnDbname, zdy1);
                            valueOrWrapper = wrapperResult;
                            groupedLfConditionsMap.Add(index, wrapperResult);
                        }
                        else
                        {
                            field.SetValue(result, valueOrWrapper ?? zdy1);
                        }
                    }
                    else
                    {
                        Object valueOrWrapper = null;
                        Object actualValue = null;
                        Object zdy2Value = null;
                        actualValue = JsonSerializer.Deserialize(zdy2, fieldCls);
                        if (optType != null && JudgeOperatorEnum.BinaryOperator().Contains(optType.Value))
                        {
                            zdy1 = zdy1 + "," + zdy2; //antflow目前只有一个自定义值,介于之间的提前定义好JudgeOperatorEnum,值用字符串拼接,使用时再分割
                        }

                        if (enumByCode.Value.IsLowCodeFlow())
                        {
                            isLowCodeFlow = true;
                            wrapperResult.Add(columnDbname, actualValue);
                            valueOrWrapper = wrapperResult;
                            groupedLfConditionsMap.Add(index, wrapperResult);
                        }
                        else
                        {
                            field.SetValue(result, valueOrWrapper ?? actualValue);
                        }
                    }
                }
            }

            groupedConditionTypes.Add(index, currentGroupConditionTypes);
        }


        if (isLowCodeFlow)
        {
            result.GroupedLfConditionsMap = groupedLfConditionsMap;
        }

        String extJson = JsonSerializer.Serialize(groupedNewModels);
        result.ExtJson = extJson;
        result.ConditionParamTypes = conditionTypes;
        result.GroupedConditionParamTypes = groupedConditionTypes;
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

        List<BpmnNodeConditionsConfVueVo> results = new List<BpmnNodeConditionsConfVueVo>();
        List<int> conditionParamTypes = baseVo.ConditionParamTypes;

        foreach (int conditionParamType in conditionParamTypes)
        {
            BpmnNodeConditionsConfVueVo vueVo = new BpmnNodeConditionsConfVueVo();
            ConditionTypeEnum? enumByCode = ConditionTypeEnumExtensions.GetEnumByCode(conditionParamType);
            if (enumByCode == null)
            {
                throw new AFBizException("node has no condition type");
            }

            ConditionTypeAttributes conditionTypeAttributes =
                ConditionTypeEnumExtensions.GetAttributes(enumByCode.Value);
            vueVo.ColumnDbname = conditionTypeAttributes.FieldName;
            int fieldType = conditionTypeAttributes.FieldType;
            vueVo.ShowName = conditionTypeAttributes.Description;

            if (fieldType == 1)
            {
                var field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName,
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                IDictionary wrappedValues = null;
                List<object> objects = new List<object>();
                if (enumByCode.Value.IsLowCodeFlow())
                {
                    var value = field.GetValue(baseVo);
                    if (value != null && value is IDictionary idc)
                    {
                        wrappedValues = idc;
                        ICollection values = wrappedValues.Values;
                        foreach (object o in values)
                        {
                            objects.Add(o);
                        }
                    }
                }
                else
                {
                    var value = field.GetValue(baseVo);
                    if (value != null)
                    {
                        objects.Add(value);
                    }
                }

                var join = string.Join(",", objects);
                vueVo.Zdy1 = join;

                PropertyInfo? extField = null;
                if (enumByCode.Value.IsLowCodeFlow())
                {
                    extField = field;
                }
                else
                {
                    extField = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(
                        conditionTypeAttributes.FieldName + "List",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                }

                List<BaseIdTranStruVo> extFields = null;
                if (enumByCode.Value.IsLowCodeFlow())
                {
                    String extJson = baseVo.ExtJson;
                    if (!string.IsNullOrEmpty(extJson))
                    {
                        JsonArray? jsonArray = JsonSerializer.Deserialize<JsonArray>(extJson);
                        if (jsonArray != null)
                        {
                            JsonObject? jsonObject = jsonArray[0]?.AsObject();
                            if (jsonObject != null &&
                                jsonObject.TryGetPropertyValue("fixedDownBoxValue", out JsonNode? valueNode))
                            {
                                string? fixedDownBoxValue = valueNode?.ToString();
                                if (!string.IsNullOrEmpty(fixedDownBoxValue))
                                {
                                    vueVo.FixedDownBoxValue = fixedDownBoxValue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    object? value = extField.GetValue(baseVo);
                    if (value != null && value is List<BaseIdTranStruVo> list)
                    {
                        extFields = list;
                    }
                }

                if (extFields == null || !extFields.Any())
                {
                    continue;
                }

                List<BaseKeyValueStruVo> keyValuePairVos = new List<BaseKeyValueStruVo>();
                foreach (BaseIdTranStruVo baseIdTranStruVo in extFields)
                {
                    BaseKeyValueStruVo keyValuePairVo = new BaseKeyValueStruVo();
                    keyValuePairVo.Key = baseIdTranStruVo.Id;
                    keyValuePairVo.Value = baseIdTranStruVo.Name;
                    keyValuePairVos.Add(keyValuePairVo);
                }

                String extJsonx = JsonSerializer.Serialize(keyValuePairVos);
                vueVo.FixedDownBoxValue = extJsonx;
            }
            else
            {
                //todo
            }

            results.Add(vueVo);
        }

        return results;
    }
}