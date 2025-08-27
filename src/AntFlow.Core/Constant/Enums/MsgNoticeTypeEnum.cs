namespace AntFlow.Core.Constant.Enums;

public enum MsgNoticeTypeEnum
{
    // 消息通知类型
    PROCESS_FLOW = 1, // 流程流转通知
    RECEIVE_FLOW_PROCESS = 2, // 收到流转流程通知
    PROCESS_FINISH = 3, // 流程完成通知
    PROCESS_REJECT = 4, // 流程被拒绝或不通过通知
    PROCESS_TIME_OUT = 5, // 流程超时通知
    PROCESS_STOP = 6, // 流程被终止通知
    PROCESS_WAIR_VERIFY = 7, // 流程等待审批通知
    PROCESS_CHANGE_ORIAL_TREATOR = 8, // 流程转办人变更通知(原处理节点处理人)
    PROCESS_CHANGE_NOW_TREATOR = 9, // 流程转办人变更通知(现处理节点处理人)
    PROCESS_SILENCE = 10 // 流程静默消息通知
}

public static class MsgNoticeTypeEnumExtensions
{
    // 获取描述信息
    public static string GetDescByCode(int code)
    {
        switch (code)
        {
            case (int)MsgNoticeTypeEnum.PROCESS_FLOW: return "流程流转通知";
            case (int)MsgNoticeTypeEnum.RECEIVE_FLOW_PROCESS: return "收到流转流程通知";
            case (int)MsgNoticeTypeEnum.PROCESS_FINISH: return "流程完成通知";
            case (int)MsgNoticeTypeEnum.PROCESS_REJECT: return "流程被拒绝或不通过通知";
            case (int)MsgNoticeTypeEnum.PROCESS_TIME_OUT: return "流程超时通知";
            case (int)MsgNoticeTypeEnum.PROCESS_STOP: return "流程被终止通知";
            case (int)MsgNoticeTypeEnum.PROCESS_WAIR_VERIFY: return "流程等待审批通知";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_ORIAL_TREATOR: return "流程转办人变更通知(原处理节点处理人)";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_NOW_TREATOR: return "流程转办人变更通知(现处理节点处理人)";
            case (int)MsgNoticeTypeEnum.PROCESS_SILENCE: return "流程静默消息通知";
            default: return null;
        }
    }

    // 获取默认值
    public static string GetDefaultValueByCode(int code)
    {
        switch (code)
        {
            case (int)MsgNoticeTypeEnum.PROCESS_FLOW: return "您有1个{流程发起人}{流程名称}{流程编号}需要处理";
            case (int)MsgNoticeTypeEnum.RECEIVE_FLOW_PROCESS: return "您有1个{流程发起人}{流程名称}{流程编号}需要查看";
            case (int)MsgNoticeTypeEnum.PROCESS_FINISH: return "您的{流程发起人}{流程名称}{流程编号}已完成";
            case (int)MsgNoticeTypeEnum.PROCESS_REJECT: return "很抱歉通知您{流程发起人}{流程名称}{流程编号}已被{拒绝或不同意人}拒绝";
            case (int)MsgNoticeTypeEnum.PROCESS_TIME_OUT: return "您有1个{流程发起人}{流程名称}{流程编号}已超过处理时限，请及时处理";
            case (int)MsgNoticeTypeEnum.PROCESS_STOP: return "很抱歉通知您{流程发起人}{流程名称}{流程编号}已被{终止人}终止";
            case (int)MsgNoticeTypeEnum.PROCESS_WAIR_VERIFY: return "很抱歉通知您{流程发起人}{流程名称}{流程编号}已被{委托人}委托";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_ORIAL_TREATOR: return "很抱歉通知您{流程发起人}{流程名称}{流程编号}已被转为{转办处理人}处理";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_NOW_TREATOR: return "您有1个由{原处理节点处理人}转办给您{流程发起人}{流程名称}{流程编号}需要处理";
            case (int)MsgNoticeTypeEnum.PROCESS_SILENCE: return "{流程发起人}{流程名称}{流程编号}因为此次处理，流程规则将进行相应预警";
            default: return null;
        }
    }
}