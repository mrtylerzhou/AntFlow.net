namespace AntFlow.Core.Constant.Enums;

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
            case MsgProcessEventEnum.PROCESS_SUBMIT: return "??????????";
            case MsgProcessEventEnum.PROCESS_RESUBMIT: return "??????";
            case MsgProcessEventEnum.PROCESS_APPROVE: return "???";
            case MsgProcessEventEnum.PROCESS_NOT_APPROVE: return "?????";
            case MsgProcessEventEnum.PROCESS_ABANDON: return "????";
            case MsgProcessEventEnum.PROCESS_UNDERTAKE: return "?а?";
            case MsgProcessEventEnum.PROCESS_CHANGE_DEALER: return "?????????";
            case MsgProcessEventEnum.PROCESS_ABORT: return "???";
            case MsgProcessEventEnum.PROCESS_FORWARD: return "???";
            case MsgProcessEventEnum.BUTTON_BACK_TO_MODIFY: return "??????";
            case MsgProcessEventEnum.PROCESS_JP: return "????";
            case MsgProcessEventEnum.PROCESS_FINISH: return "???????";
            case MsgProcessEventEnum.HISTORY_SYNC: return "??????????";
            case MsgProcessEventEnum.PROCESS_DATA_SYNC: return "??????????????";
            default: return "??";
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
        {
            return MsgProcessEventEnum.NULL;
        }

        return Enum.IsDefined(typeof(MsgProcessEventEnum), code)
            ? (MsgProcessEventEnum)code
            : MsgProcessEventEnum.NULL;
    }
}