namespace antflowcore.constant.enums;

public enum ConfigurationTableAdapterEnum
{
    FINANCE_CASHER_ADP,
    FINANCE_CASHER_SUPERVISOR_ADP,
    FINANCE_MANAGER_ADP,
    FINANCE_CASHER_MANAGER_ADP,
    FINANCE_CASHER_DIRECTOR_ADP,
    FINANCE_CFO_ADP
}

public static class ConfigurationTableAdapterEnumExtensions
{
    private static readonly Dictionary<ConfigurationTableAdapterEnum, BusinessConfTableFieldEnum> AdapterToFieldMap =
        new()
        {
            { ConfigurationTableAdapterEnum.FINANCE_CASHER_ADP, BusinessConfTableFieldEnum.FINANCE_CASHER },
            {
                ConfigurationTableAdapterEnum.FINANCE_CASHER_SUPERVISOR_ADP,
                BusinessConfTableFieldEnum.FINANCE_CASHER_SUPERVISOR
            },
            { ConfigurationTableAdapterEnum.FINANCE_MANAGER_ADP, BusinessConfTableFieldEnum.FINANCE_MANAGER },
            {
                ConfigurationTableAdapterEnum.FINANCE_CASHER_MANAGER_ADP,
                BusinessConfTableFieldEnum.FINANCE_CASHER_MANAGER
            },
            {
                ConfigurationTableAdapterEnum.FINANCE_CASHER_DIRECTOR_ADP,
                BusinessConfTableFieldEnum.FINANCE_CASHER_DIRECTOR
            },
            { ConfigurationTableAdapterEnum.FINANCE_CFO_ADP, BusinessConfTableFieldEnum.FINANCE_CFO }
        };

    public static BusinessConfTableFieldEnum GetTableFieldEnum(this ConfigurationTableAdapterEnum adapterEnum)
    {
        return AdapterToFieldMap[adapterEnum];
    }

    public static ConfigurationTableAdapterEnum? GetByTableFieldEnum(BusinessConfTableFieldEnum tableFieldEnum)
    {
        return AdapterToFieldMap.FirstOrDefault(a => a.Value == tableFieldEnum).Key;
    }
}