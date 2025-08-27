namespace AntFlow.Core.Constant.Enums;

public class AdminPersonnelTypeEnum
{
    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_PROCESS = new(
        1, "流程管理员", "ProcessAdminsStr", "ProcessAdmins", "ProcessAdminIds", "YWFLCGL"
    );

    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_APPLICATION = new(
        2, "应用管理员", "ApplicationAdminsStr", "ApplicationAdminsStr", "ApplicationAdminIds", "YWFYYGL"
    );

    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_INTERFACE = new(
        3, "接口管理员", "InterfaceAdminsStr", "InterfaceAdmins", "InterfaceAdminIds", "YWFJKGL"
    );

    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_TEMPLATE = new(
        4, "条件模板管理员", "TemplateAdminsStr", "TemplateAdminsStr", "TemplateAdminIds", "YWFMBGL"
    );

    private static readonly List<AdminPersonnelTypeEnum> _values = new()
    {
        ADMIN_PERSONNEL_TYPE_PROCESS,
        ADMIN_PERSONNEL_TYPE_APPLICATION,
        ADMIN_PERSONNEL_TYPE_INTERFACE,
        ADMIN_PERSONNEL_TYPE_TEMPLATE
    };

    private AdminPersonnelTypeEnum(int code, string desc, string strField, string listField, string idsField,
        string permCode)
    {
        Code = code;
        Desc = desc;
        StrField = strField;
        ListField = listField;
        IdsField = idsField;
        PermCode = permCode;
    }

    public int Code { get; }
    public string Desc { get; private set; }
    public string StrField { get; private set; }
    public string ListField { get; private set; }
    public string IdsField { get; private set; }
    public string PermCode { get; }

    public static AdminPersonnelTypeEnum getEnumByType(int code)
    {
        return _values.FirstOrDefault(e => e.Code == code);
    }

    public static AdminPersonnelTypeEnum getEnumByPermCode(string permCode)
    {
        return _values.FirstOrDefault(e => e.PermCode == permCode);
    }

    public static List<AdminPersonnelTypeEnum> Values()
    {
        return _values;
    }
}