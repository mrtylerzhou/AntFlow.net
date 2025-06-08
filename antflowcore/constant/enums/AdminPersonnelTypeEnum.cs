namespace antflowcore.constant.enus;

using System;
using System.Collections.Generic;
using System.Linq;

public class AdminPersonnelTypeEnum
{
    public int Code { get; private set; }
    public string Desc { get; private set; }
    public string StrField { get; private set; }
    public string ListField { get; private set; }
    public string IdsField { get; private set; }
    public string PermCode { get; private set; }

    private AdminPersonnelTypeEnum(int code, string desc, string strField, string listField, string idsField, string permCode)
    {
        this.Code = code;
        this.Desc = desc;
        this.StrField = strField;
        this.ListField = listField;
        this.IdsField = idsField;
        this.PermCode = permCode;
    }

    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_PROCESS = new AdminPersonnelTypeEnum(
        1, "流程管理员", "ProcessAdminsStr", "ProcessAdmins", "ProcessAdminIds", "YWFLCGL"
    );

    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_APPLICATION = new AdminPersonnelTypeEnum(
        2, "应用管理员", "ApplicationAdminsStr", "ApplicationAdminsStr", "ApplicationAdminIds", "YWFYYGL"
    );

    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_INTERFACE = new AdminPersonnelTypeEnum(
        3, "接口管理员", "InterfaceAdminsStr", "InterfaceAdmins", "InterfaceAdminIds", "YWFJKGL"
    );

    public static readonly AdminPersonnelTypeEnum ADMIN_PERSONNEL_TYPE_TEMPLATE = new AdminPersonnelTypeEnum(
        4, "条件模板管理员", "TemplateAdminsStr", "TemplateAdminsStr", "TemplateAdminIds", "YWFMBGL"
    );

    private static readonly List<AdminPersonnelTypeEnum> _values = new List<AdminPersonnelTypeEnum>
    {
        ADMIN_PERSONNEL_TYPE_PROCESS,
        ADMIN_PERSONNEL_TYPE_APPLICATION,
        ADMIN_PERSONNEL_TYPE_INTERFACE,
        ADMIN_PERSONNEL_TYPE_TEMPLATE
    };

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
