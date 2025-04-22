namespace antflowcore.constant.enums
{
    public enum ButtonTypeEnum
    {
        BUTTON_TYPE_PREVIEW = 0,          // 预览
        BUTTON_TYPE_SUBMIT = 1,           // 提交
        BUTTON_TYPE_RESUBMIT = 2,         // 重新提交
        BUTTON_TYPE_AGREE = 3,            // 同意
        BUTTON_TYPE_DISAGREE = 4,         // 不同意
        BUTTON_TYPE_BACK_TO_PREV_MODIFY = 6,  // 打回上节点修改
        BUTTON_TYPE_ABANDONED = 7,        // 作废
        BUTTON_TYPE_PRINT = 8,            // 打印
        BUTTON_TYPE_UNDERTAKE = 10,       // 承办
        BUTTON_TYPE_CHANGE_ASSIGNEE = 11, // 变更处理人
        BUTTON_TYPE_STOP = 12,            // 终止
        BUTTON_TYPE_ADD_ASSIGNEE = 13,    // 添加审批人
        BUTTON_TYPE_FORWARD = 15,         // 转发
        BUTTON_TYPE_BACK_TO_MODIFY = 18,  // 打回修改
        BUTTON_TYPE_JP = 19,              // 加批
        BUTTON_TYPE_ZB = 21               // 转办
    }

    public static class ButtonTypeEnumExtensions
    {
        public static string GetDescByCode(int code)
        {
            switch (code)
            {
                case (int)ButtonTypeEnum.BUTTON_TYPE_PREVIEW:
                    return "预览";

                case (int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT:
                    return "提交";

                case (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT:
                    return "重新提交";

                case (int)ButtonTypeEnum.BUTTON_TYPE_AGREE:
                    return "同意";

                case (int)ButtonTypeEnum.BUTTON_TYPE_DISAGREE:
                    return "不同意";

                case (int)ButtonTypeEnum.BUTTON_TYPE_BACK_TO_PREV_MODIFY:
                    return "打回上节点修改";

                case (int)ButtonTypeEnum.BUTTON_TYPE_ABANDONED:
                    return "作废";

                case (int)ButtonTypeEnum.BUTTON_TYPE_PRINT:
                    return "打印";

                case (int)ButtonTypeEnum.BUTTON_TYPE_UNDERTAKE:
                    return "承办";

                case (int)ButtonTypeEnum.BUTTON_TYPE_CHANGE_ASSIGNEE:
                    return "变更处理人";

                case (int)ButtonTypeEnum.BUTTON_TYPE_STOP:
                    return "终止";

                case (int)ButtonTypeEnum.BUTTON_TYPE_ADD_ASSIGNEE:
                    return "添加审批人";

                case (int)ButtonTypeEnum.BUTTON_TYPE_FORWARD:
                    return "转发";

                case (int)ButtonTypeEnum.BUTTON_TYPE_BACK_TO_MODIFY:
                    return "打回修改";

                case (int)ButtonTypeEnum.BUTTON_TYPE_JP:
                    return "加批";

                case (int)ButtonTypeEnum.BUTTON_TYPE_ZB:
                    return "转办";

                default:
                    return null;
            }
        }
    }
}