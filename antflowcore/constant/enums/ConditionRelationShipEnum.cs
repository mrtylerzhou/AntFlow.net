namespace antflowcore.constant.enums;

public class ConditionRelationShipEnum
{
    public int Code { get; }
    public bool Value { get; }
    public string Desc { get; }

    private ConditionRelationShipEnum(int code, bool value, string desc)
    {
        Code = code;
        Value = value;
        Desc = desc;
    }

    public static readonly ConditionRelationShipEnum AND = new ConditionRelationShipEnum(0, false, "and");
    public static readonly ConditionRelationShipEnum OR = new ConditionRelationShipEnum(1, true, "or");

    private static readonly List<ConditionRelationShipEnum> _all = new List<ConditionRelationShipEnum> { AND, OR };

    public static int GetCodeByValue(bool? value)
    {
        if (value == null) return OR.Code; // 默认 or

        foreach (var item in _all)
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
        if (code == null) return OR.Value; // 默认 or

        foreach (var item in _all)
        {
            if (item.Code == code)
            {
                return item.Value;
            }
        }
        return OR.Value;
    }
}