using antflowcore.vo;

namespace antflowcore.service.biz;

public class BaseIdStructVoComparer : EqualityComparer<BaseIdTranStruVo>
{
    public override bool Equals(BaseIdTranStruVo? x, BaseIdTranStruVo? y)
    {
        if (x == null || y == null)
        {
            return false;
        }
        return x.Id==y.Id;
    }

    public override int GetHashCode(BaseIdTranStruVo obj)
    {
        return obj.Id.GetHashCode();
    }
}