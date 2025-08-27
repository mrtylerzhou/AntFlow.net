namespace AntFlow.Core.Constant.Enums;

public class ElementTypeEnum
{
    public static readonly ElementTypeEnum ELEMENT_TYPE_START_EVENT = new(1, "StartEvent");
    public static readonly ElementTypeEnum ELEMENT_TYPE_USER_TASK = new(2, "UserTask");
    public static readonly ElementTypeEnum ELEMENT_TYPE_GATEWAY = new(3, "Gateway");
    public static readonly ElementTypeEnum ELEMENT_TYPE_SEQUENCE_FLOW = new(4, "SequenceFlow");
    public static readonly ElementTypeEnum ELEMENT_TYPE_END_EVENT = new(5, "EndEvent");
    public static readonly ElementTypeEnum ELEMENT_TYPE_PARALLEL_GATEWAY = new(6, "ParallelGateWay");

    private ElementTypeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public int Code { get; private set; }
    public string Desc { get; }

    public override string ToString()
    {
        return Desc;
    }
}