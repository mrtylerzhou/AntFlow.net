namespace AntFlow.Core.Constant.Enums;

public enum ConfigurationTableEnum
{
    COMPANY_FINANCE = 1
}

public static class ConfigurationTableEnumExtensions
{
    public static string GetDesc(this ConfigurationTableEnum tableEnum)
    {
        switch (tableEnum)
        {
            case ConfigurationTableEnum.COMPANY_FINANCE:
                return "��������������ñ�";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static ConfigurationTableEnum? GetInstanceByCode(int code)
    {
        if (Enum.IsDefined(typeof(ConfigurationTableEnum), code))
        {
            return (ConfigurationTableEnum)code;
        }

        return null;
    }
}