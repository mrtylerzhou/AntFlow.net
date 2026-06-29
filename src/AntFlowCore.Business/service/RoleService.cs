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

    public List<BaseIdTranStruVo> GetAllRoles()
    { 
        List<BaseIdTranStruVo> results = _repository.GetQueryable()
             .Where(a => 1 == 1)
             .ToList()
             .Select(a => new BaseIdTranStruVo(a.Id.ToString(), a.RoleName))
             .ToList();
        return results;
    }
}
