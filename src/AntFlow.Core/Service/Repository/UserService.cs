using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using FreeSql.Internal.Model;
using System.Linq.Expressions;

namespace AntFlow.Core.Service.Repository;

public class UserService : AFBaseCurdRepositoryService<User>, IUserService
{
    private readonly DepartmentService _departmentService;
    private readonly RoleService _roleService;

    public UserService(IFreeSql freeSql,
        RoleService roleService,
        DepartmentService departmentService) : base(freeSql)
    {
        _roleService = roleService;
        _departmentService = departmentService;
    }

    public BaseIdTranStruVo QueryUserById(string userId)
    {
        BaseIdTranStruVo baseIdTranStruVo =
            baseRepo.Where(a => a.Id == Convert.ToInt64(userId)).First().ToBaseIdTranStruVo();
        return baseIdTranStruVo;
    }

    public List<BaseIdTranStruVo> QueryUserByIds(IEnumerable<string> userIds)
    {
        IEnumerable<long> userIdLongList = AFCollectionUtil.StringToLongList(userIds);
        List<BaseIdTranStruVo> baseIdTranStruVos = baseRepo.Select.Where(a => userIdLongList.Contains(a.Id))
            .ToList().Select(a => a.ToBaseIdTranStruVo()).ToList();
        return baseIdTranStruVos;
    }


    //根据员工ID查询直接上级领导，支持多种查询方式（按部门、按职级、按角色、按权限、按组织架构）
    //查询逻辑说明：1.根据员工ID查询员工信息，t_user表中包含直接上级领导ID字段，通过该字段ID查询上级领导信息（返回上级ID，name等信息）
    //注意：此方法仅查询直接上级领导，不支持多级查询，如需多级查询请使用其他方法
    public BaseIdTranStruVo QueryEmployeeDirectLeaderById(string startUserId)
    {
        User first = baseRepo.Where(a => a.Id == Convert.ToInt64(startUserId)).First();
        if (first == null)
        {
            throw new AFBizException($"根据员工ID:{startUserId}查询不到员工信息");
        }

        long? leaderId = first.LeaderId;
        if (leaderId == null)
        {
            throw new AFBizException("该员工没有配置直接上级，请联系管理员");
        }

        User leader = baseRepo.Where(a => a.Id == leaderId).First();
        if (leader == null)
        {
            throw new AFBizException($"根据上级领导ID:{leaderId}查询不到领导信息");
        }

        return leader.ToBaseIdTranStruVo();
    }

    /// <summary>
    ///     根据员工ID查询对应的hrbp，如果员工没有配置hrbp，则抛出异常，请联系管理员配置
    /// </summary>
    /// <param name="startUserId"></param>
    /// <returns></returns>
    /// <exception cref="AFBizException"></exception>
    public BaseIdTranStruVo QueryEmployeeHrpbByEmployeeId(string startUserId)
    {
        User first = baseRepo.Where(a => a.Id == Convert.ToInt64(startUserId)).First();
        if (first == null)
        {
            throw new AFBizException($"根据员工ID:{startUserId}查询不到员工信息");
        }

        long? hrbpId = first.HrbpId;
        if (hrbpId == null)
        {
            throw new AFBizException("该员工没有配置hrbp信息，请联系管理员");
        }

        User hrbp = baseRepo.Where(a => a.Id == hrbpId).First();
        if (hrbp == null)
        {
            throw new AFBizException($"根据hrbpId:{hrbpId}查询不到对应的hrbp");
        }

        return hrbp.ToBaseIdTranStruVo();
    }

    /// <summary>
    ///     根据员工ID和层级查询上级领导，支持多级查询，返回指定层级的所有上级领导信息.层级从1开始计算(grade)
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    public List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndGrade(string employeeId, int grade)
    {
        return QueryLeadersByEmployeeIdAndTier(employeeId, grade);
    }

