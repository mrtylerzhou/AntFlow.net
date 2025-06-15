using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using antflowcore.adaptor.nodetypecondition;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using Antflowcore.Vo;

namespace antflowcore.adaptor;

public class NodeTypeConditionsAdaptor : BpmnNodeAdaptor
{
    private readonly BpmnNodeConditionsConfService _bpmnNodeConditionsConfService;
    private readonly BpmnNodeConditionsParamConfService _bpmnNodeConditionsParamConfService;
    private readonly BpmnConfLfFormdataFieldService _lfFormdataFieldService;

    public NodeTypeConditionsAdaptor(BpmnNodeConditionsConfService bpmnNodeConditionsConfService,
        BpmnNodeConditionsParamConfService bpmnNodeConditionsParamConfService,
        BpmnConfLfFormdataFieldService lfFormdataFieldService)
    {
        _bpmnNodeConditionsConfService = bpmnNodeConditionsConfService;
        _bpmnNodeConditionsParamConfService = bpmnNodeConditionsParamConfService;
        _lfFormdataFieldService = lfFormdataFieldService;
    }

    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodeConditionsConf bpmnNodeConditionsConf =
            _bpmnNodeConditionsConfService.baseRepo.Where(a => a.BpmnNodeId == bpmnNodeVo.Id).First();


        if (bpmnNodeConditionsConf == null)
        {
            return bpmnNodeVo;
        }

        string extJson = bpmnNodeConditionsConf.ExtJson;
        //List<BpmnNodeConditionsConfVueVo> extFields = JsonSerializer.Deserialize<List<BpmnNodeConditionsConfVueVo>>(extJson);
        List<List<BpmnNodeConditionsConfVueVo>>? extFieldsGroup =
            JsonSerializer.Deserialize<List<List<BpmnNodeConditionsConfVueVo>>>(extJson);
        var name2confVueMap = extFieldsGroup
            .SelectMany(list => list)
            .GroupBy(x => $"{x.ColumnDbname}_{x.CondGroup}")
            .ToDictionary(
                g => g.Key,
                g => g.First() // 保留第一个
            );

        BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo = new BpmnNodeConditionsConfBaseVo
        {
            IsDefault = bpmnNodeConditionsConf.IsDefault,
            Sort = bpmnNodeConditionsConf.Sort,
            ExtJson = extJson,
            GroupRelation = bpmnNodeConditionsConf.GroupRelation,
        };

        if (bpmnNodeConditionsConf.IsDefault == 1)
        {
            SetProperty(bpmnNodeVo, bpmnNodeConditionsConfBaseVo);
            bpmnNodeVo.Property.IsDefault = bpmnNodeConditionsConf.IsDefault;
            bpmnNodeVo.Property.Sort = bpmnNodeConditionsConf.Sort;
            return bpmnNodeVo;
        }

        List<BpmnNodeConditionsParamConf> nodeConditionsParamConfs = _bpmnNodeConditionsParamConfService.baseRepo
            .Where(a => a.BpmnNodeConditionsId == bpmnNodeConditionsConf.Id)
            .OrderBy(a => a.CondGroup)
            .ToList();


        if (nodeConditionsParamConfs.Any())
        {
            bpmnNodeConditionsConfBaseVo.ConditionParamTypes = nodeConditionsParamConfs
                .Select(c => c.ConditionParamType)
                .ToList();

            var groupedCondRelations = bpmnNodeConditionsConfBaseVo.GroupedCondRelations;

            foreach (var item in nodeConditionsParamConfs)
            {
                int? condGroup = item.CondGroup;
                int? condRelation = item.CondRelation;

                // 可选：判断 null 抛异常
                if (condGroup == null || condRelation == null)
                {
                    throw new Exception("logic error, please contact the Administrator");
                }

                if (condGroup.HasValue && condRelation.HasValue)
                {
                    groupedCondRelations[condGroup.Value] = condRelation.Value;
                }
            }

            // 分组 + 收集为 List<int>
            bpmnNodeConditionsConfBaseVo.GroupedConditionParamTypes = nodeConditionsParamConfs
                .Where(x => x.CondGroup.HasValue)
                .GroupBy(x => x.CondGroup.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ConditionParamType).ToList()
                );

