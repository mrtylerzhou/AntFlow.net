using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using antflowcore.adaptor.nodetypecondition;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
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
        List<BpmnNodeConditionsConfVueVo> extFields = JsonSerializer.Deserialize<List<BpmnNodeConditionsConfVueVo>>(extJson);
        var name2confVueMap = extFields.ToDictionary(b => b.ColumnDbname, b => b);

        BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo = new BpmnNodeConditionsConfBaseVo
        {
            IsDefault = bpmnNodeConditionsConf.IsDefault,
            Sort = bpmnNodeConditionsConf.Sort
        };

        if (bpmnNodeConditionsConf.IsDefault == 1)
        {
            SetProperty(bpmnNodeVo, bpmnNodeConditionsConfBaseVo);
            bpmnNodeVo.Property.IsDefault = bpmnNodeConditionsConf.IsDefault;
            bpmnNodeVo.Property.Sort = bpmnNodeConditionsConf.Sort;
            return bpmnNodeVo;
        }

        List<BpmnNodeConditionsParamConf> nodeConditionsParamConfs = _bpmnNodeConditionsParamConfService.baseRepo
            .Where(a=>a.BpmnNodeConditionsId==bpmnNodeConditionsConf.Id).ToList();
        

        if (nodeConditionsParamConfs.Any())
        {
            bpmnNodeConditionsConfBaseVo.ConditionParamTypes = nodeConditionsParamConfs
                .Select(c => c.ConditionParamType)
                .ToList();

            IDictionary<String,Object> wrappedValue=null;
            bool isLowCodeFlow=false;
            foreach (BpmnNodeConditionsParamConf nodeConditionsParamConf in nodeConditionsParamConfs)
            {
                ConditionTypeEnum? conditionTypeEnum =
                    ConditionTypeEnumExtensions.GetEnumByCode(nodeConditionsParamConf.ConditionParamType);
                if(conditionTypeEnum==null){
                    throw  new AFBizException($"can not get ConditionTypeEnum by code:{nodeConditionsParamConf.ConditionParamType}");
                }
                ConditionTypeAttributes conditionTypeAttributes = conditionTypeEnum.Value.GetAttributes();
                string conditionParamJson = nodeConditionsParamConf.ConditionParamJsom;
                int? theOperator = nodeConditionsParamConf.TheOperator;
                if (!string.IsNullOrEmpty(conditionParamJson))
                {
                    if (conditionTypeAttributes.FieldType == 1) // List
                    {
                        Type genericTypeToDeserialize = typeof(List<>).MakeGenericType(conditionTypeAttributes.FieldClass);
                        var objects = JsonSerializer.Deserialize(conditionParamJson, genericTypeToDeserialize);
                        if (conditionTypeEnum.Value.IsLowCodeFlow())
                        {
                            String columnDbname = name2confVueMap[nodeConditionsParamConf.ConditionParamName].ColumnDbname;
                            if(wrappedValue==null){
                                wrappedValue=new SortedDictionary<string,object>();
                            }
                            wrappedValue.Add(columnDbname,objects);
                        }

                        var field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName,
                            BindingFlags.Public | BindingFlags.Instance);
                        field.SetValue(bpmnNodeConditionsConfBaseVo, wrappedValue??objects);
                    }
                    else if (conditionTypeAttributes.FieldType == 2) // Object
                    {
                        object obj = conditionTypeAttributes.FieldClass==typeof(string)
                            ? conditionParamJson
                            : JsonSerializer.Deserialize(conditionParamJson, conditionTypeAttributes.FieldClass);

                        if (conditionTypeEnum.Value.IsLowCodeFlow())
                        {
                            isLowCodeFlow=true;
                            String columnDbname = name2confVueMap[nodeConditionsParamConf.ConditionParamName].ColumnDbname;
                            if(wrappedValue==null){
                                wrappedValue=new SortedDictionary<string, object>();
                            }
                            wrappedValue.Add(columnDbname,obj);
                        }
                        else
                        {
                            var field = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName,
                                BindingFlags.Public | BindingFlags.Instance);
                            field.SetValue(bpmnNodeConditionsConfBaseVo, wrappedValue??obj); 
                        }
                        
                    }
                }

                // Set response
                var service = (IBpmnNodeConditionsAdaptor)Activator.CreateInstance(conditionTypeAttributes.AdaptorClass);
                service.SetConditionsResps(bpmnNodeConditionsConfBaseVo);
                if (theOperator.HasValue)
                {
                    bpmnNodeConditionsConfBaseVo.NumberOperatorList.Add(theOperator.Value);
                }
            }
        }

        SetProperty(bpmnNodeVo, bpmnNodeConditionsConfBaseVo);

        List<BpmnNodeConditionsConfVueVo> bpmnNodeConditionsConfVueVos = BpmnConfNodePropertyConverter.ToVue3Model(bpmnNodeConditionsConfBaseVo);
        Dictionary<string,BpmnNodeConditionsConfVueVo> voMap = bpmnNodeConditionsConfVueVos.ToDictionary(v => v.ColumnDbname, v => v);

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
                string first = voMap.Keys.First(a=>a.Equals(columnDbname,StringComparison.CurrentCultureIgnoreCase));
                var vueVo = voMap.GetValueOrDefault(first);
                if (vueVo == null)
                {
                    throw new AFBizException("Logic error!");
                }

                extField.FixedDownBoxValue = vueVo.FixedDownBoxValue;
            }
        }

        bpmnNodeVo.Property.IsDefault = bpmnNodeConditionsConf.IsDefault;
        bpmnNodeVo.Property.Sort = bpmnNodeConditionsConf.Sort;
        bpmnNodeVo.Property.ConditionList = extFields;

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
            bpmnNodeConditionsConfBaseVo=BpmnConfNodePropertyConverter.FromVue3Model(bpmnNodePropertysVo);
        }
        else
        {
            bpmnNodeConditionsConfBaseVo=new BpmnNodeConditionsConfBaseVo();
        }
        BpmnNodeConditionsConf bpmnNodeConditionsConf = new BpmnNodeConditionsConf
        {
            BpmnNodeId = bpmnNodeVo.Id,
            IsDefault = bpmnNodeConditionsConfBaseVo.IsDefault,
            Sort = bpmnNodeConditionsConfBaseVo.Sort,
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
            List<BpmnNodeConditionsConfVueVo> extFields = JsonSerializer.Deserialize<List<BpmnNodeConditionsConfVueVo>>(extJson);
            foreach (var extField in extFields)
            {
                string columnId = extField.ColumnId;
                String columnDbname = extField.ColumnDbname;
                ConditionTypeEnum? conditionTypeEnum = ConditionTypeEnumExtensions.GetEnumByCode(int.Parse(columnId));
                if (conditionTypeEnum == null)
                {
                    throw new AFBizException($"Cannot get node ConditionTypeEnum by code: {columnId}");
                }
                ConditionTypeAttributes conditionTypeAttributes = ConditionTypeEnumExtensions.GetAttributes(conditionTypeEnum.Value);
                PropertyInfo? fieldInfo = typeof(BpmnNodeConditionsConfBaseVo).GetProperty(conditionTypeAttributes.FieldName);
                if (fieldInfo == null)
                {
                    throw new AFBizException("fieldInfo is null");
                }
                var conditionParam =  fieldInfo.GetValue(bpmnNodeConditionsConfBaseVo);
                if (conditionParam != null)
                {
                    if(conditionTypeEnum.Value.IsLowCodeFlow()){
                        IDictionary containerWrapper = (IDictionary) conditionParam;
                        conditionParam= containerWrapper[columnDbname];
                       
                    }
                    string conditionParamJson = conditionParam is string
                        ? conditionParam.ToString()
                        : JsonSerializer.Serialize(conditionParam);

                    JsonSerializer.Deserialize<JsonNode>(conditionParamJson);
                    if (conditionTypeAttributes.FieldType == 1)
                    {
                        JsonNode? jsonNode = JsonSerializer.Deserialize<JsonNode>(conditionParamJson);
                        if (jsonNode == null||jsonNode is JsonArray { Count: 0 })
                        {
                            continue;
                        }
                    }

                    _bpmnNodeConditionsParamConfService.baseRepo.Insert(new BpmnNodeConditionsParamConf
                    {
                        BpmnNodeConditionsId = nodeConditionsId,
                        ConditionParamType = (int)conditionTypeEnum,
                        ConditionParamName = extField.ColumnDbname,
                        ConditionParamJsom = conditionParamJson,
                        CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                        Remark = "",
                        CreateTime = DateTime.Now
                    });

                    //if condition value doest not a collection and doest not a string type,it must have an operator
                    if (conditionTypeAttributes.FieldType == 2 && !(conditionParam is string))
                    {
                        var numberOperator = bpmnNodeConditionsConfBaseVo.NumberOperator;
                        _bpmnNodeConditionsParamConfService.baseRepo.Insert(new BpmnNodeConditionsParamConf
                        {
                            BpmnNodeConditionsId = nodeConditionsId,
                            ConditionParamType = (int)ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR,
                            ConditionParamName = ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR.GetAttributes().FieldName,
                            ConditionParamJsom = JsonSerializer.Serialize(numberOperator),
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

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_CONDITIONS);
    }
}