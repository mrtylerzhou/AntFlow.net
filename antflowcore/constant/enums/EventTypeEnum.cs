using antflowcore.exception;

namespace antflowcore.constant.enums
{
    public enum EventTypeEnum
    {
        PROCESS_INITIATOR = 1,       // 流程发起
        PROCESS_CANCELLATION = 2,     // 作废操作
        PROCESS_FLOW = 3,             // 流程流转至当前节点
        PROCESS_CONSENT = 4,          // 同意操作
        PROCESS_DISAGREE = 5,         // 不同意操作
        PROCESS_ADDAPPROVE = 6,       // 加批操作
        PROCESS_REPULSE = 7,          // 打回修改操作
        PROCESS_TRANSPOND = 8,        // 转发操作
        PROCESS_END = 9               // 流程结束
    }

    public static class EventTypeEnumExtensions
    {
        private static readonly Dictionary<EventTypeEnum, EventTypeProperties> EventTypeMappings = new()
        {
            { EventTypeEnum.PROCESS_INITIATOR, new EventTypeProperties(true, "流程发起", (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT, 1, new List<int>()) },
            { EventTypeEnum.PROCESS_CANCELLATION, new EventTypeProperties(false, "作废操作", (int)ProcessOperationEnum.BUTTON_TYPE_ABANDON, 0, new List<int> { (int)InformEnum.APPLICANT }) },
            { EventTypeEnum.PROCESS_FLOW, new EventTypeProperties(true, "流程流转至当前节点", 0, 4, new List<int> { (int)InformEnum.AT_APPROVER }) },
            { EventTypeEnum.PROCESS_CONSENT, new EventTypeProperties(true, "同意操作", (int) ProcessOperationEnum.BUTTON_TYPE_AGREE, 4, new List<int>()) },
            { EventTypeEnum.PROCESS_DISAGREE, new EventTypeProperties(true, "不同意操作", (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE, 4, new List<int> { (int)InformEnum.APPLICANT }) },
            { EventTypeEnum.PROCESS_ADDAPPROVE, new EventTypeProperties(true, "加批操作", (int) ProcessOperationEnum.BUTTON_TYPE_JP, 4, new List<int>()) },
            { EventTypeEnum.PROCESS_REPULSE, new EventTypeProperties(true, "打回修改操作", (int)ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY, 4, new List<int>()) },
            { EventTypeEnum.PROCESS_TRANSPOND, new EventTypeProperties(false, "转发操作", (int)ProcessOperationEnum.BUTTON_TYPE_FORWARD, 0, new List<int> { (int)InformEnum.BY_TRANSPOND }) },
            { EventTypeEnum.PROCESS_END, new EventTypeProperties(false, "流程结束", 0, 0, new List<int> { (int)InformEnum.APPLICANT }) }
        };

        // 获取描述
        public static string GetDescription(this EventTypeEnum eventType)
        {
            return EventTypeMappings[eventType].Description;
        }

        public static string GetDescByCode(int code)
        {
            string description = EventTypeMappings[(EventTypeEnum)code].Description;
            return description;
        }

        // 根据操作类型获取枚举
        public static EventTypeEnum? GetEnumByOperationType(int operationType)
        {
            return EventTypeMappings.FirstOrDefault(kv => kv.Value.ProcessOperationType == operationType).Key;
        }

        // 获取通知对象ID列表
        public static List<int> GetInformIdList(this EventTypeEnum eventTypeEnum)
        {
            return EventTypeMappings[eventTypeEnum].InformIdList;
        }

        public static bool IsInNode(this EventTypeEnum? eventType)
        {
            if (eventType == null)
            {
                throw new AFBizException($"can not get EventTypeEnum by code {eventType}");
            }
            return EventTypeMappings[eventType.Value].IsInNode;
        }
    }
}

internal class EventTypeProperties
{
    public bool IsInNode { get; }
    public string Description { get; }
    public int ProcessOperationType { get; }
    public int NodeType { get; }
    public List<int> InformIdList { get; }

    public EventTypeProperties(bool isInNode, string description, int processOperationType, int nodeType, List<int> informIdList)
    {
        IsInNode = isInNode;
        Description = description;
        ProcessOperationType = processOperationType;
        NodeType = nodeType;
        InformIdList = informIdList;
    }
}