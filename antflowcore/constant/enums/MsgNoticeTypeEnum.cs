namespace antflowcore.constant.enums;

public enum MsgNoticeTypeEnum
{
    // 消息通知类型
    PROCESS_FLOW = 1,               // 工作流流转通知

    RECEIVE_FLOW_PROCESS = 2,       // 收到转发工作流通知
    PROCESS_FINISH = 3,             // 工作流完成通知
    PROCESS_REJECT = 4,             // 工作流流程审批不通过通知
    PROCESS_TIME_OUT = 5,           // 工作流超时通知
    PROCESS_STOP = 6,               // 工作流被终止通知
    PROCESS_WAIR_VERIFY = 7,       // 工作流代审批通知
    PROCESS_CHANGE_ORIAL_TREATOR = 8,  // 工作流变更处理人通知(原审批节点处理人)
    PROCESS_CHANGE_NOW_TREATOR = 9,    // 工作流变更处理人通知(现审批节点处理人)
    PROCESS_SILENCE = 10           // 发送流程沉默消息通知
}

public static class MsgNoticeTypeEnumExtensions
{
    // 获取描述信息
    public static string GetDescByCode(int code)
    {
        switch (code)
        {
            case (int)MsgNoticeTypeEnum.PROCESS_FLOW: return "工作流流转通知";
            case (int)MsgNoticeTypeEnum.RECEIVE_FLOW_PROCESS: return "收到转发工作流通知";
            case (int)MsgNoticeTypeEnum.PROCESS_FINISH: return "工作流完成通知";
            case (int)MsgNoticeTypeEnum.PROCESS_REJECT: return "工作流流程审批不通过通知";
            case (int)MsgNoticeTypeEnum.PROCESS_TIME_OUT: return "工作流超时通知";
            case (int)MsgNoticeTypeEnum.PROCESS_STOP: return "工作流被终止通知";
            case (int)MsgNoticeTypeEnum.PROCESS_WAIR_VERIFY: return "工作流代审批通知";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_ORIAL_TREATOR: return "工作流变更处理人通知(原审批节点处理人)";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_NOW_TREATOR: return "工作流变更处理人通知(现审批节点处理人)";
            case (int)MsgNoticeTypeEnum.PROCESS_SILENCE: return "发送流程沉默消息通知";
            default: return null;
        }
    }

    // 获取默认值
    public static string GetDefaultValueByCode(int code)
    {
        switch (code)
        {
            case (int)MsgNoticeTypeEnum.PROCESS_FLOW: return "您有1个{流程类型}{流程名称}{流程编号}需要处理";
            case (int)MsgNoticeTypeEnum.RECEIVE_FLOW_PROCESS: return "您有1个{流程类型}{流程名称}{流程编号}需要查看";
            case (int)MsgNoticeTypeEnum.PROCESS_FINISH: return "您的{流程类型}{流程名称}{流程编号}已完成";
            case (int)MsgNoticeTypeEnum.PROCESS_REJECT: return "您参与审批的{流程类型}{流程名称}{流程编号}已被{审批不同意者}驳回";
            case (int)MsgNoticeTypeEnum.PROCESS_TIME_OUT: return "您有1个{流程类型}{流程名称}{流程编号}已超过处理期限，请立即处理";
            case (int)MsgNoticeTypeEnum.PROCESS_STOP: return "您参与审批的{流程类型}{流程名称}{流程编号}已被{操作者}终止";
            case (int)MsgNoticeTypeEnum.PROCESS_WAIR_VERIFY: return "您参与审批的{流程类型}{流程名称}{流程编号}已被{操作者}代审批";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_ORIAL_TREATOR: return "您参与审批的{流程类型}{流程名称}{流程编号}已被变更为{变更后处理人}处理";
            case (int)MsgNoticeTypeEnum.PROCESS_CHANGE_NOW_TREATOR: return "您有1个从{原审批节点处理人}转给您的{流程类型}{流程名称}{流程编号}需要处理";
            case (int)MsgNoticeTypeEnum.PROCESS_SILENCE: return "{流程类型}{流程名称}{流程编号}无人处理，请至流程管理中进行干预。";
            default: return null;
        }
    }
}