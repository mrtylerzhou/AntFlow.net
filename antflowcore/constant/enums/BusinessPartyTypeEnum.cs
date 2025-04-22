namespace antflowcore.constant.enus;

using System.Collections.Generic;
using System.Linq;

public class BusinessPartyTypeEnum
{
    public int Code { get; }
    public string Desc { get; }

    private BusinessPartyTypeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public static readonly BusinessPartyTypeEnum BUSINESS_PARTY_TYPE_Embed = new(1, "嵌入式");
    public static readonly BusinessPartyTypeEnum BUSINESS_PARTY_TYPE_ACCESS = new(2, "调入式");

    private static readonly List<BusinessPartyTypeEnum> _values = new List<BusinessPartyTypeEnum>() { BUSINESS_PARTY_TYPE_Embed, BUSINESS_PARTY_TYPE_ACCESS };

    public static string GetDescByCode(int code)
    {
        return _values.FirstOrDefault(e => e.Code == code)?.Desc;
    }

    public static BusinessPartyTypeEnum FromCode(int code)
    {
        return _values.FirstOrDefault(e => e.Code == code);
    }

    public static List<BusinessPartyTypeEnum> Values() => _values;
}
