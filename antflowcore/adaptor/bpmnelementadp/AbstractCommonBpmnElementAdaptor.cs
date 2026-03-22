using antflowcore.constant.enus;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 通用BPMN元素适配器抽象基类
/// 提供通用的元素创建逻辑，子类只需提供少量配置即可
/// </summary>
public abstract class AbstractCommonBpmnElementAdaptor : BpmnElementAdaptor
{
    /// <summary>
    /// 获取集合名称前缀（用于生成elementCodeStr）
    /// </summary>
    protected abstract string CollectionName { get; }

    /// <summary>
    /// 获取支持的节点属性类型
    /// </summary>
    protected abstract NodePropertyEnum SupportedNodeProperty { get; }

    /// <summary>
    /// 是否支持顺序签（默认不支持）
    /// </summary>
    protected virtual bool SupportSignInOrder => false;

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        return DoGetElementVo(property, paramsVo, elementCode, elementId);
    }

    /// <summary>
    /// 子类实现具体的元素获取逻辑
    /// </summary>
    protected abstract BpmnConfCommonElementVo DoGetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId);

    /// <summary>
    /// 生成elementCodeStr
    /// </summary>
    protected string BuildElementCodeStr(int elementCode)
    {
        return $"{CollectionName}{elementCode}";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(SupportedNodeProperty);
    }
}

/// <summary>
/// 单人审批元素适配器基类
/// 用于只有一个审批人的场景（如发起人自己、直属领导、HRBP等）
/// </summary>
public abstract class AbstractCommonBpmnElementSingleAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override BpmnConfCommonElementVo DoGetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        var assigneeVo = paramsVo.Assignee ?? new BpmnNodeParamsAssigneeVo();
        
        var assigneeMap = new Dictionary<string, string>
        {
            { assigneeVo.Assignee, assigneeVo.AssigneeName }
        };

        return BpmnElementUtils.GetSingleElement(
            elementId,
            assigneeVo.ElementName,
            BuildElementCodeStr(elementCode),
            assigneeVo.Assignee,
            assigneeMap);
    }
}

/// <summary>
/// 多人审批元素适配器基类
/// 用于多人审批场景（如指定人员、指定角色等）
/// 支持会签、或签、顺序签
/// </summary>
public abstract class AbstractCommonBpmnElementMultiAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override BpmnConfCommonElementVo DoGetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        var assigneeList = paramsVo.AssigneeList ?? new List<BpmnNodeParamsAssigneeVo>();
        
        string elementName = assigneeList.FirstOrDefault()?.ElementName;
        
        // 创建审批人映射（排除重复的人）
        var assigneeMap = assigneeList
            .Where(o => o.IsDeduplication == 0)
            .ToDictionary(
                assigneeVo => assigneeVo.Assignee,
                assigneeVo => assigneeVo.AssigneeName,
                StringComparer.OrdinalIgnoreCase);

        var assigneeIds = assigneeMap.Keys.ToList();

        // 根据签批类型返回不同的元素
        if (property.SignType == (int)SignTypeEnum.SIGN_TYPE_SIGN)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName, BuildElementCodeStr(elementCode), assigneeIds, assigneeMap);
        }
        else if (SupportSignInOrder && property.SignType == (int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER)
        {
            return BpmnElementUtils.GetMultiplayerSignInOrderElement(elementId, elementName, BuildElementCodeStr(elementCode), assigneeIds, assigneeMap);
        }
        else
        {
            return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName, BuildElementCodeStr(elementCode), assigneeIds, assigneeMap);
        }
    }
}