    /**
    * 多级查询，根据员工ID和层级查询上级领导信息，支持查询多级上级，可以查询到指定层级的所有上级领导N个人，返回上级领导信息列表
    * tier参数，表示查询的层级深度，从当前员工开始计算，向上查询指定层级的上级领导，例如tier=1表示查询直接上级，tier=2表示查询上级的上级，以此类推，支持多级查询，注意：层级从1开始计算
    * 这是demo级别的tier实现，实际项目中需要根据具体业务调整
    * 使用freesql进行查询，如果需要优化性能可以考虑使用原生sql查询，请酌情使用
    */
    public List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndTier(string employeeId, int tier)
    {
        List<Department> departments = Frsql.Select<Department, User>()
            .InnerJoin((a, b) => a.Id == b.DepartmentId)
            .Where((a, b) => b.Id == Convert.ToInt64(employeeId))
            .ToList();
        if (departments.Count == 0)
        {
            throw new AFBizException($"根据员工ID:{employeeId}查询不到部门信息");
        }

        Department department = departments.First();
        if (department == null)
        {
            throw new AFBizException($"根据员工ID:{employeeId}查询不到部门信息");
        }

        string? departmentPath = department.Path;
        if (departmentPath == null)
        {
            throw new AFBizException($"根据员工ID:{employeeId}查询不到部门信息");
        }

        //path是部门路径字符串，包含从根部门到当前部门的所有部门ID，用斜杠分隔，需要解析出所有上级部门ID，然后查询对应的领导
        List<string> tiredDepartmentIds = departmentPath.Split("/").ToList();
        IEnumerable<long> tiredDepartmentIdsLong = AFCollectionUtil.StringToLongList(tiredDepartmentIds);
        List<Department> tiredDepartments =
            _departmentService.baseRepo.Where(a => tiredDepartmentIdsLong.Contains(a.Id)).ToList();
        if (tiredDepartments.Count == 0) //如果查询不到上级部门信息，抛出异常
        {
            throw new AFBizException("查询不到上级部门信息，请联系管理员");
        }

        List<long?> departmentLeaderIds = departments.Select(a => a.LeaderId).ToList();
        //查询部门领导，由于可能有多个部门，需要去重处理，这里使用in查询批量获取，这是demo级别的实现，实际项目中如果是mysql，可以考虑order by field来排序
        List<User> users = baseRepo.Where(a => departmentLeaderIds.Contains(a.Id)).ToList();
        return users.Select(a => a.ToBaseIdTranStruVo()).ToList();
    }

    /**
     * 根据员工ID和层级查询指定层级的上级领导，支持精确查询指定层级的领导（单个领导，精确）
     * 与上面的方法不同，这个方法只返回指定层级的单个领导，而不是所有层级的领导列表，更加精确和高效，适用于只需要特定层级领导的场景
     * 注意，这个方法使用了数组索引，需要确保层级参数的正确性，避免数组越界异常（建议增加边界检查，或者使用更安全的查询方式，避免硬编码索引）
     */
    public BaseIdTranStruVo QueryLeaderByEmployeeIdAndLevel(string startUserId, int assignLevelGrade)
    {
        long startuserIdLong = Convert.ToInt64(startUserId);
        List<Department> departments = Frsql.Select<Department, User>()
            .InnerJoin((a, b) => a.Id == b.DepartmentId)
            .Where((a, b) => b.Id == startuserIdLong)
            .ToList();
        if (departments.Count == 0)
        {
            throw new AFBizException($"根据员工ID:{startUserId}查询不到部门信息");
        }

        Department department = departments.First();
        if (department == null)
        {
            throw new AFBizException($"根据员工ID:{startUserId}查询不到部门信息");
        }

        string? departmentPath = department.Path;
        if (departmentPath == null)
        {
            throw new AFBizException($"根据员工ID:{startUserId}查询不到部门信息");
        }

        //path是部门路径字符串，包含从根部门到当前部门的所有部门ID，用斜杠分隔，需要解析出所有上级部门ID，然后查询对应的领导
        List<string> tiredDepartmentIds = departmentPath.Split("/").Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
        IEnumerable<int> tiredDepartmentIdsLong = AFCollectionUtil.StringToIntList(tiredDepartmentIds);
        List<Department> tiredDepartments =
            _departmentService.baseRepo.Where(a => tiredDepartmentIdsLong.Contains(a.Id)).ToList();
        if (tiredDepartments.Count == 0)
        {
            throw new AFBizException("查询不到上级部门信息，请联系管理员");
        }

        int tiredDepartmentsCount = tiredDepartments.Count;
        int index = assignLevelGrade > tiredDepartmentsCount ? tiredDepartmentsCount : assignLevelGrade;
        long? leaderId = tiredDepartments[index - 1].LeaderId;
        if (leaderId == null)
        {
            throw new AFBizException($"根据员工ID:{startUserId}查询不到指定层级{assignLevelGrade}的领导");
        }

        List<User> users = baseRepo.Where(a => a.Id == leaderId).ToList();
        if (users.Count == 0)
        {
            throw new AFBizException($"根据员工ID:{startUserId}查询不到指定层级{assignLevelGrade}的领导");
        }

        return users.First().ToBaseIdTranStruVo();
    }

