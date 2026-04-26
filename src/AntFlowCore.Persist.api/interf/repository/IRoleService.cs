using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IRoleService : IAntFlowRepositoryMix<Role, IRoleRepository>
{
    List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<string> roleIds);
    List<BaseIdTranStruVo> QuerySassUserByRoleIds(ICollection<String> roleIds);
}
