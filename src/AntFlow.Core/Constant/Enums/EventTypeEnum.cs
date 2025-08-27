using AntFlow.Core.Exception;

namespace AntFlow.Core.Constant.Enums
{
    public enum EventTypeEnum
    {
        PROCESS_INITIATOR = 1, // ???????
        PROCESS_CANCELLATION = 2, // ???????
        PROCESS_FLOW = 3, // ???????????????
        PROCESS_CONSENT = 4, // ??????
        PROCESS_DISAGREE = 5, // ????????
        PROCESS_ADDAPPROVE = 6, // ????????
        PROCESS_REPULSE = 7, // ?????????
        PROCESS_TRANSPOND = 8, // ???????
        PROCESS_END = 9 // ???????
    }

    public static class EventTypeEnumExtensions
    {
        public static readonly Dictionary<EventTypeEnum, EventTypeProperties> EventTypeMappings = new()
        {
            {
                EventTypeEnum.PROCESS_INITIATOR,
                new EventTypeProperties(true, "???????", (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT, 1,
                    new List<int>())
            },
            {
                EventTypeEnum.PROCESS_CANCELLATION,
                new EventTypeProperties(false, "???????", (int)ProcessOperationEnum.BUTTON_TYPE_ABANDON, 0,
                    new List<int> { (int)InformEnum.APPLICANT })
            },
            {
                EventTypeEnum.PROCESS_FLOW,
                new EventTypeProperties(true, "???????????????", 0, 4, new List<int> { (int)InformEnum.AT_APPROVER })
            },
            {
                EventTypeEnum.PROCESS_CONSENT,
                new EventTypeProperties(true, "??????", (int)ProcessOperationEnum.BUTTON_TYPE_AGREE, 4, new List<int>())
            },
            {
                EventTypeEnum.PROCESS_DISAGREE,
                new EventTypeProperties(true, "????????", (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE, 4,
                    new List<int> { (int)InformEnum.APPLICANT })
            },
            {
                EventTypeEnum.PROCESS_ADDAPPROVE,
                new EventTypeProperties(true, "????????", (int)ProcessOperationEnum.BUTTON_TYPE_JP, 4, new List<int>())
            },
            {
                EventTypeEnum.PROCESS_REPULSE,
                new EventTypeProperties(true, "?????????", (int)ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY, 4,
                    new List<int>())
            },
            {
                EventTypeEnum.PROCESS_TRANSPOND,
                new EventTypeProperties(false, "???????", (int)ProcessOperationEnum.BUTTON_TYPE_FORWARD, 0,
                    new List<int> { (int)InformEnum.BY_TRANSPOND })
            },
            {
                EventTypeEnum.PROCESS_END,
                new EventTypeProperties(false, "???????", 0, 0, new List<int> { (int)InformEnum.APPLICANT })
            }
        };

        // ???????
        public static string GetDescription(this EventTypeEnum eventType)
        {
            return EventTypeMappings[eventType].Description;
        }

        public static string GetDescByCode(int code)
        {
            string description = EventTypeMappings[(EventTypeEnum)code].Description;
            return description;
        }

        // ????????????????
        public static EventTypeEnum? GetEnumByOperationType(int operationType)
        {
            return EventTypeMappings.FirstOrDefault(kv => kv.Value.ProcessOperationType == operationType).Key;
        }

        // ?????????ID?งา?
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

            return IsInNode(eventType.Value);
        }

        public static bool IsInNode(this EventTypeEnum eventType)
        {
            if (eventType == null)
            {
                throw new AFBizException($"can not get EventTypeEnum by code {eventType}");
            }

            return EventTypeMappings[eventType].IsInNode;
        }
    }
}

public class EventTypeProperties
{
    public EventTypeProperties(bool isInNode, string description, int processOperationType, int nodeType,
        List<int> informIdList)
    {
        IsInNode = isInNode;
        Description = description;
        ProcessOperationType = processOperationType;
        NodeType = nodeType;
        InformIdList = informIdList;
    }

    public bool IsInNode { get; }
    public string Description { get; }
    public int ProcessOperationType { get; }
    public int NodeType { get; }
    public List<int> InformIdList { get; }
}