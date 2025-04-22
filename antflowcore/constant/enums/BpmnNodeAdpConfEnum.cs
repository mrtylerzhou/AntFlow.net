namespace antflowcore.constant.enums;

public enum BpmnNodeAdpConfEnum
{
    ADP_CONF_NODE_PROPERTY_LOOP = 1,
    ADP_CONF_NODE_PROPERTY_LEVEL = 2,
    ADP_CONF_NODE_PROPERTY_ROLE = 3,
    ADP_CONF_NODE_PROPERTY_PERSONNEL = 4,
    ADP_CONF_NODE_TYPE_CONDITIONS = 5,
    ADP_CONF_NODE_TYPE_COPY = 6,
    ADP_CONF_NODE_TYPE_OUT_SIDE_CONDITIONS = 7,
    ADP_CONF_NODE_PROPERTY_OUT_SIDE_ACCESS = 8,
    ADP_CONF_NODE_PROPERTY_START_USER = 9,
    ADP_CONF_NODE_PROPERTY_HRBP = 10,
    ADP_CONF_NODE_PROPERTY_BUSINESSTABLE = 11,
    ADP_CONF_NODE_PROPERTY_DIRECT_LEADER = 12,
}

public static class BpmnNodeAdpConfEnumExtensions
{
    // 定义映射规则
    private static readonly Dictionary<BpmnNodeAdpConfEnum, Enum> MappingsDictionary = new()
    {
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LOOP, NodePropertyEnum.NODE_PROPERTY_LOOP },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LEVEL, NodePropertyEnum.NODE_PROPERTY_LEVEL },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_ROLE, NodePropertyEnum.NODE_PROPERTY_ROLE },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_PERSONNEL, NodePropertyEnum.NODE_PROPERTY_PERSONNEL },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_CONDITIONS, NodeTypeEnum.NODE_TYPE_CONDITIONS },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_COPY, NodeTypeEnum.NODE_TYPE_COPY },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_OUT_SIDE_CONDITIONS, NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_OUT_SIDE_ACCESS, NodePropertyEnum.NODE_PROPERTY_OUT_SIDE_ACCESS },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_START_USER, NodePropertyEnum.NODE_PROPERTY_START_USER },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_HRBP, NodePropertyEnum.NODE_PROPERTY_HRBP },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_BUSINESSTABLE, NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE },
        { BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_DIRECT_LEADER, NodePropertyEnum.NODE_PROPERTY_DIRECT_LEADER },
    };

    // 根据 Enum 获取对应的 BpmnNodeAdpConfEnum
    public static BpmnNodeAdpConfEnum? GetBpmnNodeAdpConfEnumByEnum(Enum? enumValue)
    {
        if (enumValue == null)
        {
            return null;
        }
        return MappingsDictionary.FirstOrDefault(x => Equals(x.Value, enumValue)).Key;
    }

    // 获取所有具有属性表的枚举值
    public static List<BpmnNodeAdpConfEnum> GetBpmnNodeAdpConfWithPersonnels()
    {
        List<BpmnNodeAdpConfEnum> bpmnNodeAdpConfEnums = MappingsDictionary
            .Where(x => x.Value is NodePropertyEnum prop && prop.GetHasPropertyTable() == 1)
            .Select(x => x.Key)
            .ToList();
        return bpmnNodeAdpConfEnums;
    }
}