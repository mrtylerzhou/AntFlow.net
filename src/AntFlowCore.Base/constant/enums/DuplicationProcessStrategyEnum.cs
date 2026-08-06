namespace AntFlowCore.Base.constant.enums;

/// <summary>
/// Process duplication strategy enum.
/// Determines how deduplicated assignees are handled at runtime.
/// </summary>
public class DuplicationProcessStrategyEnum
{
    public int Code { get; }
    public string Desc { get; }

    private DuplicationProcessStrategyEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    /// <summary>
    /// 去除: 不生成审批人的审批任务
    /// </summary>
    public static readonly DuplicationProcessStrategyEnum REMOVE = new DuplicationProcessStrategyEnum(1, "去除");

    /// <summary>
    /// 跳过: 会生成审批人的审批任务,但是多个节点出现相同审批人时,
    /// 前置(前去重)、后续(后去重)、相邻(后续)节点会自动同意
    /// </summary>
    public static readonly DuplicationProcessStrategyEnum SKIP = new DuplicationProcessStrategyEnum(2, "跳过");

    public static IEnumerable<DuplicationProcessStrategyEnum> Values
    {
        get
        {
            yield return REMOVE;
            yield return SKIP;
        }
    }

    public static DuplicationProcessStrategyEnum? GetByCode(int? code)
    {
        if (code == null) return null;
        foreach (var v in Values)
        {
            if (v.Code == code.Value) return v;
        }
        return null;
    }
}
