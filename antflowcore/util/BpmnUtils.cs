using System.Collections.ObjectModel;
using antflowcore.vo;

namespace antflowcore.util;

public class BpmnUtils {
    public static BpmnNodeVo GetAggregationNode(BpmnNodeVo parallelNode, IEnumerable<BpmnNodeVo> bpmnNodeVos){
        String parallelNodeNodeId = parallelNode.NodeId;
        List<String> parallelNodeNodeTo = parallelNode.NodeTo;
        foreach (BpmnNodeVo bpmnNodeVo in bpmnNodeVos) {
            if(bpmnNodeVo.NodeFrom.Equals(parallelNodeNodeId)&& !parallelNodeNodeTo.Contains(bpmnNodeVo.NodeId)){
                return bpmnNodeVo;
            }
        }
        return null;
    }
}
