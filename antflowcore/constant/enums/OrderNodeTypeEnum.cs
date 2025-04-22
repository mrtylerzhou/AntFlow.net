namespace antflowcore.constant.enums;

public enum OrderNodeTypeEnum
{
    TEST_ORDERED_SIGN = 1,  // 示例顺序节点
    OUT_SIDE_NODE = 2,      // 外部系统传入节点
    LOOP_NODE = 3           // 循环节点
}

public static class OrderNodeTypeEnumExtensions
{
    private static readonly Dictionary<OrderNodeTypeEnum, string> Descriptions = new()
    {
        { OrderNodeTypeEnum.TEST_ORDERED_SIGN, "示例顺序节点" },
        { OrderNodeTypeEnum.OUT_SIDE_NODE, "外部系统传入节点" },
        { OrderNodeTypeEnum.LOOP_NODE, "循环节点" }
    };

    /// <summary>
    /// 获取枚举对应的描述信息。
    /// </summary>
    /// <param name="enumValue">OrderNodeTypeEnum 枚举值。</param>
    /// <returns>描述信息字符串。</returns>
    public static string GetDescription(this OrderNodeTypeEnum enumValue)
    {
        return Descriptions.TryGetValue(enumValue, out var description) ? description : "未知节点";
    }

    /// <summary>
    /// 根据 Code 获取枚举实例。
    /// </summary>
    /// <param name="code">节点 Code。</param>
    /// <returns>匹配的 OrderNodeTypeEnum 或 null。</returns>
    public static OrderNodeTypeEnum? GetByCode(int? code)
    {
        if (!code.HasValue) return null;

        foreach (var value in Enum.GetValues<OrderNodeTypeEnum>())
        {
            if ((int)value == code.Value)
            {
                return value;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取所有枚举值。
    /// </summary>
    /// <returns>枚举值列表。</returns>
    public static IEnumerable<OrderNodeTypeEnum> GetAllValues()
    {
        return Enum.GetValues<OrderNodeTypeEnum>();
    }
}