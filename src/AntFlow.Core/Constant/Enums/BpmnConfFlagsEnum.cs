namespace AntFlow.Core.Constant.Enums;

public class BpmnConfFlagsEnum
{
    public static readonly BpmnConfFlagsEnum NOTHING = new(0, "������");
    public static readonly BpmnConfFlagsEnum HAS_NODE_LABELS = new(0b1, "�����ڵ��ǩ(����ڵ������ǩ)");
    public static readonly BpmnConfFlagsEnum HAS_STARTUSER_CHOOSE_MODULES = new(0b10, "�Ƿ������������ѡģ��");
    public static readonly BpmnConfFlagsEnum HAS_DYNAMIC_CONDITIONS = new(0b100, "�Ƿ������̬����");
    public static readonly BpmnConfFlagsEnum HAS_COPY = new(0b1000, "�Ƿ��������");
    public static readonly BpmnConfFlagsEnum HAS_LAST_NODE_COPY = new(0b10000, "���һ���ڵ��Ƿ��������");

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
    ///     ���� flag��λ�����㣩
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
    ///     �����еı�ʶ�����ĳ����ʶ
    /// </summary>
    public static int BinaryAndNot(int alreadyFlags, BpmnConfFlagsEnum flagToRemove)
    {
        return alreadyFlags & ~flagToRemove.Code;
    }

    /// <summary>
    ///     ���еı�ʶ��ֳ���ö��ֵ�б�
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
    ///     �ж��Ƿ����ĳ�� flag
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