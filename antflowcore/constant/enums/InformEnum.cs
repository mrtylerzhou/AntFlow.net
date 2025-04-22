namespace antflowcore.constant.enums
{
    public enum InformEnum
    {
        APPLICANT = 1,           // 申请人
        ALL_APPROVER = 2,        // 所有已审批人
        AT_APPROVER = 3,         // 当前节点审批人
        BY_TRANSPOND = 4         // 被转发人
    }

    public static class InformEnumExtensions
    {
        // 获取描述
        public static string GetDesc(this InformEnum informEnum)
        {
            switch (informEnum)
            {
                case InformEnum.APPLICANT:
                    return "申请人";

                case InformEnum.ALL_APPROVER:
                    return "所有已审批人";

                case InformEnum.AT_APPROVER:
                    return "当前节点审批人";

                case InformEnum.BY_TRANSPOND:
                    return "被转发人";

                default:
                    return null;
            }
        }

        // 获取文件名
        public static string GetFileName(this InformEnum informEnum)
        {
            switch (informEnum)
            {
                case InformEnum.APPLICANT:
                    return "startUser";

                case InformEnum.ALL_APPROVER:
                    return "approveds";

                case InformEnum.AT_APPROVER:
                    return "assignee";

                case InformEnum.BY_TRANSPOND:
                    return "forwardUsers";

                default:
                    return null;
            }
        }

        // 根据Code获得枚举
        public static InformEnum? GetEnumByCode(int code)
        {
            if (Enum.IsDefined(typeof(InformEnum), code))
            {
                return (InformEnum)code;
            }
            return null;
        }

        // 获取所有的枚举项及其描述
        public static Dictionary<int, Tuple<string, string>> GetAllEnums()
        {
            var enumDescriptions = new Dictionary<int, Tuple<string, string>>();
            foreach (var value in Enum.GetValues(typeof(InformEnum)))
            {
                var enumValue = (InformEnum)value;
                enumDescriptions.Add((int)value, new Tuple<string, string>(enumValue.GetDesc(), enumValue.GetFileName()));
            }
            return enumDescriptions;
        }
    }
}