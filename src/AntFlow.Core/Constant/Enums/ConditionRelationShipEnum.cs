namespace AntFlow.Core.Constant.Enums;

public class ConditionRelationShipEnum
{
    public static readonly ConditionRelationShipEnum AND = new(0, false, "and");
    public static readonly ConditionRelationShipEnum OR = new(1, true, "or");

    private static readonly List<ConditionRelationShipEnum> _all = new() { AND, OR };

    private ConditionRelationShipEnum(int code, bool value, string desc)
    {
        Code = code;
        Value = value;
        Desc = desc;
    }

    public int Code { get; }
    public bool Value { get; }
    public string Desc { get; }

    public static int GetCodeByValue(bool? value)
    {
        if (value == null)
        {
            return OR.Code; // Ĭ�� or
        }

        foreach (ConditionRelationShipEnum? item in _all)
        {
            if (item.Value == value)
            {
                return item.Code;
            }
        }

        return OR.Code;
    }

    public static bool GetValueByCode(int? code)
    {
        if (code == null)
        {
            return OR.Value; // Ĭ�� or
        }

        foreach (ConditionRelationShipEnum? item in _all)
        {
            if (item.Code == code)
            {
                return item.Value;
            }
        }

        return OR.Value;
    }
}