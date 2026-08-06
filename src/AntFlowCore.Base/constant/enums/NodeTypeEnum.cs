namespace AntFlowCore.Base.constant.enums;

public enum NodeTypeEnum
    {
        NODE_TYPE_START = 1,           // 发起人节点
        NODE_TYPE_GATEWAY = 2,         // 网关节点
        NODE_TYPE_CONDITIONS = 3,      // 条件节点
        NODE_TYPE_APPROVER = 4,        // 审批人节点
        NODE_TYPE_OUT_SIDE_CONDITIONS = 5, // 接入方条件节点
        NODE_TYPE_COPY = 6,            // 抄送节点
        NODE_TYPE_PARALLEL_GATEWAY = 7, // 并行网关
        NODE_TYPE_COPY_V2 = 8,         // 抄送节点v2
        NODE_TYPE_AUTO_NODE = 9,        // 自动节点
        NODE_TYPE_CONDITION_APPROVE = 12, // 条件审批节点
        NODE_TYPE_CONDITION_COPY = 13,  // 条件抄送节点
        NODE_TYPE_ASSIST = 17,          // 协助节点
        NODE_TYPE_AUTO_ADVANCE = 18,    // 自动推进节点
        NODE_TYPE_AUTO_RETURN = 19,     // 自动退回节点
        NODE_TYPE_CONDITION_RETURN = 20, // 条件退回节点
        NODE_TYPE_CONDITION_RETURN_STARTER = 21 // 条件退回发起人节点
    }

    public class NodeTypeEnumExtensions
    {
        // 获取枚举的描述
        public static string GetDesc(NodeTypeEnum nodeType)
        {
            switch (nodeType)
            {
                case NodeTypeEnum.NODE_TYPE_START: return "发起人节点";
                case NodeTypeEnum.NODE_TYPE_GATEWAY: return "网关节点";
                case NodeTypeEnum.NODE_TYPE_CONDITIONS: return "条件节点";
                case NodeTypeEnum.NODE_TYPE_APPROVER: return "审批人节点";
                case NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS: return "接入方条件节点";
                case NodeTypeEnum.NODE_TYPE_COPY: return "抄送节点";
                case NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY: return "并行网关";
                case NodeTypeEnum.NODE_TYPE_COPY_V2: return "抄送节点v2";
                case NodeTypeEnum.NODE_TYPE_AUTO_NODE: return "自动节点";
                case NodeTypeEnum.NODE_TYPE_CONDITION_APPROVE: return "条件审批节点";
                case NodeTypeEnum.NODE_TYPE_CONDITION_COPY: return "条件抄送节点";
                case NodeTypeEnum.NODE_TYPE_ASSIST: return "协助节点";
                case NodeTypeEnum.NODE_TYPE_AUTO_ADVANCE: return "自动推进节点";
                case NodeTypeEnum.NODE_TYPE_AUTO_RETURN: return "自动退回节点";
                case NodeTypeEnum.NODE_TYPE_CONDITION_RETURN: return "条件退回节点";
                case NodeTypeEnum.NODE_TYPE_CONDITION_RETURN_STARTER: return "条件退回发起人节点";
                default: return string.Empty;
            }
        }

        // 获取含有属性表的节点
        public static List<NodeTypeEnum> GetNodeTypesWithPropertyTable()
        {
            return Enum.GetValues(typeof(NodeTypeEnum))
                       .Cast<NodeTypeEnum>()
                       .Where(x => GetHasPropertyTable(x) == 1)
                       .ToList();
        }

        // 判断节点是否有属性表
        public static int GetHasPropertyTable(NodeTypeEnum nodeType)
        {
            switch (nodeType)
            {
                case NodeTypeEnum.NODE_TYPE_CONDITIONS:
                case NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS:
                case NodeTypeEnum.NODE_TYPE_COPY:
                    return 1; // 有属性表
                default:
                    return 0; // 没有属性表
            }
        }

        // 根据编号获取节点类型
        public static NodeTypeEnum? GetNodeTypeEnumByCode(int code)
        {
            var nodeTypeEnums = GetNodeTypesWithPropertyTable();

            return nodeTypeEnums.FirstOrDefault(x => (int)x == code);
        }
    }
