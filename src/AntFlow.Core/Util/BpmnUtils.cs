using AntFlow.Core.Vo;

namespace AntFlow.Core.Util;

public class BpmnUtils
{
    public static BpmnNodeVo GetAggregationNode(BpmnNodeVo parallelNode, IEnumerable<BpmnNodeVo> bpmnNodeVos)
    {
        string parallelNodeNodeId = parallelNode.NodeId;
        List<string> parallelNodeNodeTo = parallelNode.NodeTo;
        foreach (BpmnNodeVo bpmnNodeVo in bpmnNodeVos)
        {
            if (bpmnNodeVo.NodeFrom.Equals(parallelNodeNodeId) && !parallelNodeNodeTo.Contains(bpmnNodeVo.NodeId))
            {
                return bpmnNodeVo;
            }
        }

        return null;
    }
}