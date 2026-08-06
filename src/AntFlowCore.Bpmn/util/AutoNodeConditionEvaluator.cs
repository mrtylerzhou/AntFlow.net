using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.util;

/// <summary>
/// 自动节点/条件节点默认条件评估器.
/// 参照 Java AbstractFormOperationAdaptor.automaticCondition:
/// 1. 从 DB 加载节点配置 autoNodeConf (loadAutoNodeConf)
/// 2. 直接从 vo.LfFields 评估条件 (evaluateConditions), 不查 DB 业务数据, 不构造 BpmnStartConditionsVo
/// </summary>
public class AutoNodeConditionEvaluator
{
    private readonly ILogger<AutoNodeConditionEvaluator> _logger;
    private readonly IBpmvariableBizService _bpmVariableBizService;
    private readonly IBpmnNodeService _bpmnNodeService;
    private readonly IBpmnConfService _bpmnConfService;

    public AutoNodeConditionEvaluator(
        ILogger<AutoNodeConditionEvaluator> logger,
        IBpmvariableBizService bpmVariableBizService,
        IBpmnNodeService bpmnNodeService,
        IBpmnConfService bpmnConfService)
    {
        _logger = logger;
        _bpmVariableBizService = bpmVariableBizService;
        _bpmnNodeService = bpmnNodeService;
        _bpmnConfService = bpmnConfService;
    }