            IDictionary<int, IDictionary<String, Object>> groupedWrappedValue =
                new Dictionary<int, IDictionary<string, object>>();
            bool isLowCodeFlow = false;
            foreach (BpmnNodeConditionsParamConf nodeConditionsParamConf in nodeConditionsParamConfs)
            {
                IDictionary<String, Object> wrappedValue = null;
                ConditionTypeEnum? conditionTypeEnum =
                    ConditionTypeEnumExtensions.GetEnumByCode(nodeConditionsParamConf.ConditionParamType);
                if (conditionTypeEnum == null)
                {
                    throw new AFBizException(
                        $"can not get ConditionTypeEnum by code:{nodeConditionsParamConf.ConditionParamType}");
                }

                ConditionTypeAttributes conditionTypeAttributes = conditionTypeEnum.Value.GetAttributes();
                int? condGroup = nodeConditionsParamConf.CondGroup;
                string conditionParamJson = nodeConditionsParamConf.ConditionParamJsom;
                int? theOperator = nodeConditionsParamConf.TheOperator;
                String paramKey = nodeConditionsParamConf.ConditionParamName + "_" + nodeConditionsParamConf.CondGroup;
                if (!string.IsNullOrEmpty(conditionParamJson))
                {
                    if (conditionTypeAttributes.FieldType == 1) // List
                    {
                        Type genericTypeToDeserialize =
                            typeof(List<>).MakeGenericType(conditionTypeAttributes.FieldClass);
                        var objects = JsonSerializer.Deserialize(conditionParamJson, genericTypeToDeserialize);
                        if (conditionTypeEnum.Value.IsLowCodeFlow())
                        {
                            String columnDbname = name2confVueMap[paramKey].ColumnDbname;
                            if (wrappedValue == null)
                            {
                                wrappedValue = new SortedDictionary<string, object>();
                            }

                            wrappedValue[columnDbname] = objects;
                            if (groupedWrappedValue.ContainsKey(condGroup.Value))
                            {
                                groupedWrappedValue[condGroup.Value].Add(columnDbname, objects);
                            }
                            else
                            {
                                groupedWrappedValue.Add(condGroup.Value, wrappedValue);
                            }
                        }

                        var field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName,
                            BindingFlags.Public | BindingFlags.Instance);
                        field.SetValue(bpmnNodeConditionsConfBaseVo, wrappedValue ?? objects);
                    }
                    else if (conditionTypeAttributes.FieldType == 2) // Object
                    {
                        object obj = conditionTypeAttributes.FieldClass == typeof(string)
                            ? conditionParamJson
                            : JsonSerializer.Deserialize(conditionParamJson, conditionTypeAttributes.FieldClass);

                        if (conditionTypeEnum.Value.IsLowCodeFlow())
                        {
                            isLowCodeFlow = true;
                            String columnDbname = name2confVueMap[paramKey].ColumnDbname;
                            if (wrappedValue == null)
                            {
                                wrappedValue = new SortedDictionary<string, object>();
                            }

                            wrappedValue.Add(columnDbname, obj);
                            if (groupedWrappedValue.ContainsKey(condGroup.Value))
                            {
                                groupedWrappedValue[condGroup.Value].Add(columnDbname, obj);
                            }
                            else
                            {
                                groupedWrappedValue.Add(condGroup.Value, wrappedValue);
                            }
                        }
                        else
                        {
                            var field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(
                                conditionTypeAttributes.FieldName,
                                BindingFlags.Public | BindingFlags.Instance);
                            field.SetValue(bpmnNodeConditionsConfBaseVo, wrappedValue ?? obj);
                        }
                    }
                }

