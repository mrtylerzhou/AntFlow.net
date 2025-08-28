namespace AntFlow.Core.Constant.Enums;

public enum BusinessConfTableFieldEnum
{
    FINANCE_CASHER = 1,
    FINANCE_CASHER_SUPERVISOR = 2,
    FINANCE_MANAGER = 3,
    FINANCE_CASHER_MANAGER = 4,
    FINANCE_CASHER_DIRECTOR = 5,
    FINANCE_CFO = 16
}

public static class BusinessConfTableFieldEnumExtensions
{
    public static ConfigurationTableEnum ParentTable(this BusinessConfTableFieldEnum field)
    {
        switch (field)
        {
            case BusinessConfTableFieldEnum.FINANCE_CASHER:
            case BusinessConfTableFieldEnum.FINANCE_CASHER_SUPERVISOR:
            case BusinessConfTableFieldEnum.FINANCE_MANAGER:
            case BusinessConfTableFieldEnum.FINANCE_CASHER_MANAGER:
            case BusinessConfTableFieldEnum.FINANCE_CASHER_DIRECTOR:
            case BusinessConfTableFieldEnum.FINANCE_CFO:
                return ConfigurationTableEnum.COMPANY_FINANCE;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static string GetDesc(this BusinessConfTableFieldEnum field)
    {
        switch (field)
        {
            case BusinessConfTableFieldEnum.FINANCE_CASHER:
                return "Ʊ�����";
            case BusinessConfTableFieldEnum.FINANCE_CASHER_SUPERVISOR:
                return "Ʊ�ݸ���";
            case BusinessConfTableFieldEnum.FINANCE_MANAGER:
                return "�ʽ�����";
            case BusinessConfTableFieldEnum.FINANCE_CASHER_MANAGER:
                return "���ҹ�˾�ʽ����ɫ��Ա";
            case BusinessConfTableFieldEnum.FINANCE_CASHER_DIRECTOR:
                return "���ҹ�˾�ʽ��ܼ��ɫ";
            case BusinessConfTableFieldEnum.FINANCE_CFO:
                return "���ҹ�˾CFO��ɫ��Ա";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static List<BusinessConfTableFieldEnum> GetByParentTable(ConfigurationTableEnum parentTable)
    {
        if (parentTable == ConfigurationTableEnum.COMPANY_FINANCE)
        {
            return new List<BusinessConfTableFieldEnum>
            {
                BusinessConfTableFieldEnum.FINANCE_CASHER,
                BusinessConfTableFieldEnum.FINANCE_CASHER_SUPERVISOR,
                BusinessConfTableFieldEnum.FINANCE_MANAGER,
                BusinessConfTableFieldEnum.FINANCE_CASHER_MANAGER,
                BusinessConfTableFieldEnum.FINANCE_CASHER_DIRECTOR,
                BusinessConfTableFieldEnum.FINANCE_CFO
            };
        }

        return null;
    }

    public static BusinessConfTableFieldEnum? GetTableFieldEnumByCode(int code)
    {
        return Enum.IsDefined(typeof(BusinessConfTableFieldEnum), code)
            ? (BusinessConfTableFieldEnum?)code
            : null;
    }
}