    /// <summary>
    /// 评估自动节点条件.
    /// 从 vo 取 lfFields (运行时数据,由 BpmnSendMessageAspect 通过 ThreadLocal 传递),
    /// 从 DB 加载节点配置 autoNodeConf (配置数据), 直接评估.
    /// </summary>
    /// <param name="vo">包含 lfFields 和流程信息的 BusinessDataVo</param>
    /// <returns>true=条件满足;false=条件不满足;null=无条件配置(直接执行动作)</returns>
    public bool? Evaluate(BusinessDataVo vo)
    {
        try
        {
            // 仅支持低代码流程
            if (vo.IsLowCodeFlow != 1)
            {
                return false;
            }

            // 从 DB 加载节点配置 autoNodeConf
            BpmnNodeAutoNodeConfJson? autoNodeConf = LoadAutoNodeConf(vo);
            if (autoNodeConf == null || autoNodeConf.ConditionList == null || autoNodeConf.ConditionList.Count == 0)
            {
                return null;
            }

            // 从 vo 取 lfFields (运行时数据,不查 DB)
            Dictionary<string, object>? lfFields = vo.LfFields;
            if (lfFields == null || lfFields.Count == 0)
            {
                return false;
            }

            bool result = EvaluateConditions(autoNodeConf, lfFields);
            _logger.LogInformation("条件评估结果:{}, processNumber={}, taskDefKey={}", result, vo.ProcessNumber, vo.TaskDefKey);
            return result;
        }
        catch (AFBizException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "条件评估异常,返回 null, processNumber={}, taskDefKey={}", vo.ProcessNumber, vo.TaskDefKey);
            return null;
        }
    }

    /// <summary>
    /// 从 DB 加载节点配置 autoNodeConf.
    /// 对应 Java AbstractFormOperationAdaptor.loadAutoNodeConf.
    /// </summary>
    private BpmnNodeAutoNodeConfJson? LoadAutoNodeConf(BusinessDataVo vo)
    {
        string processNumber = vo.ProcessNumber;
        string taskDefKey = vo.TaskDefKey;
        if (string.IsNullOrEmpty(processNumber) || string.IsNullOrEmpty(taskDefKey))
        {
            return null;
        }

        // 通过 formCode 找 BpmnConf
        BpmnConf? bpmnConf = _bpmnConfService._repository
            .Find(a => a.FormCode == vo.FormCode&&a.EffectiveStatus==1)
            .FirstOrDefault();
        if (bpmnConf == null)
        {
            throw new AFBizException("cant not get bpmnconf by formcode:" + vo.FormCode);
        }

        // 通过 elementId 找 nodeId, 再找到 BpmnNode
        NodeElementDto? nodeElementDto = _bpmVariableBizService.GetNodeIdByElementId(processNumber, taskDefKey);
        if (nodeElementDto == null || string.IsNullOrEmpty(nodeElementDto.NodeId))
        {
            _logger.LogError("条件评估失败:无法根据 elementId 找到 nodeId,processNumber={}, elementId={}",
                processNumber, taskDefKey);
            return null;
        }

        BpmnNode? bpmnNode = _bpmnNodeService._repository
            .Find(a => a.ConfId == bpmnConf.Id && a.Id == Convert.ToInt32(nodeElementDto.NodeId))
            .FirstOrDefault();
        if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
        {
            return null;
        }

        BpmnNodeConfigJson? configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
        return configJson?.AutoNodeConf;
    }

    /// <summary>
    /// 评估自动节点条件.
    /// 对应 Java AbstractFormOperationAdaptor.evaluateConditions.
    /// 直接对 BpmnNodeConditionsConfVueVo (前端原始格式) 评估, 不转换为 BpmnNodeConditionsConfBaseVo.
    /// </summary>
    private bool EvaluateConditions(BpmnNodeAutoNodeConfJson autoNodeConf, Dictionary<string, object> formFields)
    {
        List<List<BpmnNodeConditionsConfVueVo>>? conditionList = autoNodeConf.ConditionList;
        if (conditionList == null || conditionList.Count == 0)
        {
            return false;
        }

        bool isOrBetweenGroups = autoNodeConf.GroupRelation ?? false;
        bool overallResult = !isOrBetweenGroups; // AND starts true, OR starts false

        foreach (var group in conditionList)
        {
            if (group == null || group.Count == 0)
            {
                continue;
            }

            List<BpmnNodeConditionsConfVueVo> groupItems = group
                .Where(x => x != null)
                .ToList();

            if (groupItems.Count == 0)
            {
                continue;
            }

            bool groupResult = EvaluateConditionGroup(groupItems, formFields);

            if (isOrBetweenGroups)
            {
                overallResult = overallResult || groupResult;
                if (overallResult) break; // OR: first true wins
            }
            else
            {
                overallResult = overallResult && groupResult;
                if (!overallResult) break; // AND: first false wins
            }
        }
        return overallResult;
    }

    /// <summary>
    /// 评估单个条件组.
    /// </summary>
    private bool EvaluateConditionGroup(List<BpmnNodeConditionsConfVueVo> group, Dictionary<string, object> formFields)
    {
        bool isOrWithinGroup = group[0].CondRelation;
        bool groupResult = !isOrWithinGroup;

        foreach (var item in group)
        {
            bool itemResult = EvaluateSingleCondition(item, formFields);
            if (isOrWithinGroup)
            {
                groupResult = groupResult || itemResult;
                if (groupResult) break;
            }
            else
            {
                groupResult = groupResult && itemResult;
                if (!groupResult) break;
            }
        }
        return groupResult;
    }

    /// <summary>
    /// 评估单个条件项.
    /// </summary>
    private bool EvaluateSingleCondition(BpmnNodeConditionsConfVueVo item, Dictionary<string, object> formFields)
    {
        string fieldName = item.ColumnDbname;
        if (string.IsNullOrEmpty(fieldName))
        {
            return false;
        }

        formFields.TryGetValue(fieldName, out object? formValue);
        string formValueStr = formValue != null ? formValue.ToString() : "";
        string targetValue = item.Zdy1 ?? "";

        string fieldTypeName = item.FieldTypeName ?? "";
        int? optType = item.OptType;

        // Switch type: compare boolean-like values
        if ("switch" == fieldTypeName)
        {
            return "1" == formValueStr == ("1" == targetValue);
        }

        // Select / Radio: equality check
        if ("select" == fieldTypeName || "radio" == fieldTypeName)
        {
            return targetValue == formValueStr;
        }

        // Checkbox: check if form value collection contains the target element
        if ("checkbox" == fieldTypeName)
        {
            if (string.IsNullOrEmpty(formValueStr) || string.IsNullOrEmpty(targetValue))
            {
                return false;
            }
            return formValueStr.Split(',').Contains(targetValue);
        }

        // Numeric / Date / Time comparisons using optType
        if ("number" == fieldTypeName || "date" == fieldTypeName || "time" == fieldTypeName)
        {
            try
            {
                return CompareNumeric(formValueStr, targetValue, optType, item.Zdy2, item.Opt1, item.Opt2);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Default: string equality
        return targetValue == formValueStr;
    }

    /// <summary>
    /// 数值比较: >=, >, <=, &lt;, ==, between.
    /// optType: 1=>=, 2=>, 3=<=, 4=&lt;, 5===, 6~9=between(zdy1 opt1 x opt2 zdy2)
    /// </summary>
    private bool CompareNumeric(string formValueStr, string targetValue, int? optType,
        string zdy2, string opt1, string opt2)
    {
        if (string.IsNullOrEmpty(formValueStr) || string.IsNullOrEmpty(targetValue))
        {
            return false;
        }
        double formVal = double.Parse(formValueStr);
        double target = double.Parse(targetValue);

        if (optType == null) return formVal == target;

        switch (optType.Value)
        {
            case 1: return formVal >= target;
            case 2: return formVal > target;
            case 3: return formVal <= target;
            case 4: return formVal < target;
            case 5: return formVal == target;
            case 6:
            case 7:
            case 8:
            case 9:
                // Between: zdy1 opt1 x opt2 zdy2
                if (string.IsNullOrEmpty(zdy2)) return false;
                double target2 = double.Parse(zdy2);
                bool leftBound = "<" == opt1 ? formVal > target : formVal >= target;
                bool rightBound = "<" == opt2 ? formVal < target2 : formVal <= target2;
                return leftBound && rightBound;
            default:
                return formVal == target;
        }
    }
}