                // Set response
                var service =
                    (IBpmnNodeConditionsAdaptor)Activator.CreateInstance(conditionTypeAttributes.AdaptorClass);
                service.SetConditionsResps(bpmnNodeConditionsConfBaseVo);
                if (theOperator.HasValue)
                {
                    bpmnNodeConditionsConfBaseVo.NumberOperatorList.Add(theOperator.Value);
                    IDictionary<int, List<int>> groupedNumberOperatorListMap =
                        bpmnNodeConditionsConfBaseVo.GroupedNumberOperatorListMap;
                    if (groupedNumberOperatorListMap.ContainsKey(nodeConditionsParamConf.CondGroup.Value))
                    {
                        groupedNumberOperatorListMap[nodeConditionsParamConf.CondGroup.Value].Add(theOperator.Value);
                    }
                    else
                    {
                        List<int> numberOperatorList = new List<int>();
                        numberOperatorList.Add(theOperator.Value);
                        groupedNumberOperatorListMap[nodeConditionsParamConf.CondGroup.Value] = numberOperatorList;
                    }
                }
            }

            if (isLowCodeFlow)
            {
                bpmnNodeConditionsConfBaseVo.GroupedLfConditionsMap = groupedWrappedValue;
            }
        }

        SetProperty(bpmnNodeVo, bpmnNodeConditionsConfBaseVo);

        List<BpmnNodeConditionsConfVueVo> bpmnNodeConditionsConfVueVos =
            BpmnConfNodePropertyConverter.ToVue3Model(bpmnNodeConditionsConfBaseVo);
        Dictionary<string, BpmnNodeConditionsConfVueVo> voMap =
            bpmnNodeConditionsConfVueVos.ToDictionary(v => v.ColumnDbname, v => v);
        List<BpmnNodeConditionsConfVueVo> extFields = extFieldsGroup.SelectMany(group => group).ToList();
        foreach (BpmnNodeConditionsConfVueVo extField in extFields)
        {
            string columnDbname = extField.ColumnDbname;
            var lowCodeFlow =
                ConditionTypeEnumExtensions.GetEnumByCode(int.Parse(extField.ColumnId)).Value.IsLowCodeFlow();
            if (lowCodeFlow)
            {
                columnDbname = StringConstants.LOWFLOW_CONDITION_CONTAINER_FIELD_NAME;
            }

            if (voMap.Any())
            {
                //由于前端定义的是首字母小写,
                string first = voMap.Keys.First(a => a.Equals(columnDbname, StringComparison.CurrentCultureIgnoreCase));
                var vueVo = voMap.GetValueOrDefault(first);
                if (vueVo == null)
                {
                    throw new AFBizException("Logic error!");
                }

                //extField.FixedDownBoxValue = vueVo.FixedDownBoxValue;
            }
        }

        bpmnNodeVo.Property.IsDefault = bpmnNodeConditionsConf.IsDefault;
        bpmnNodeVo.Property.Sort = bpmnNodeConditionsConf.Sort;
        bpmnNodeVo.Property.GroupRelation =
            ConditionRelationShipEnum.GetValueByCode(bpmnNodeConditionsConf.GroupRelation);
        bpmnNodeVo.Property.ConditionList = extFieldsGroup;
        return bpmnNodeVo;
    }


    private void SetProperty(BpmnNodeVo bpmnNodeVo, BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo)
    {
        bpmnNodeVo.Property = new BpmnNodePropertysVo
        {
            ConditionsConf = bpmnNodeConditionsConfBaseVo
        };
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodePropertysVo bpmnNodePropertysVo = bpmnNodeVo.Property;
        BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo = null;
        if (bpmnNodePropertysVo != null)
        {
            bpmnNodeConditionsConfBaseVo = BpmnConfNodePropertyConverter.FromVue3Model(bpmnNodePropertysVo);
        }
        else
        {
            bpmnNodeConditionsConfBaseVo = new BpmnNodeConditionsConfBaseVo();
        }

        BpmnNodeConditionsConf bpmnNodeConditionsConf = new BpmnNodeConditionsConf
        {
            BpmnNodeId = bpmnNodeVo.Id,
            IsDefault = bpmnNodeConditionsConfBaseVo.IsDefault,
            Sort = bpmnNodeConditionsConfBaseVo.Sort,
            GroupRelation = bpmnNodeConditionsConfBaseVo.GroupRelation,
            ExtJson = bpmnNodeConditionsConfBaseVo.ExtJson,
            CreateTime = DateTime.Now,
            Remark = "",
            CreateUser = SecurityUtils.GetLogInEmpNameSafe()
        };

        _bpmnNodeConditionsConfService.baseRepo.Insert(bpmnNodeConditionsConf);

        // if it is default condition return
        if (bpmnNodeConditionsConfBaseVo.IsDefault == 1)
        {
            return;
        }

        long nodeConditionsId = bpmnNodeConditionsConf.Id;

        if (nodeConditionsId > 0)
        {
            var extJson = bpmnNodeConditionsConfBaseVo.ExtJson;
            List<List<BpmnNodeConditionsConfVueVo>>? extFieldsArray = JsonSerializer.Deserialize<List<List<BpmnNodeConditionsConfVueVo>>>(extJson);
            int index = 0;
            foreach (List<BpmnNodeConditionsConfVueVo> extFields in extFieldsArray)
            {
                index++;
                foreach (var extField in extFields)
                {
                    string columnId = extField.ColumnId;
                    String columnDbname = extField.ColumnDbname;
                    ConditionTypeEnum? conditionTypeEnum =
                        ConditionTypeEnumExtensions.GetEnumByCode(int.Parse(columnId));
                    if (conditionTypeEnum == null)
                    {
                        throw new AFBizException($"Cannot get node ConditionTypeEnum by code: {columnId}");
                    }

                    ConditionTypeAttributes conditionTypeAttributes =
                        ConditionTypeEnumExtensions.GetAttributes(conditionTypeEnum.Value);
                    PropertyInfo? fieldInfo =
                        typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName);
                    if (fieldInfo == null)
                    {
                        throw new AFBizException("fieldInfo is null");
                    }

                    Object conditionParam = null;
                    if(conditionTypeEnum.Value.IsLowCodeFlow())
                    {
                        IDictionary<int,IDictionary<string,object>> groupedLfConditionsMap = bpmnNodeConditionsConfBaseVo.GroupedLfConditionsMap;
                        if (groupedLfConditionsMap.ContainsKey(index))
                        {
                            conditionParam = groupedLfConditionsMap[index];
                        }
                    }
                    else
                    {
                        conditionParam=fieldInfo.GetValue(bpmnNodeConditionsConfBaseVo);
                    }
                    if (conditionParam != null)
                    {
                        if (conditionTypeEnum.Value.IsLowCodeFlow())
                        {
                            IDictionary containerWrapper = (IDictionary)conditionParam;
                            conditionParam = containerWrapper[columnDbname];
                        }

                        string conditionParamJson = conditionParam is string
                            ? conditionParam.ToString()
                            : JsonSerializer.Serialize(conditionParam);


                        if (conditionTypeAttributes.FieldType == 1)
                        {
                            JsonNode? jsonNode = JsonSerializer.Deserialize<JsonNode>(conditionParamJson);
                            if (jsonNode == null || jsonNode is JsonArray { Count: 0 })
                            {
                                continue;
                            }
                        }

                        var numberOperator = extField.OptType;
                        _bpmnNodeConditionsParamConfService.baseRepo.Insert(new BpmnNodeConditionsParamConf
                        {
                            BpmnNodeConditionsId = nodeConditionsId,
                            ConditionParamType = (int)conditionTypeEnum,
                            ConditionParamName = extField.ColumnDbname,
                            ConditionParamJsom = conditionParamJson,
                            TheOperator = numberOperator,
                            CondGroup = extField.CondGroup,
                            CondRelation = ConditionRelationShipEnum.GetCodeByValue(extField.CondRelation),
                            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                            Remark = "",
                            CreateTime = DateTime.Now
                        });

                        //if condition value doest not a collection and doest not a string type,it must have an operator
                        if (conditionTypeAttributes.FieldType == 2 && !(conditionParam is string))
                        {
                            _bpmnNodeConditionsParamConfService.baseRepo.Insert(new BpmnNodeConditionsParamConf
                            {
                                BpmnNodeConditionsId = nodeConditionsId,
                                ConditionParamType = (int)ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR,
                                ConditionParamName = ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR.GetAttributes().FieldName,
                                ConditionParamJsom = JsonSerializer.Serialize(numberOperator),
                                CondGroup = extField.CondGroup,
                                CondRelation = ConditionRelationShipEnum.GetCodeByValue(extField.CondRelation),
                                CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                                CreateTime = DateTime.Now
                            });
                        }

                        long confId = bpmnNodeVo.ConfId;
                        _lfFormdataFieldService.Frsql
                            .Update<BpmnConfLfFormdataField>()
                            .Set(a => a.IsConditionField, 1)
                            .Where(a => a.BpmnConfId == confId && a.FieldId == columnDbname)
                            .ExecuteAffrows();
                    }
                }
            }
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_CONDITIONS);
    }
}