namespace antflowcore.constant.enums;

public enum BpmnNodeParamTypeEnum
{
    BPMN_NODE_PARAM_SINGLE = 1,       // 单人
    BPMN_NODE_PARAM_MULTIPLAYER = 2,  // 多人
    BPMN_NODE_PARAM_MULTIPLAYER_SORT = 3 // 多人有序
}

public static class BpmnNodeParamTypeEnumExtensions
{
    // 存储元数据
    private static readonly Dictionary<BpmnNodeParamTypeEnum, string> Descriptions = new()
    {
        { BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE, "单人" },
        { BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER, "多人" },
        { BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER_SORT, "多人有序" }
    };

    // 获取描述
    public static string GetDescription(this BpmnNodeParamTypeEnum paramType)
    {
        return Descriptions.TryGetValue(paramType, out var desc) ? desc : null;
    }

    // 根据 Code 获取枚举
    public static BpmnNodeParamTypeEnum? GetByCode(int code)
    {
        return Enum.IsDefined(typeof(BpmnNodeParamTypeEnum), code)
            ? (BpmnNodeParamTypeEnum?)code
            : null;
    }

    // 获取所有描述
    public static IEnumerable<(BpmnNodeParamTypeEnum EnumValue, string Description)> GetAllDescriptions()
    {
        return Descriptions.Select(kv => (kv.Key, kv.Value));
    }
}