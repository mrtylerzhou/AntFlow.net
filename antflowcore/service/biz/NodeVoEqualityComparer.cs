using antflowcore.vo;

namespace antflowcore.service.biz;

/// <summary>
/// 新版本linq有distinctby方法,为了兼容旧版本,自己实现
/// </summary>
public class NodeVoEqualityComparer: EqualityComparer<BpmnNodeVo>
{
    public override bool Equals(BpmnNodeVo? x, BpmnNodeVo? y)
    {
        if (x == null || y == null)
        {
            return false;
        }
        return x.NodeId==y.NodeId;
    }

    public override int GetHashCode(BpmnNodeVo obj)
    {
      return obj.NodeId.GetHashCode();
    }
}