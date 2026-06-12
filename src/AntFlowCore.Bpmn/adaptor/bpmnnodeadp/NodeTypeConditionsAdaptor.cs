using System.Text.Json;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor.nodetypecondition;
using AntFlowCore.Bpmn.constants;
using AntFlowCore.Bpmn.util;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodeTypeConditionsAdaptor : IBpmnNodeAdaptor
{
    public void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodeConfigJson? nodeConfig = JsonConfUtil.ParseNodeConfig(bpmnNodeVo.NodeConfigJson);
        BpmnNodeConditionsConfJson? conditionsConf = nodeConfig?.ConditionsConf;
        if (conditionsConf?.ConditionGroups == null || conditionsConf.ConditionGroups.Count == 0)
        {
            throw new AFBizException("migration error, condition node config is missing in node_config_json");
        }

        FormatFromJson(bpmnNodeVo, conditionsConf);
        ApplyOutsideCondition(bpmnNodeVo, conditionsConf);
    }

    private static void FormatFromJson(BpmnNodeVo bpmnNodeVo, BpmnNodeConditionsConfJson conditionsConf)
    {
        BpmnNodeConditionsConfJson.ConditionGroup firstGroup = conditionsConf.ConditionGroups![0];
        if (firstGroup.IsDefault == 1)
        {
            BpmnNodeConditionsConfBaseVo baseVoDefault = new()
            {
                IsDefault = firstGroup.IsDefault,
                Sort = firstGroup.Sort,
                GroupRelation = firstGroup.GroupRelation,
                ExtJson = firstGroup.ExtJson
            };

            SetProperty(bpmnNodeVo, baseVoDefault);
            bpmnNodeVo.Property.IsDefault = firstGroup.IsDefault;
            bpmnNodeVo.Property.Sort = firstGroup.Sort;
            return;
        }

        if (string.IsNullOrWhiteSpace(firstGroup.ExtJson))
        {
            BpmnNodeConditionsConfBaseVo baseVoSimple = new()
            {
                IsDefault = firstGroup.IsDefault,
                Sort = firstGroup.Sort,
                GroupRelation = firstGroup.GroupRelation
            };

            SetProperty(bpmnNodeVo, baseVoSimple);
            bpmnNodeVo.Property.IsDefault = firstGroup.IsDefault;
            bpmnNodeVo.Property.Sort = firstGroup.Sort;
            bpmnNodeVo.Property.GroupRelation = ConditionRelationShipEnum.GetValueByCode(firstGroup.GroupRelation);
            return;
        }

        List<List<BpmnNodeConditionsConfVueVo>> extFieldsGroup =
            JsonSerializer.Deserialize<List<List<BpmnNodeConditionsConfVueVo>>>(
                firstGroup.ExtJson,
                JsonConfUtil.Options) ?? new List<List<BpmnNodeConditionsConfVueVo>>();

        BpmnNodePropertysVo propertysVo = new()
        {
            IsDefault = firstGroup.IsDefault,
            Sort = firstGroup.Sort,
            GroupRelation = ConditionRelationShipEnum.GetValueByCode(firstGroup.GroupRelation),
            ConditionList = extFieldsGroup
        };

        BpmnNodeConditionsConfBaseVo baseVo = BpmnConfNodePropertyConverter.FromVue3Model(propertysVo);
        IDictionary<int, int> groupedCondRelations = new Dictionary<int, int>();
        IDictionary<int, List<int>> groupedNumberOperatorListMap = new Dictionary<int, List<int>>();
        HashSet<int> processedTypes = new();

        int groupIndex = 0;
        foreach (List<BpmnNodeConditionsConfVueVo> groupConds in extFieldsGroup)
        {
            groupIndex++;
            foreach (BpmnNodeConditionsConfVueVo cond in groupConds)
            {
                int condGroup = cond.CondGroup == 0 ? groupIndex : cond.CondGroup;
                int conditionTypeCode = GetEffectiveConditionTypeCode(cond);
                groupedCondRelations[condGroup] = ConditionRelationShipEnum.GetCodeByValue(cond.CondRelation);

                if (cond.OptType.HasValue)
                {
                    baseVo.NumberOperatorList.Add(cond.OptType.Value);
                    if (!groupedNumberOperatorListMap.TryGetValue(condGroup, out List<int>? operatorList))
                    {
                        operatorList = new List<int>();
                        groupedNumberOperatorListMap[condGroup] = operatorList;
                    }

                    operatorList.Add(cond.OptType.Value);
                }

                if (processedTypes.Add(conditionTypeCode))
                {
                    ConditionTypeEnum? conditionTypeEnum = ConditionTypeEnumExtensions.GetEnumByCode(conditionTypeCode);
                    if (conditionTypeEnum == null)
                    {
                        throw new AFBizException($"can not get ConditionTypeEnum by code:{conditionTypeCode}");
                    }

                    ConditionTypeAttributes attributes = conditionTypeEnum.Value.GetAttributes();
                    if (Activator.CreateInstance(attributes.AdaptorClass) is IBpmnNodeConditionsAdaptor service)
                    {
                        service.SetConditionsResps(baseVo);
                    }
                }
            }
        }

        baseVo.GroupedCondRelations = groupedCondRelations;
        baseVo.GroupedNumberOperatorListMap = groupedNumberOperatorListMap;

        SetProperty(bpmnNodeVo, baseVo);
        bpmnNodeVo.Property.IsDefault = firstGroup.IsDefault;
        bpmnNodeVo.Property.Sort = firstGroup.Sort;
        bpmnNodeVo.Property.GroupRelation = ConditionRelationShipEnum.GetValueByCode(firstGroup.GroupRelation);
        bpmnNodeVo.Property.ConditionList = extFieldsGroup;
    }

    private static int GetEffectiveConditionTypeCode(BpmnNodeConditionsConfVueVo cond)
    {
        if (string.IsNullOrWhiteSpace(cond.ColumnId))
        {
            throw new AFBizException("each and every condition node must have a columnId value");
        }

        int conditionTypeCode = int.Parse(cond.ColumnId);
        if (conditionTypeCode == (int)ConditionTypeEnum.CONDITION_TYPE_LF_STR_CONDITION
            && cond.Multiple == true)
        {
            return (int)ConditionTypeEnum.CONDITION_TYPE_LF_COLLECTION_CONDITION;
        }

        return conditionTypeCode;
    }

    private static void SetProperty(BpmnNodeVo bpmnNodeVo, BpmnNodeConditionsConfBaseVo conditionsConf)
    {
        bpmnNodeVo.Property = new BpmnNodePropertysVo
        {
            ConditionsConf = conditionsConf
        };
    }

    private static void ApplyOutsideCondition(BpmnNodeVo bpmnNodeVo, BpmnNodeConditionsConfJson conditionsConf)
    {
        if (bpmnNodeVo.NodeType != (int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS)
        {
            return;
        }

        if (bpmnNodeVo.Property?.ConditionsConf != null)
        {
            bpmnNodeVo.Property.ConditionsConf.OutSideConditionsId = conditionsConf.OutSideConditionId;
            if (!string.IsNullOrWhiteSpace(bpmnNodeVo.ConditionsUrl)
                && !string.IsNullOrWhiteSpace(conditionsConf.OutSideConditionId))
            {
                bpmnNodeVo.Property.ConditionsConf.OutSideConditionsUrl =
                    bpmnNodeVo.ConditionsUrl + conditionsConf.OutSideConditionId;
            }
        }

        bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_CONDITIONS;
    }

    public void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        // Conditions are persisted in t_bpmn_node.node_config_json.
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(
            BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_CONDITIONS,
            BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_OUT_SIDE_CONDITIONS);
    }
}
