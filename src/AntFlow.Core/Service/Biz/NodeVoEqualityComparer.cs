using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Business;

/// <summary>
///     ?¡ã·Úlinq??distinctby????,???????·Ú,??????
/// </summary>
public class NodeVoEqualityComparer : EqualityComparer<BpmnNodeVo>
{
    public override bool Equals(BpmnNodeVo? x, BpmnNodeVo? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.NodeId == y.NodeId;
    }

    public override int GetHashCode(BpmnNodeVo obj)
    {
        return obj.NodeId.GetHashCode();
    }
}