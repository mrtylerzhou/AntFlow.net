namespace AntFlow.Core.Constant.Enums;

public enum ProcessOperationEnum
{
    BUTTON_TYPE_SUBMIT = 1, // 提交
    BUTTON_TYPE_RESUBMIT = 2, // 重新提交
    BUTTON_TYPE_AGREE = 3, // 同意
    BUTTON_TYPE_DIS_AGREE = 4, // 不同意
    BUTTON_TYPE_VIEW_BUSINESS_PROCESS = 5, // 查看业务流程
    BUTTON_TYPE_ABANDON = 7, // 废弃
    BUTTON_TYPE_UNDERTAKE = 10, // 承办
    BUTTON_TYPE_CHANGE_ASSIGNEE = 11, // 更改办理人
    BUTTON_TYPE_STOP = 12, // 终止
    BUTTON_TYPE_FORWARD = 15, // 转发
    BUTTON_TYPE_BACK_TO_MODIFY = 18, // 退回修改
    BUTTON_TYPE_JP = 19, // 加批
    BUTTON_TYPE_ZB = 21, // 主办
    BUTTON_TYPE_CHOOSE_ASSIGNEE = 22, //选择办理人
    BUTTON_TYPE_BACK_TO_ANY_NODE = 23, //退回任意节点
    BUTTON_TYPE_REMOVE_ASSIGNEE = 24, //移除办理人
    BUTTON_TYPE_ADD_ASSIGNEE = 25, //添加办理人//添加办理人到当前节点，用于多人会签等场景
    BUTTON_TYPE_CHANGE_FUTURE_ASSIGNEE = 26, //更改后续办理人
    BUTTON_TYPE_REMOVE_FUTURE_ASSIGNEE = 27, //移除后续办理人
    BUTTON_TYPE_ADD_FUTURE_ASSIGNEE = 28, //添加后续办理人
    BUTTON_TYPE_PROCESS_DRAW_BACK = 29 //流程撤回
}

public static class ProcessOperationEnumExtensions
{
    // 获取描述
    public static string GetDesc(this ProcessOperationEnum operation)
    {
        switch (operation)
        {
            case ProcessOperationEnum.BUTTON_TYPE_SUBMIT:
                return "提交";
            case ProcessOperationEnum.BUTTON_TYPE_RESUBMIT:
                return "重新提交";
            case ProcessOperationEnum.BUTTON_TYPE_AGREE:
                return "同意";
            case ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE:
                return "不同意";
            case ProcessOperationEnum.BUTTON_TYPE_VIEW_BUSINESS_PROCESS:
                return "查看业务流程";
            case ProcessOperationEnum.BUTTON_TYPE_ABANDON:
                return "废弃";
            case ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE:
                return "承办";
            case ProcessOperationEnum.BUTTON_TYPE_CHANGE_ASSIGNEE:
                return "更改办理人";
            case ProcessOperationEnum.BUTTON_TYPE_STOP:
                return "终止";
            case ProcessOperationEnum.BUTTON_TYPE_FORWARD:
                return "转发";
            case ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY:
                return "退回修改";
            case ProcessOperationEnum.BUTTON_TYPE_JP:
                return "加批";
            case ProcessOperationEnum.BUTTON_TYPE_ZB:
                return "主办";
            case ProcessOperationEnum.BUTTON_TYPE_CHOOSE_ASSIGNEE:
                return "选择办理人";
            case ProcessOperationEnum.BUTTON_TYPE_BACK_TO_ANY_NODE:
                return "退回任意节点";
            case ProcessOperationEnum.BUTTON_TYPE_REMOVE_ASSIGNEE:
                return "移除办理人";
            case ProcessOperationEnum.BUTTON_TYPE_ADD_ASSIGNEE:
                return "添加办理人";
            case ProcessOperationEnum.BUTTON_TYPE_CHANGE_FUTURE_ASSIGNEE:
                return "更改后续办理人";
            case ProcessOperationEnum.BUTTON_TYPE_REMOVE_FUTURE_ASSIGNEE:
                return "移除后续办理人";
            case ProcessOperationEnum.BUTTON_TYPE_ADD_FUTURE_ASSIGNEE:
                return "添加后续办理人";
            case ProcessOperationEnum.BUTTON_TYPE_PROCESS_DRAW_BACK:
                return "流程撤回";
            default:
                return null;
        }
    }

    // 根据Code获取枚举
    public static ProcessOperationEnum? GetEnumByCode(int? code)
    {
        if (code == null)
        {
            return null;
        }

        if (Enum.IsDefined(typeof(ProcessOperationEnum), code))
        {
            return (ProcessOperationEnum)code;
        }

        return null;
    }

    // 获取所有枚举及描述
    public static Dictionary<int, string> GetAllEnums()
    {
        Dictionary<int, string>? enumDescriptions = new();
        foreach (object? value in Enum.GetValues(typeof(ProcessOperationEnum)))
        {
            enumDescriptions.Add((int)value, ((ProcessOperationEnum)value).GetDesc());
        }

        return enumDescriptions;
    }
}