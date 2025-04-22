namespace antflowcore.constant.enus;

public class ElementTypeEnum
{
    public static readonly ElementTypeEnum ELEMENT_TYPE_START_EVENT = new ElementTypeEnum(1, "StartEvent");
    public static readonly ElementTypeEnum ELEMENT_TYPE_USER_TASK = new ElementTypeEnum(2, "UserTask");
    public static readonly ElementTypeEnum ELEMENT_TYPE_GATEWAY = new ElementTypeEnum(3, "Gateway");
    public static readonly ElementTypeEnum ELEMENT_TYPE_SEQUENCE_FLOW = new ElementTypeEnum(4, "SequenceFlow");
    public static readonly ElementTypeEnum ELEMENT_TYPE_END_EVENT = new ElementTypeEnum(5, "EndEvent");
    public static readonly ElementTypeEnum ELEMENT_TYPE_PARALLEL_GATEWAY = new ElementTypeEnum(6, "ParallelGateWay");

    public int Code { get; private set; }
    public string Desc { get; private set; }

    private ElementTypeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public override string ToString()
    {
        return Desc;
    }
}