namespace antflowcore.constant.enus;

using System;
using System.Collections.Generic;
using System.Linq;

    public enum NodeTypeEnum
    {
        NODE_TYPE_START = 1,           // 发起人节点
        NODE_TYPE_GATEWAY = 2,         // 网关节点
        NODE_TYPE_CONDITIONS = 3,      // 条件节点
        NODE_TYPE_APPROVER = 4,        // 审批人节点
        NODE_TYPE_OUT_SIDE_CONDITIONS = 5, // 接入方条件节点
        NODE_TYPE_COPY = 6,            // 抄送节点
        NODE_TYPE_PARALLEL_GATEWAY = 7 // 并行网关
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

