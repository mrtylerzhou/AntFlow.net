namespace AntFlow.Core.Constant.Enums;

public class BpmnConfFlagsEnum
{
    public static readonly BpmnConfFlagsEnum NOTHING = new(0, "不存在");
    public static readonly BpmnConfFlagsEnum HAS_NODE_LABELS = new(0b1, "包含节点标签(任意节点包含标签)");
    public static readonly BpmnConfFlagsEnum HAS_STARTUSER_CHOOSE_MODULES = new(0b10, "是否包含发起人自选模块");
    public static readonly BpmnConfFlagsEnum HAS_DYNAMIC_CONDITIONS = new(0b100, "是否包含动态条件");
    public static readonly BpmnConfFlagsEnum HAS_COPY = new(0b1000, "是否包含抄送");
    public static readonly BpmnConfFlagsEnum HAS_LAST_NODE_COPY = new(0b10000, "最后一个节点是否包含抄送");

    private static readonly List<BpmnConfFlagsEnum> _allFlags = new()
    {
        NOTHING,
        HAS_NODE_LABELS,
        HAS_STARTUSER_CHOOSE_MODULES,
        HAS_DYNAMIC_CONDITIONS,
        HAS_COPY,
        HAS_LAST_NODE_COPY
    };

    private BpmnConfFlagsEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public int Code { get; }
    public string Desc { get; }

    public static IReadOnlyList<BpmnConfFlagsEnum> GetAllFlags()
    {
        return _allFlags;
    }

    /// <summary>
    ///     叠加 flag（位或运算）
    /// </summary>
    public static int BinaryOr(int alreadyFlags, BpmnConfFlagsEnum newFlag)
    {
        return alreadyFlags | newFlag.Code;
    }

    public static int BinaryOr(int alreadyFlags, int newFlag)
    {
        return alreadyFlags | newFlag;
    }

    /// <summary>
    ///     从已有的标识中清除某个标识
    /// </summary>
    public static int BinaryAndNot(int alreadyFlags, BpmnConfFlagsEnum flagToRemove)
    {
        return alreadyFlags & ~flagToRemove.Code;
    }

    /// <summary>
    ///     现有的标识拆分出的枚举值列表
    /// </summary>
    public static List<BpmnConfFlagsEnum> FlagEnumsByCode(int? flags)
    {
        if (!flags.HasValue)
        {
            flags = 0;
        }

        return _allFlags.Where(flag => (flags & flag.Code) != 0).ToList();
    }

    /// <summary>
    ///     判断是否包含某个 flag
    /// </summary>
    public static bool HasFlag(int? alreadyFlags, BpmnConfFlagsEnum flag)
    {
        if (!alreadyFlags.HasValue)
        {
            alreadyFlags = 0;
        }

        return (alreadyFlags & flag.Code) != 0;
    }

    public override string ToString()
    {
        return $"{Desc} ({Code})";
    }
}