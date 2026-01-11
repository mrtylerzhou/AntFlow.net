using antflowcore.exception;

namespace antflowcore.constant.enus;

public enum NodePropertyEnum
{
    NODE_PROPERTY_LOOP = 2,
    NODE_PROPERTY_LEVEL = 3,
    NODE_PROPERTY_ROLE = 4,
    NODE_PROPERTY_PERSONNEL = 5,
    NODE_PROPERTY_HRBP = 6,
    NODE_PROPERTY_CUSTOMIZE = 7,
    NODE_PROPERTY_BUSINESSTABLE = 8,
    NODE_PROPERTY_OUT_SIDE_ACCESS = 11,
    NODE_PROPERTY_START_USER = 12,
    NODE_PROPERTY_DIRECT_LEADER = 13
}
public static class NodePropertyEnumExtensions
{
    // 存储每个枚举的附加信息
    private static readonly Dictionary<NodePropertyEnum, (string Desc, int HasPropertyTable, BpmnNodeParamTypeEnum ParamType)> Metadata =
        new()
        {
            { NodePropertyEnum.NODE_PROPERTY_LOOP, ("层层审批", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE) },
            { NodePropertyEnum.NODE_PROPERTY_LEVEL, ("指定层级审批", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE) },
            { NodePropertyEnum.NODE_PROPERTY_ROLE, ("指定角色", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) },
            { NodePropertyEnum.NODE_PROPERTY_PERSONNEL, ("指定人员", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) },
            { NodePropertyEnum.NODE_PROPERTY_HRBP, ("HRBP", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE) },
            { NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE, ("自选模块", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) },
            { NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE, ("关联业务表", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) },
            { NodePropertyEnum.NODE_PROPERTY_OUT_SIDE_ACCESS, ("外部传入人员", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) },
            { NodePropertyEnum.NODE_PROPERTY_START_USER, ("发起人", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE) },
            { NodePropertyEnum.NODE_PROPERTY_DIRECT_LEADER, ("直属领导", 1, BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER) }
        };

    public static BpmnNodeParamTypeEnum GetParamTypeEnum(this NodePropertyEnum property)
    {
        return Metadata[property].ParamType;
    }
    // 获取描述
    public static string GetDesc(this NodePropertyEnum nodePropertyEnum)
    {
        return Metadata.TryGetValue(nodePropertyEnum, out var data) ? data.Desc : null;
    }

    // 是否有属性表
    public static int GetHasPropertyTable(this NodePropertyEnum nodePropertyEnum)
    {
        return Metadata.TryGetValue(nodePropertyEnum, out var data) ? data.HasPropertyTable : 0;
    }

    // 获取参数类型
    public static BpmnNodeParamTypeEnum GetParamType(this NodePropertyEnum nodePropertyEnum)
    {
        return Metadata.TryGetValue(nodePropertyEnum, out var data) ? data.ParamType : default;
    }

    // 获取所有含有属性表的枚举
    public static IEnumerable<NodePropertyEnum> GetEnumsWithPropertyTable()
    {
        return Metadata.Where(kv => kv.Value.HasPropertyTable == 1).Select(kv => kv.Key);
    }

    public static string GetDescByCode(int? code)
    {
        NodePropertyEnum? nodePropertyEnum = GetByCode(code);
        if (nodePropertyEnum == null)
        {
            return null;
        }

        return Metadata[nodePropertyEnum.Value].Desc;
    }
    // 根据 Code 获取枚举
    public static NodePropertyEnum? GetByCode(int? code)
    {
        if (code == null)
        {
            return null;
        }
        return Enum.IsDefined(typeof(NodePropertyEnum), code) 
            ? (NodePropertyEnum?)code 
            : null;
    }
}