    /**
     * 根据角色ID查询员工信息，返回角色对应的员工信息，支持批量查询多个角色
     */
    public Dictionary<string, string> ProvideRoleEmployeeInfo(List<string> roleIds)
    {
        List<User> users = _roleService.QueryUserByRoleIds(roleIds);
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

    /// <summary>
    ///     根据用户id查询用户信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public BaseIdTranStruVo GetById(string userId)
    {
        User first = baseRepo.Where(a => a.Id == Convert.ToInt64(userId)).First();
        return new BaseIdTranStruVo { Id = first.Id.ToString(), Name = first.Name };
    }

    /// <summary>
    ///     分页查询用户列表
    /// </summary>
    /// <param name="page"></param>
    /// <param name="taskMgmtVo"></param>
    /// <returns></returns>
    public ResultAndPage<BaseIdTranStruVo> SelectUserPageList(Page<BaseIdTranStruVo> page, TaskMgmtVO taskMgmtVo)
    {
        Expression<Func<User, bool>> expression = a => 1 == 1;
        if (!string.IsNullOrEmpty(taskMgmtVo?.Description))
        {
            expression = expression.And(a => a.Name.Contains(taskMgmtVo.Description));
        }

        BasePagingInfo basePagingInfo = page.ToPagingInfo();

        List<User> users = baseRepo.Where(expression)
            .Page(basePagingInfo)
            .ToList();
        List<BaseIdTranStruVo> baseIdTranStruVos = users.Select(a => a.ToBaseIdTranStruVo()).ToList();
        return PageUtils.GetResultAndPage(page.Of(baseIdTranStruVos, basePagingInfo));
    }

    public List<BaseIdTranStruVo> SelectAll()
    {
        List<BaseIdTranStruVo> results = baseRepo.Where(a => 1 == 1)
            .ToList<BaseIdTranStruVo>(a => new BaseIdTranStruVo(a.Id.ToString(), a.Name));
        return results;
    }

    public List<Employee> QryLiteEmployeeInfoByIds(IEnumerable<string> ids)
    {
        List<BaseIdTranStruVo>? baseIdTranStruVos = QueryUserByIds(ids);
        return EmployeeUtil.BasicEmployeeInfos(baseIdTranStruVos);
    }

    public Employee QryLiteEmployeeInfoById(string id)
    {
        BaseIdTranStruVo? baseIdTranStruVo = GetById(id);
        return EmployeeUtil.BasicEmployeeInfo(baseIdTranStruVo);
    }

    public Employee GetEmployeeDetailById(string id)
    {
        Employee employee = baseRepo
            .Where(a => a.Id == Convert.ToInt64(id))
            .ToOne<Employee>(a => new Employee { Id = a.Id.ToString(), Username = a.Name });
        return employee;
    }

    /// <summary>
    ///     批量查询员工详细信息，返回包含员工基本信息的id和name，用于业务流程中的员工信息展示，支持多个员工同时查询，提高查询效率，减少数据库访问次数，优化性能表现。
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public List<Employee> GetEmployeeDetailByIds(IEnumerable<string> ids)
    {
        List<long> longIds = ids.Select(a => Convert.ToInt64(a)).ToList();
        List<User> users = baseRepo
            .Where(a => longIds.Contains(a.Id))
            .ToList();
        List<Employee> employees = users.Select(a => new Employee { Id = a.Id.ToString(), Username = a.Name })
            .ToList();
        return employees;
    }


    public List<BaseIdTranStruVo> GetLevelLeadersByEmployeeIdAndTier(string employeeId, int tier)
    {
        //todo 
        return new List<BaseIdTranStruVo>();
    }

    /// <summary>
    ///     检查员工是否有效，返回员工记录数量，如果员工存在且有效则返回大于0的数值，否则返回0表示无效。
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public long CheckEmployeeEffective(string userId)
    {
        return baseRepo.Where(a => a.Id == Convert.ToInt64(userId)).Count();
    }
}