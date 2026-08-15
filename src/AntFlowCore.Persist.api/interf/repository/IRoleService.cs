using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IRoleService : IAntFlowRepositoryMix<Role, IRoleRepository>
{
    List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<string> roleIds);
    List<BaseIdTranStruVo> QuerySassUserByRoleIds(ICollection<String> roleIds);
    List<BaseIdTranStruVo> GetAllRoles();
    /// <summary>
    /// 角色名称模糊查询(搜索下拉用)
    /// </summary>
    List<BaseIdTranStruVo> QueryRoleByNameFuzzy(string name);
    /// <summary>
    /// 按角色id集合批量查询
    /// </summary>
    List<BaseIdTranStruVo> QueryRoleByIds(IEnumerable<string> roleIds);
}
