namespace AntFlow.Core.Constant.Enums;

public class BusinessPartyTypeEnum
{
    public static readonly BusinessPartyTypeEnum BUSINESS_PARTY_TYPE_Embed = new(1, "嵌入式");
    public static readonly BusinessPartyTypeEnum BUSINESS_PARTY_TYPE_ACCESS = new(2, "调入式");

    private static readonly List<BusinessPartyTypeEnum> _values = new()
    {
        BUSINESS_PARTY_TYPE_Embed, BUSINESS_PARTY_TYPE_ACCESS
    };

    private BusinessPartyTypeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public int Code { get; }
    public string Desc { get; }

    public static string GetDescByCode(int code)
    {
        return _values.FirstOrDefault(e => e.Code == code)?.Desc;
    }

    public static BusinessPartyTypeEnum FromCode(int code)
    {
        return _values.FirstOrDefault(e => e.Code == code);
    }

    public static List<BusinessPartyTypeEnum> Values()
    {
        return _values;
    }
}