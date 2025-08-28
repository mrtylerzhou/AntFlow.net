using AntFlow.Core.Adaptor.NodeTypeCondition;
using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Adaptor;

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
        Dictionary<string, BpmnNodeConditionsConfVueVo>? name2confVueMap = extFieldsGroup
            .SelectMany(list => list)
            .GroupBy(x => $"{x.ColumnDbname}_{x.CondGroup}")
            .ToDictionary(
                g => g.Key,
                g => g.First() // 取第一个
            );

        BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo = new()
        {
            IsDefault = bpmnNodeConditionsConf.IsDefault,
            Sort = bpmnNodeConditionsConf.Sort,
            ExtJson = extJson,
            GroupRelation = bpmnNodeConditionsConf.GroupRelation
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

            IDictionary<int, int>? groupedCondRelations = bpmnNodeConditionsConfBaseVo.GroupedCondRelations;

            foreach (BpmnNodeConditionsParamConf? item in nodeConditionsParamConfs)
            {
                int? condGroup = item.CondGroup;
                int? condRelation = item.CondRelation;

                // 可选参数判断 null 异常
                if (condGroup == null || condRelation == null)
                {
                    throw new System.Exception("logic error, please contact the Administrator");
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

            IDictionary<int, IDictionary<string, object>> groupedWrappedValue =
                new Dictionary<int, IDictionary<string, object>>();
            bool isLowCodeFlow = false;
            foreach (BpmnNodeConditionsParamConf nodeConditionsParamConf in nodeConditionsParamConfs)
            {
                IDictionary<string, object> wrappedValue = null;
                ConditionTypeEnum? conditionTypeEnum =
                    ConditionTypeEnumExtensions.GetEnumByCode(nodeConditionsParamConf.ConditionParamType);
                if (conditionTypeEnum == null)
                {
                    throw new AntFlowException.AFBizException(
                        $"can not get ConditionTypeEnum by code:{nodeConditionsParamConf.ConditionParamType}");
                }

                ConditionTypeAttributes conditionTypeAttributes = conditionTypeEnum.Value.GetAttributes();
                int? condGroup = nodeConditionsParamConf.CondGroup;
                string conditionParamJson = nodeConditionsParamConf.ConditionParamJsom;
                int? theOperator = nodeConditionsParamConf.TheOperator;
                string paramKey = nodeConditionsParamConf.ConditionParamName + "_" + nodeConditionsParamConf.CondGroup;
                if (!string.IsNullOrEmpty(conditionParamJson))
                {
                    if (conditionTypeAttributes.FieldType == 1) // List
                    {
                        Type genericTypeToDeserialize =
                            typeof(List<>).MakeGenericType(conditionTypeAttributes.FieldClass);
                        object? objects = JsonSerializer.Deserialize(conditionParamJson, genericTypeToDeserialize);
                        if (conditionTypeEnum.Value.IsLowCodeFlow())
                        {
                            string columnDbname = name2confVueMap[paramKey].ColumnDbname;
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

                        PropertyInfo? field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(
                            conditionTypeAttributes.FieldName,
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
                            string columnDbname = name2confVueMap[paramKey].ColumnDbname;
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
                            PropertyInfo? field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(
                                conditionTypeAttributes.FieldName,
                                BindingFlags.Public | BindingFlags.Instance);
                            field.SetValue(bpmnNodeConditionsConfBaseVo, wrappedValue ?? obj);
                        }
                    }
                }

                // Set response
                IBpmnNodeConditionsAdaptor? service =
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
                        List<int> numberOperatorList = new();
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
            bpmnNodeConditionsConfVueVos.GroupBy(a => a.ColumnDbname).ToDictionary(a => a.Key, a => a.First());
        List<BpmnNodeConditionsConfVueVo> extFields = extFieldsGroup.SelectMany(group => group).ToList();
        foreach (BpmnNodeConditionsConfVueVo extField in extFields)
        {
            string columnDbname = extField.ColumnDbname;
            bool lowCodeFlow =
                ConditionTypeEnumExtensions.GetEnumByCode(int.Parse(extField.ColumnId)).Value.IsLowCodeFlow();
            if (lowCodeFlow)
            {
                columnDbname = StringConstants.LOWFLOW_CONDITION_CONTAINER_FIELD_NAME;
            }

            if (voMap.Any())
            {
                //检查前端对象是否包含该字段？
                string first = voMap.Keys.First(a => a.Equals(columnDbname, StringComparison.CurrentCultureIgnoreCase));
                BpmnNodeConditionsConfVueVo? vueVo = voMap.GetValueOrDefault(first);
                if (vueVo == null)
                {
                    throw new AntFlowException.AFBizException("Logic error!");
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
        bpmnNodeVo.Property = new BpmnNodePropertysVo { ConditionsConf = bpmnNodeConditionsConfBaseVo };
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

        BpmnNodeConditionsConf bpmnNodeConditionsConf = new()
        {
            BpmnNodeId = bpmnNodeVo.Id,
            IsDefault = bpmnNodeConditionsConfBaseVo.IsDefault,
            Sort = bpmnNodeConditionsConfBaseVo.Sort,
            GroupRelation = bpmnNodeConditionsConfBaseVo.GroupRelation,
            ExtJson = bpmnNodeConditionsConfBaseVo.ExtJson,
            CreateTime = DateTime.Now,
            Remark = "",
            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
            TenantId = MultiTenantUtil.GetCurrentTenantId()
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
            string? extJson = bpmnNodeConditionsConfBaseVo.ExtJson;
            List<List<BpmnNodeConditionsConfVueVo>>? extFieldsArray =
                JsonSerializer.Deserialize<List<List<BpmnNodeConditionsConfVueVo>>>(extJson);
            int index = 0;
            foreach (List<BpmnNodeConditionsConfVueVo> extFields in extFieldsArray)
            {
                index++;
                foreach (BpmnNodeConditionsConfVueVo? extField in extFields)
                {
                    string columnId = extField.ColumnId;
                    string columnDbname = extField.ColumnDbname;
                    ConditionTypeEnum? conditionTypeEnum =
                        ConditionTypeEnumExtensions.GetEnumByCode(int.Parse(columnId));
                    if (conditionTypeEnum == null)
                    {
                        throw new AntFlowException.AFBizException(
                            $"Cannot get node ConditionTypeEnum by code: {columnId}");
                    }

                    ConditionTypeAttributes conditionTypeAttributes =
                        conditionTypeEnum.Value.GetAttributes();
                    PropertyInfo? fieldInfo =
                        typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName);
                    if (fieldInfo == null)
                    {
                        throw new AntFlowException.AFBizException("fieldInfo is null");
                    }

                    object conditionParam = null;
                    if (conditionTypeEnum.Value.IsLowCodeFlow())
                    {
                        IDictionary<int, IDictionary<string, object>> groupedLfConditionsMap =
                            bpmnNodeConditionsConfBaseVo.GroupedLfConditionsMap;
                        if (groupedLfConditionsMap.ContainsKey(index))
                        {
                            conditionParam = groupedLfConditionsMap[index];
                        }
                    }
                    else
                    {
                        conditionParam = fieldInfo.GetValue(bpmnNodeConditionsConfBaseVo);
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

                        int? numberOperator = extField.OptType;
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
                            CreateTime = DateTime.Now,
                            TenantId = MultiTenantUtil.GetCurrentTenantId()
                        });

                        //if condition value doest not a collection and doest not a string type,it must have an operator
                        if (conditionTypeAttributes.FieldType == 2 && !(conditionParam is string))
                        {
                            _bpmnNodeConditionsParamConfService.baseRepo.Insert(new BpmnNodeConditionsParamConf
                            {
                                BpmnNodeConditionsId = nodeConditionsId,
                                ConditionParamType = (int)ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR,
                                ConditionParamName =
                                    ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR.GetAttributes().FieldName,
                                ConditionParamJsom = JsonSerializer.Serialize(numberOperator),
                                CondGroup = extField.CondGroup,
                                CondRelation = ConditionRelationShipEnum.GetCodeByValue(extField.CondRelation),
                                CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                                CreateTime = DateTime.Now,
                                TenantId = MultiTenantUtil.GetCurrentTenantId()
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