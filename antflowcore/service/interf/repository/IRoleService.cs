using antflowcore.vo;

namespace antflowcore.service.interf.repository;

public interface IRoleService
{
    List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<String> roleIds);
    public List<BaseIdTranStruVo> QuerySassUserByRoleIds(ICollection<String> roleIds)
    {
        throw new NotImplementedException("not implement yet");
    }
}