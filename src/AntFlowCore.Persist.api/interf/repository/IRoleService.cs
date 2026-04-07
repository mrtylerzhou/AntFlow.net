using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IRoleService : IBaseRepositoryService<Role>
{
    List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<string> roleIds);
    public List<BaseIdTranStruVo> QuerySassUserByRoleIds(ICollection<String> roleIds)
    {
        throw new NotImplementedException("not implement yet");
    }
}
