namespace AntFlowCore.Common.constant.enums;

/// <summary>
/// 审批标准枚举（富枚举实现）
/// </summary>
public sealed class ApprovalStandardEnum
{
    private ApprovalStandardEnum(int code, string desc)
    {
        Code = code;
        Desc = desc ?? throw new ArgumentNullException(nameof(desc));
    }

    public int Code { get; }
    public string Desc { get; }

    // 静态只读实例（相当于 Java 的 enum constants）
    public static readonly ApprovalStandardEnum START_USER = new(1, "发起人");
    public static readonly ApprovalStandardEnum APPROVAL = new(2, "被审批人");
    public static readonly ApprovalStandardEnum FROM_PREV_NODE = new(3, "上一节点审批人的");

    // 所有值的集合（用于遍历、查找等）
    private static readonly IReadOnlyList<ApprovalStandardEnum> AllValues = new[]
    {
        START_USER,
        APPROVAL,
        APPROVAL
    }.ToList();

    /// <summary>
    /// 根据 code 获取对应的枚举实例
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>匹配的实例，未找到返回 null</returns>
    public static ApprovalStandardEnum? GetByCode(int? code)
    {
        if (!code.HasValue) return null;
        return AllValues.FirstOrDefault(e => e.Code == code.Value);
    }

   
    public override bool Equals(object? obj)
    {
        return obj is ApprovalStandardEnum other && Code == other.Code;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

  
    public override string ToString()
    {
        return $"{nameof(Code)}: {Code}, {nameof(Desc)}: {Desc}";
    }
}