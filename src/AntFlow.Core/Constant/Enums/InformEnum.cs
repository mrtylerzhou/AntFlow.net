namespace AntFlow.Core.Constant.Enums;

public enum InformEnum
{
    APPLICANT = 1, // ??????
    ALL_APPROVER = 2, // ????????????
    AT_APPROVER = 3, // ????????????
    BY_TRANSPOND = 4 // ???????
}

public static class InformEnumExtensions
{
    // ???????
    public static string GetDesc(this InformEnum informEnum)
    {
        switch (informEnum)
        {
            case InformEnum.APPLICANT:
                return "??????";
            case InformEnum.ALL_APPROVER:
                return "????????????";
            case InformEnum.AT_APPROVER:
                return "????????????";
            case InformEnum.BY_TRANSPOND:
                return "???????";
            default:
                return null;
        }
    }

    // ????????
    public static string GetFileName(this InformEnum informEnum)
    {
        switch (informEnum)
        {
            case InformEnum.APPLICANT:
                return "StartUser";
            case InformEnum.ALL_APPROVER:
                return "Approveds";
            case InformEnum.AT_APPROVER:
                return "Assignee";
            case InformEnum.BY_TRANSPOND:
                return "ForwardUsers";
            default:
                return null;
        }
    }

    // ????Code??????
    public static InformEnum? GetEnumByCode(int code)
    {
        if (Enum.IsDefined(typeof(InformEnum), code))
        {
            return (InformEnum)code;
        }

        return null;
    }

    // ??????е????????????
    public static Dictionary<int, Tuple<string, string>> GetAllEnums()
    {
        Dictionary<int, Tuple<string, string>>? enumDescriptions = new();
        foreach (object? value in Enum.GetValues(typeof(InformEnum)))
        {
            InformEnum enumValue = (InformEnum)value;
            enumDescriptions.Add((int)value, new Tuple<string, string>(enumValue.GetDesc(), enumValue.GetFileName()));
        }

        return enumDescriptions;
    }
}