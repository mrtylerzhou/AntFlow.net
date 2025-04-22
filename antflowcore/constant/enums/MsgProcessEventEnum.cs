namespace antflowcore.constant.enums
{
    public enum MsgProcessEventEnum
    {
        NULL = 0,
        PROCESS_SUBMIT = 1,
        PROCESS_RESUBMIT = 2,
        PROCESS_APPROVE = 3,
        PROCESS_NOT_APPROVE = 4,
        PROCESS_ABANDON = 7,
        PROCESS_UNDERTAKE = 10,
        PROCESS_CHANGE_DEALER = 11,
        PROCESS_ABORT = 12,
        PROCESS_FORWARD = 15,
        BUTTON_BACK_TO_MODIFY = 18,
        PROCESS_JP = 19,
        PROCESS_FINISH = 20,
        HISTORY_SYNC = 100,
        PROCESS_DATA_SYNC = 101
    }

    public static class MsgProcessEventEnumExtensions
    {
        public static string GetDescription(this MsgProcessEventEnum value)
        {
            switch (value)
            {
                case MsgProcessEventEnum.PROCESS_SUBMIT: return "流程提交操作";
                case MsgProcessEventEnum.PROCESS_RESUBMIT: return "重新提交";
                case MsgProcessEventEnum.PROCESS_APPROVE: return "同意";
                case MsgProcessEventEnum.PROCESS_NOT_APPROVE: return "不同意";
                case MsgProcessEventEnum.PROCESS_ABANDON: return "作废";
                case MsgProcessEventEnum.PROCESS_UNDERTAKE: return "承办";
                case MsgProcessEventEnum.PROCESS_CHANGE_DEALER: return "变更处理人";
                case MsgProcessEventEnum.PROCESS_ABORT: return "终止";
                case MsgProcessEventEnum.PROCESS_FORWARD: return "转发";
                case MsgProcessEventEnum.BUTTON_BACK_TO_MODIFY: return "打回修改";
                case MsgProcessEventEnum.PROCESS_JP: return "加批";
                case MsgProcessEventEnum.PROCESS_FINISH: return "流程完成";
                case MsgProcessEventEnum.HISTORY_SYNC: return "同步历史数据";
                case MsgProcessEventEnum.PROCESS_DATA_SYNC: return "流程历史数据同步";
                default: return "空";
            }
        }

        public static MsgProcessEventEnum OfCode(int code)
        {
            return Enum.IsDefined(typeof(MsgProcessEventEnum), code)
                ? (MsgProcessEventEnum)code
                : MsgProcessEventEnum.NULL;
        }

        public static MsgProcessEventEnum GetEnumByCode(int? code)
        {
            if (code == null)
                return MsgProcessEventEnum.NULL;

            return Enum.IsDefined(typeof(MsgProcessEventEnum), code)
                ? (MsgProcessEventEnum)code
                : MsgProcessEventEnum.NULL;
        }
    }
}