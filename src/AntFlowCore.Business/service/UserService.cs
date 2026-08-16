using System.Linq.Expressions;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class UserService : IUserService
{
    private readonly IRoleService _roleService;
    private readonly IDepartmentService _departmentService;

    public UserService(
        IUserRepository repository,
        IRoleService roleService,
        IDepartmentService departmentService
    )
    {
        _repository = repository;
        _roleService = roleService;
        _departmentService = departmentService;
    }

    public IUserRepository _repository { get; }

    public List<BaseIdTranStruVo> QueryUserByNameFuzzy(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return new List<BaseIdTranStruVo>();
        }
        return _repository.GetQueryable()
            .Where(a => a.Name.Contains(userName))
            .ToList()
            .Select(a => a.ToBaseIdTranStruVo())
            .ToList();
    }

    public BaseIdTranStruVo QueryUserById(string userId)
    {
        BaseIdTranStruVo baseIdTranStruVo = _repository.Find(a => a.Id == Convert.ToInt64(userId)).FirstOrDefault().ToBaseIdTranStruVo();
        return baseIdTranStruVo;
    }

    public List<BaseIdTranStruVo> QueryUserByIds(IEnumerable<String> userIds)
    {
        IEnumerable<long> userIdLongList = AFCollectionUtil.StringToLongList(userIds);
        List<BaseIdTranStruVo> baseIdTranStruVos = _repository.GetQueryable()
            .Where(a => userIdLongList.Contains(a.Id))
            .ToList()
            .Select(a => a.ToBaseIdTranStruVo())
            .ToList();
        return baseIdTranStruVos;
    }

    public List<BaseIdTranStruVo> QueryEmployeeDirectLeaderByIds(IEnumerable<string> userIds)
    {
        List<User> usersByIds = _repository.GetQueryable()
            .Where(a => userIds.Select(b => Convert.ToInt64(b)).Contains(a.Id))
            .ToList();
        if (usersByIds.IsEmpty())
        {
            throw new AFBizException($"未能根据人员Id:{userIds}找到人员信息");
        }

        List<long?> leaderIdsByEmplIds = usersByIds.Select(a => a.LeaderId).ToList();

        if (leaderIdsByEmplIds.IsEmpty())
        {
            throw new AFBizException("发起人没有直属领导信息,请检查人员信息");
        }

        List<User> leaders = _repository.GetQueryable()
            .Where(a => leaderIdsByEmplIds.Contains(a.Id))
            .ToList();
        if (leaders.IsEmpty())
        {
            throw new AFBizException($"未能根据人员直属领导Id:{leaderIdsByEmplIds}找到人员直属领导");
        }
        return leaders.Select(a => a.ToBaseIdTranStruVo()).ToList();
    }

    public List<BaseIdTranStruVo> QueryEmployeeHrpbsByEmployeeIds(IEnumerable<string> userIds)
    {
        List<User> usersByIds = _repository.GetQueryable()
            .Where(a => userIds.Select(b => Convert.ToInt64(b)).Contains(a.Id))
            .ToList();
        if (usersByIds.IsEmpty())
        {
            throw new AFBizException($"未能根据人员Id:{userIds}找到人员信息");
        }

        List<long?> hrbpIdsByEmplIds = usersByIds.Select(a => a.HrbpId).ToList();

        if (hrbpIdsByEmplIds.IsEmpty())
        {
            throw new AFBizException("发起人没有hrbp信息,请检查人员信息");
        }

        List<User> hrbpList = _repository.GetQueryable()
            .Where(a => hrbpIdsByEmplIds.Contains(a.Id))
            .ToList();
        if (hrbpList.IsEmpty())
        {
            throw new AFBizException($"未能根据人员直属领导Id:{hrbpIdsByEmplIds}找到人员直属领导");
        }
        return hrbpList.Select(a => a.ToBaseIdTranStruVo()).ToList();
    }

    public List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndGrade(String employeeId, int grade)
    {
        return QueryLeadersByEmployeeIdAndTier(employeeId, grade);
    }

    public List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndTier(String employeeId, int tier)
    {
        List<Department> departments = _repository.QueryDepartmentAndUserByUserId(Convert.ToInt64(employeeId));
        if (departments.Count == 0)
        {
            throw new AFBizException($"未能根据用户Id:{employeeId}找到用户所在组织");
        }
        Department department = departments.First();
        if (department == null)
        {
            throw new AFBizException($"未能根据用户Id:{employeeId}找到用户所在组织");
        }

        string? departmentPath = department.Path;
        if (departmentPath == null)
        {
            throw new AFBizException($"未能根据用户Id:{employeeId}找到用户所在组织");
        }
        List<string> tiredDepartmentIds = departmentPath.Split("/").ToList();
        IEnumerable<long> tiredDepartmentIdsLong = AFCollectionUtil.StringToLongList(tiredDepartmentIds);
        List<Department> tiredDepartments = _departmentService._repository
            .Find(a => tiredDepartmentIdsLong.Contains(a.Id))
            .ToList();
        if (tiredDepartments.Count == 0)
        {
            throw new AFBizException("用户所在组织线上没有领导,请检查用户信息");
        }

        List<long?> departmentLeaderIds = departments.Select(a => a.LeaderId).ToList();
        List<User> users = _repository.GetQueryable()
            .Where(a => departmentLeaderIds.Contains(a.Id))
            .ToList();
        return users.Select(a => a.ToBaseIdTranStruVo()).ToList();
    }

    public BaseIdTranStruVo QueryLeaderByEmployeeIdAndLevel(string startUserId, int assignLevelGrade)
    {
        long startuserIdLong = Convert.ToInt64(startUserId);
        List<Department> departments = _repository.QueryDepartmentAndUserByUserId(startuserIdLong);
        if (departments.Count == 0)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织");
        }
        Department department = departments.First();
        if (department == null)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织");
        }

        string? departmentPath = department.Path;
        if (departmentPath == null)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织");
        }
        List<string> tiredDepartmentIds = departmentPath.Split("/").Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
        IEnumerable<int> tiredDepartmentIdsLong = AFCollectionUtil.StringToIntList(tiredDepartmentIds);
        List<Department> tiredDepartments = _departmentService._repository
            .Find(a => tiredDepartmentIdsLong.Contains(a.Id))
            .ToList();
        if (tiredDepartments.Count == 0)
        {
            throw new AFBizException($"用户所在组织线上没有领导,请检查用户信息");
        }

        int tiredDepartmentsCount = tiredDepartments.Count;
        int index = assignLevelGrade > tiredDepartmentsCount ? tiredDepartmentsCount : assignLevelGrade;
        long? leaderId = tiredDepartments[index - 1].LeaderId;
        if (leaderId == null)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织线上第{assignLevelGrade}级领导");
        }

        List<User> users = _repository.GetQueryable().Where(a => a.Id == leaderId).ToList();
        if (users.Count == 0)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织线上第{assignLevelGrade}级领导");
        }
        return users.First().ToBaseIdTranStruVo();
    }

    public Dictionary<string, string> ProvideRoleEmployeeInfo(List<string> roleIds)
    {
        List<BaseIdTranStruVo> users = _roleService.QueryUserByRoleIds(roleIds);
        if (users == null || users.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        return users
            .ToLookup(a => a.Id.ToString())
            .ToDictionary(
            a => a.Key,
            a => a.First().Name,
            StringComparer.OrdinalIgnoreCase);
    }

    public BaseIdTranStruVo GetById(string userId)
    {
        User first = _repository.Find(a => a.Id == Convert.ToInt64(userId)).FirstOrDefault();
        return new BaseIdTranStruVo { Id = first.Id.ToString(), Name = first.Name };
    }

    public ResultAndPage<BaseIdTranStruVo> SelectUserPageList(Page<BaseIdTranStruVo> page, TaskMgmtVO taskMgmtVo)
    {
        Expression<Func<User, bool>> expression = a => 1 == 1;
        if (!string.IsNullOrEmpty(taskMgmtVo?.Description))
        {
            expression =LinqExtensions.And(expression, a => a.Name.Contains(taskMgmtVo.Description));
        }

        PagingInfo pagingInfo = page.ToPagingInfo();
       
        List<User> users = _repository.QueryUserListByExpression(expression, pagingInfo);;
        List<BaseIdTranStruVo> baseIdTranStruVos = users.Select(a => a.ToBaseIdTranStruVo()).ToList();
        return PageUtils.GetResultAndPage(page.Of(baseIdTranStruVos, pagingInfo));
    }

    public List<BaseIdTranStruVo> SelectAll()
    {
        List<BaseIdTranStruVo> results = _repository.GetQueryable()
            .Where(a => 1 == 1)
            .ToList()
            .Select(a => new BaseIdTranStruVo(a.Id.ToString(), a.Name))
            .ToList();
        return results;
    }

    public DetailedUser GetEmployeeDetailById(string id)
    {
        var user = _repository.GetQueryable()
            .Where(a => a.Id == Convert.ToInt64(id))
            .FirstOrDefault();
        return new DetailedUser()
        {
            Id = user.Id.ToString(),
            UserName = user.Name,
            Email = user.Email,
            Mobile = user.Mobile,
            MobileIsShow = user.MobileIsShow ?? false,
        };
    }

    public List<DetailedUser> GetEmployeeDetailByIds(IEnumerable<string> ids)
    {
        List<long> longIds = ids.Select(a => Convert.ToInt64(a)).ToList();
        List<User> users = _repository.GetQueryable()
            .Where(a => longIds.Contains(a.Id))
            .ToList();
        List<DetailedUser> employees = users.Select(a => new DetailedUser
        {
            Id = a.Id.ToString(),
            UserName = a.Name,
            Email = a.Email,
            Mobile = a.Mobile,
            MobileIsShow = a.MobileIsShow ?? false,
        })
            .ToList();
        return employees;
    }

    public List<BaseIdTranStruVo> GetLevelLeadersByEmployeeIdAndTier(string employeeId, int tier)
    {
        return new List<BaseIdTranStruVo>();
    }

    /// <summary>
    /// 查询员工可用性(办公状态)骨架空实现:不查询任何表,默认恒返回"可用"。
    /// 真实场景由使用方自行实现(可联合查询员工表、工作日历表等,甚至调用三方接口),
    /// 只需保证返回结构符合 UserAvailableVo 契约即可。
    /// </summary>
    /// <param name="userId">用户id</param>
    public UserAvailableVo CheckEmployeeEffective(string userId)
    {
        // 骨架空实现:默认可用,不涉及转办。使用方按需覆盖此方法
        return new UserAvailableVo { Available = true };
    }

    public DetailedUser GetDetailedUserById(string Id)
    {
        User? firstOrDefault = _repository
            .GetQueryable()
            .FirstOrDefault(a => a.Id == Convert.ToInt64(Id));
        DetailedUser employee = new DetailedUser()
        {
            Id = firstOrDefault.Id.ToString(),
            UserName = firstOrDefault.Name,
            Email = firstOrDefault.Email,
            Mobile = firstOrDefault.Mobile,
            MobileIsShow = firstOrDefault.MobileIsShow ?? false,
        };
        return employee;
    }

    public List<BaseIdTranStruVo> GetUserRolesById(string userId)
    {
        if (!long.TryParse(userId, out long uid))
        {
            return new List<BaseIdTranStruVo>();
        }
        List<UserRole> userRoles = _repository.QueryUserRolesByUserId(uid);
        if (userRoles == null || userRoles.Count == 0)
        {
            return new List<BaseIdTranStruVo>();
        }
        List<string> roleIds = userRoles
            .Where(r => r.RoleId != null)
            .Select(r => r.RoleId!.Value.ToString())
            .Distinct()
            .ToList();
        if (roleIds.Count == 0)
        {
            return new List<BaseIdTranStruVo>();
        }
        return _roleService.QueryRoleByIds(roleIds);
    }

    public List<BaseIdTranStruVo> GetUserDepartmentsById(string userId)
    {
        if (!long.TryParse(userId, out long uid))
        {
            return new List<BaseIdTranStruVo>();
        }
        List<Department> departments = _repository.QueryDepartmentAndUserByUserId(uid);
        if (departments == null || departments.Count == 0)
        {
            return new List<BaseIdTranStruVo>();
        }
        //用户所在部门 + path 前缀子部门(去重)
        Dictionary<long, string> result = new();
        foreach (Department dept in departments)
        {
            result.TryAdd(dept.Id, dept.Name);
            if (!string.IsNullOrEmpty(dept.Path))
            {
                List<Department> children = _departmentService._repository
                    .Find(d => d.Path != null && d.Path.StartsWith(dept.Path));
                foreach (Department child in children)
                {
                    result.TryAdd(child.Id, child.Name);
                }
            }
        }
        return result.Select(kv => new BaseIdTranStruVo { Id = kv.Key.ToString(), Name = kv.Value }).ToList();
    }
}
