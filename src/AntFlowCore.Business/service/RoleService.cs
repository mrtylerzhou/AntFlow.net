using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class RoleService : IRoleService
{
    public RoleService(IRoleRepository repository)
    {
        _repository = repository;
    }

    public IRoleRepository _repository { get; }

    public List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<string> roleIds)
    {
        return _repository.QueryUserByRoleIds(roleIds);
    }

    public List<BaseIdTranStruVo> QuerySassUserByRoleIds(ICollection<String> roleIds)
    {
        throw new NotImplementedException("not implement yet");
    }
}
