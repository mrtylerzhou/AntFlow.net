using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.util;

public class AfNodeUtils
{
    public static void AddOrEditProperty(BpmnNodeVo bpmnNodeVo, Action<BpmnNodePropertysVo> action)
    {
        if (bpmnNodeVo.Property == null)
        {
            bpmnNodeVo.Property = new BpmnNodePropertysVo();
        }

        action(bpmnNodeVo.Property);
    }

    /// <summary>
    /// 设计期节点类型特殊处理.
    /// 自动节点(nodeType=9)本质上是一个审批人节点(nodeType=4),
    /// 带有 automaticNode 标签和虚拟指派人 AUTO_NODE_SKIP(-3).
    /// 在保存前将 nodeType=9 转换为 nodeType=4,并设置默认属性.
    /// 对应 Java NodeUtil.nodeSpecialProcess.
    /// </summary>
    public static void NodeSpecialProcess(BpmnNodeVo bpmnNodeVo)
    {
        int? nodeType = bpmnNodeVo.NodeType;
        if (nodeType == null)
        {
            return;
        }

        // 抄送节点V2: 转换为审批人节点并标记为抄送节点
        if (nodeType == (int)NodeTypeEnum.NODE_TYPE_COPY_V2)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsCarbonCopyNode = true;
        }

        // 自动节点: 转换为审批人节点,设置虚拟指派人
        if (nodeType == (int)NodeTypeEnum.NODE_TYPE_AUTO_NODE)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsAutomaticNode = true;
            bpmnNodeVo.NodeProperty = (int)NodePropertyEnum.NODE_PROPERTY_PERSONNEL;
            BpmnNodePropertysVo prop = bpmnNodeVo.Property;
            if (prop == null)
            {
                prop = new BpmnNodePropertysVo();
                bpmnNodeVo.Property = prop;
            }
            if (prop.SignType == null)
            {
                prop.SignType = 1;
            }
            if (prop.EmplIds == null || prop.EmplIds.Count == 0)
            {
                prop.EmplIds = new List<string> { AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id };
            }
            if (prop.EmplList == null || prop.EmplList.Count == 0)
            {
                BaseIdTranStruVo virtualUser = new BaseIdTranStruVo(
                    AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id,
                    AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc);
                prop.EmplList = new List<BaseIdTranStruVo> { virtualUser };
            }
        }
    }

    /// <summary>
    /// 读取期节点标签特殊处理.
    /// 根据标签将 nodeType 还原为前端识别的类型:
    /// - automaticNode 标签 -> nodeType=9 (自动节点)
    /// - copyNodeV2 标签 -> nodeType=8 (抄送节点V2)
    /// 对应 Java NodeUtil.nodeLabelSpecialProcess.
    /// </summary>
    public static void NodeLabelSpecialProcess(BpmnNodeVo bpmnNodeVo)
    {
        List<BpmnNodeLabelVO> labelList = bpmnNodeVo.LabelList;
        if (labelList == null || labelList.Count == 0)
        {
            return;
        }
        foreach (var nodeLabelVO in labelList)
        {
            if (NodeLabelConstants.CopyNodeV2.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_COPY_V2;
            }
            if (NodeLabelConstants.AutomaticNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_AUTO_NODE;
            }
        }
    }
}
