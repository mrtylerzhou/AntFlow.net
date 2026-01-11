using antflowcore.vo;

namespace antflowcore.util;

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
}