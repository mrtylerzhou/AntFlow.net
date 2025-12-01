using antflowcore.vo;

namespace antflowcore.service.interf.repository;

public interface IRoleService
{
    List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<String> roleIds);
}