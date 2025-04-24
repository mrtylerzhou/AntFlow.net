using System.Reflection;
using System.Text.Json;
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
        var bpmnNodeConditionsConf =
            _bpmnNodeConditionsConfService.baseRepo.Where(a => a.BpmnNodeId == bpmnNodeVo.Id).First();
        

        if (bpmnNodeConditionsConf == null)
        {
            return bpmnNodeVo;
        }

        string extJson = bpmnNodeConditionsConf.ExtJson;
        var extFields = JsonSerializer.Deserialize<List<BpmnNodeConditionsConfVueVo>>(extJson);
        var name2confVueMap = extFields.ToDictionary(b => b.ColumnDbname, b => b);

        var bpmnNodeConditionsConfBaseVo = new BpmnNodeConditionsConfBaseVo
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

        var nodeConditionsParamConfs = _bpmnNodeConditionsParamConfService.baseRepo.Where(a=>a.BpmnNodeConditionsId==bpmnNodeConditionsConf.Id).ToList();
        

        if (nodeConditionsParamConfs.Any())
        {
            bpmnNodeConditionsConfBaseVo.ConditionParamTypes = nodeConditionsParamConfs
                .Select(c => c.ConditionParamType)
                .ToList();

            foreach (var nodeConditionsParamConf in nodeConditionsParamConfs)
            {
                var conditionTypeEnum =
                    ConditionTypeEnumExtensions.GetEnumByCode(nodeConditionsParamConf.ConditionParamType);
                ConditionTypeAttributes conditionTypeAttributes = ConditionTypeEnumExtensions.GetAttributes(conditionTypeEnum.Value);
                string conditionParamJson = nodeConditionsParamConf.ConditionParamJsom;

                if (!string.IsNullOrEmpty(conditionParamJson))
                {
                    if (conditionTypeAttributes.FieldType == 1) // List
                    {
                        var objects = JsonSerializer.Deserialize(conditionParamJson, conditionTypeAttributes.FieldClass);
                        var wrappedValue = new Dictionary<string, object>
                        {
                            { name2confVueMap[nodeConditionsParamConf.ConditionParamName].ColumnDbname, objects }
                        };

                        var field = typeof(BpmnNodeConditionsConfBaseVo).GetField(conditionTypeAttributes.FieldName,
                            BindingFlags.Public | BindingFlags.Instance);
                        field.SetValue(bpmnNodeConditionsConfBaseVo, wrappedValue);
                    }
                    else if (conditionTypeAttributes.FieldType == 2) // Object
                    {
                        object obj = string.Equals(conditionTypeAttributes.FieldClass.FullName, typeof(string).FullName)
                            ? conditionParamJson
                            : JsonSerializer.Deserialize(conditionParamJson, conditionTypeAttributes.FieldClass);

                        var wrappedValue = new Dictionary<string, object>
                        {
                            { name2confVueMap[nodeConditionsParamConf.ConditionParamName].ColumnDbname, obj }
                        };

                        var field = typeof(BpmnNodeConditionsConfBaseVo).GetField(conditionTypeAttributes.FieldName,
                            BindingFlags.Public | BindingFlags.Instance);
                        field.SetValue(bpmnNodeConditionsConfBaseVo, wrappedValue);
                    }
                }

                // Set response
                var service = (IBpmnNodeConditionsAdaptor)Activator.CreateInstance(conditionTypeAttributes.AdaptorClass);
                service.SetConditionsResps(bpmnNodeConditionsConfBaseVo);
            }
        }

        SetProperty(bpmnNodeVo, bpmnNodeConditionsConfBaseVo);

        var bpmnNodeConditionsConfVueVos = BpmnConfNodePropertyConverter.ToVue3Model(bpmnNodeConditionsConfBaseVo);
        var voMap = bpmnNodeConditionsConfVueVos.ToDictionary(v => v.ColumnDbname, v => v);

        foreach (var extField in extFields)
        {
            var columnDbname = extField.ColumnDbname;
            var lowCodeFlow =
                ConditionTypeEnumExtensions.IsLowCodeFlow(ConditionTypeEnumExtensions.GetEnumByCode(int.Parse(extField.ColumnId)).Value);
            if (lowCodeFlow)
            {
                columnDbname = StringConstants.LOWFLOW_CONDITION_CONTAINER_FIELD_NAME;
            }

            if (voMap.Any())
            {
                var vueVo = voMap.GetValueOrDefault(columnDbname);
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
        if (bpmnNodePropertysVo != null)
        {
            
        }
        BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo = bpmnNodeVo.Property?.ConditionsConf ?? new BpmnNodeConditionsConfBaseVo();
        BpmnNodeConditionsConf bpmnNodeConditionsConf = new BpmnNodeConditionsConf
        {
            BpmnNodeId = bpmnNodeVo.Id,
            IsDefault = bpmnNodeConditionsConfBaseVo.IsDefault,
            Sort = bpmnNodeConditionsConfBaseVo.Sort,
            ExtJson = bpmnNodeConditionsConfBaseVo.ExtJson,
            CreateTime = DateTime.Now,
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
            var extFields = JsonSerializer.Deserialize<List<BpmnNodeConditionsConfVueVo>>(extJson);
            foreach (var extField in extFields)
            {
                var columnId = extField.ColumnId;
                var conditionTypeEnum = ConditionTypeEnumExtensions.GetEnumByCode(int.Parse(columnId));
                if (conditionTypeEnum == null)
                {
                    throw new AFBizException($"Cannot get node ConditionTypeEnum by code: {columnId}");
                }
                ConditionTypeAttributes conditionTypeAttributes = ConditionTypeEnumExtensions.GetAttributes(conditionTypeEnum.Value);
                FieldInfo? fieldInfo = typeof(BpmnNodeConditionsConfBaseVo).GetField(conditionTypeAttributes.FieldName);
                if (fieldInfo == null)
                {
                    throw new AFBizException("fieldInfo is null");
                }
                var conditionParam =  fieldInfo.GetValue(bpmnNodeConditionsConfBaseVo);
                if (conditionParam != null)
                {
                    var conditionParamJson = conditionParam is string
                        ? conditionParam.ToString()
                        : JsonSerializer.Serialize(conditionParam);

                    if (conditionTypeAttributes.FieldType == 1 && string.IsNullOrEmpty(conditionParamJson))
                    {
                        continue;
                    }

                    _bpmnNodeConditionsParamConfService.baseRepo.Insert(new BpmnNodeConditionsParamConf
                    {
                        BpmnNodeConditionsId = nodeConditionsId,
                        ConditionParamType = (int)conditionTypeEnum,
                        ConditionParamName = extField.ColumnDbname,
                        ConditionParamJsom = conditionParamJson,
                        CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                        CreateTime = DateTime.Now
                    });

                    if (conditionTypeAttributes.FieldType == 2 && !(conditionParam is string))
                    {
                        var numberOperator = bpmnNodeConditionsConfBaseVo.NumberOperator;
                        _bpmnNodeConditionsParamConfService.baseRepo.Insert(new BpmnNodeConditionsParamConf
                        {
                            BpmnNodeConditionsId = nodeConditionsId,
                            ConditionParamType = (int)ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR,
                            ConditionParamName = ConditionTypeEnumExtensions.GetAttributes(ConditionTypeEnum.CONDITION_TYPE_NUMBER_OPERATOR).FieldName,
                            ConditionParamJsom = JsonSerializer.Serialize(numberOperator),
                            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                            CreateTime = DateTime.Now
                        });
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