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

    public List<BaseIdTranStruVo> QueryRoleByNameFuzzy(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new List<BaseIdTranStruVo>();
        }
        return _repository.GetQueryable()
            .Where(a => a.RoleName.Contains(name))
            .ToList()
            .Select(a => new BaseIdTranStruVo(a.Id.ToString(), a.RoleName))
            .ToList();
    }

    public List<BaseIdTranStruVo> QueryRoleByIds(IEnumerable<string> roleIds)
    {
        List<long> ids = roleIds.Where(x => long.TryParse(x, out _)).Select(long.Parse).Distinct().ToList();
        if (ids.Count == 0)
        {
            return new List<BaseIdTranStruVo>();
        }
        return _repository.GetQueryable()
            .Where(a => ids.Contains(a.Id))
            .ToList()
            .Select(a => new BaseIdTranStruVo(a.Id.ToString(), a.RoleName))
            .ToList();
    }
}