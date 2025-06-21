using System.Linq.Expressions;
using antflowcore.dto;
using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.util;
using AntFlowCore.Util;
using antflowcore.vo;
using AntFlowCore.Vo;
using FreeSql.Internal.Model;

namespace antflowcore.service.repository;

public class UserService: AFBaseCurdRepositoryService<User>
{
    private readonly RoleService _roleService;
    private readonly DepartmentService _departmentService;

    public UserService(IFreeSql freeSql,
        RoleService roleService,
        DepartmentService departmentService) : base(freeSql)
    {
        _roleService = roleService;
        _departmentService = departmentService;
    }

    public BaseIdTranStruVo QueryUserById(string userId)
    {
        BaseIdTranStruVo baseIdTranStruVo = baseRepo.Where(a=>a.Id==Convert.ToInt64(userId)).First().ToBaseIdTranStruVo();
        return baseIdTranStruVo;
    }
   public List<BaseIdTranStruVo> QueryUserByIds(IEnumerable<String> userIds)
    {
        IEnumerable<long> userIdLongList = AFCollectionUtil.StringToLongList(userIds);
        List<BaseIdTranStruVo> baseIdTranStruVos = baseRepo.Select.Where(a => userIdLongList.Contains(a.Id))
            .ToList().Select(a => a.ToBaseIdTranStruVo()).ToList();
        return baseIdTranStruVos;
    }
   
   

    //根据发起人查找发起人的直属领导,用户实现时不用关心函数参数哪里来的(模块太多,一口吃不了胖子,先把功能实现,用户起来,然后再逐步搞清楚)
    //这里面逻辑是:1.根据发起人Id找到发起人,t_user表里面发起人记录里面包含了发起人的直属领导的Id,然后再根据直属领导Id找到直属领导员工信息(实际上只需要Id,name两个字段)
    //实际上,用户的组织架构系统用户的直属领导可能不是这样设计的,这里只是demo, 最终只要根据发起人id拿到他的领导信息即可
    public BaseIdTranStruVo QueryEmployeeDirectLeaderById(string startUserId)
    {
        User first = baseRepo.Where(a=>a.Id==Convert.ToInt64(startUserId)).First();
        if(first==null)
        {
            throw new AFBizException($"未能根据发起人Id:{startUserId}找到发起人");
        }

        long? leaderId = first.LeaderId;
        if (leaderId == null)
        {
            throw new AFBizException("发起人没有直属领导信息,请检查发起人信息");
        }

        User leader = baseRepo.Where(a => a.Id == leaderId).First();
        if(leader==null)
        {
            throw new AFBizException($"未能根据发起人直属领导Id:{leaderId}找到发起人直属领导");
        }
        return leader.ToBaseIdTranStruVo();
    }

    
    public BaseIdTranStruVo QueryEmployeeHrpbByEmployeeId(string startUserId)
    {
        User first = baseRepo.Where(a=>a.Id==Convert.ToInt64(startUserId)).First();
        if(first==null)
        {
            throw new AFBizException($"未能根据发起人Id:{startUserId}找到发起人");
        }

        long? hrbpId = first.HrbpId;
        if (hrbpId == null)
        {
            throw new AFBizException("发起人没有hrbp信息,请检查发起人信息");
        }

        User hrbp = baseRepo.Where(a=>a.Id==hrbpId).First();
        if(hrbp==null)
        {
            throw new AFBizException($"未能根据发起人hrbpId:{hrbpId}找到发起人hrbp");
        }
        return hrbp.ToBaseIdTranStruVo();
    }

    public  List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndGrade(String employeeId,int grade)
    {
       return QueryLeadersByEmployeeIdAndTier(employeeId, grade);
    }
    /**
    * 层层审批,此方法根据用户Id找到用户所在组织线上的一层层的负责人,角色也是返回一个集合,不同的是层层审批是根据返回的人生成了N个节点,角色审批所有的角色人物都在一个节点上
    * tier层级,有时候不需要用户所在组织线上所有领导都审批,可以只向上一层或者两层,实际上都是自定义的一些属性,用于控制特殊逻辑,用户也可以自定义一些其它的属性,实现原理就是在设计是增加属性,然后存表里,发起流程时读取出来
    * 这里demo忽略tier属性,即默认组织线上所有领导都审批
    * 作者freesql水平有限,暂无法将整个个逻辑用一条sql写出,仅作展示逻辑
    */
    public  List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndTier(String employeeId,int tier)
    {
        List<Department> departments = Frsql.Select<Department,User>()
            .InnerJoin((a, b) => a.Id == b.DepartmentId)
            .Where((a,b)=>b.Id==Convert.ToInt64(employeeId))
            .ToList();
        if(departments.Count==0)
        {
            throw new AFBizException($"未能根据用户Id:{employeeId}找到用户所在组织");
        }
        Department department = departments.First();
        if(department==null)
        {
            throw new AFBizException($"未能根据用户Id:{employeeId}找到用户所在组织");
        }

        string? departmentPath = department.Path;
        if(departmentPath==null)
        {
            throw new AFBizException($"未能根据用户Id:{employeeId}找到用户所在组织");
        }
        //path即为用户的部门路径,当然实际中用户的系统并不是这样设计的,根据实际情况查出来用户层级部门信息即可,这里和流程核心引擎逻辑没关系,纯业务
        List<string> tiredDepartmentIds = departmentPath.Split("/").ToList();
        IEnumerable<long> tiredDepartmentIdsLong = AFCollectionUtil.StringToLongList(tiredDepartmentIds);
        List<Department> tiredDepartments = _departmentService.baseRepo.Where(a=>tiredDepartmentIdsLong.Contains(a.Id)).ToList();
        if(tiredDepartments.Count==0)  //如果用户所在组织线上没有领导,则直接返回空
        {
            throw new AFBizException($"用户所在组织线上没有领导,请检查用户信息");
        }

        List<long?> departmentLeaderIds = departments.Select(a=>a.LeaderId).ToList();
        //这里需要注意,用户的领导是有层级的,审批时是有顺序的,但是这里in查询并不能保证顺序,这里demo就不处理了,如果是直接使用mysql,可以加上order by field保持顺序
        List<User> users = this.baseRepo.Where(a=>departmentLeaderIds.Contains(a.Id)).ToList();
        return users.Select(a=>a.ToBaseIdTranStruVo()).ToList();
    }
    /**
     * 查找指定层级的领导,直属领导相当于找当前层级的领导,有时候可能需要找用户所在当前组织上一级(或者两级,三级)领导
     * 有时候会有这样情况,比如找当前用户上三级领导,但是有些部门层级很浅,向上没有三层,这时候要怎么处理就要和产品来沟通一下了,比如层数不够,就找最高的层级
     * 总之,这里都和特定业务有关,用户要结合自己组织架构来设计和业务特点来写sql实现(当然也可以是调http接口,所有审批人规则的最终目标都是找到审批人,怎么拿到的引擎不关心,只约定返回结果的格式)
     */
    public BaseIdTranStruVo QueryLeaderByEmployeeIdAndLevel(string startUserId, int assignLevelGrade)
    {
        long startuserIdLong = Convert.ToInt64(startUserId);
        List<Department> departments = Frsql.Select<Department,User>()
            .InnerJoin((a, b) => a.Id == b.DepartmentId)
            .Where((a,b)=>b.Id==startuserIdLong)
            .ToList();
        if(departments.Count==0)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织");
        }
        Department department = departments.First();
        if(department==null)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织");
        }

        string? departmentPath = department.Path;
        if(departmentPath==null)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织");
        }
        //path即为用户的部门路径,当然实际中用户的系统并不是这样设计的,根据实际情况查出来用户层级部门信息即可,这里和流程核心引擎逻辑没关系,纯业务
        List<string> tiredDepartmentIds = departmentPath.Split("/").Where(a=>!string.IsNullOrWhiteSpace(a)).ToList();
        IEnumerable<int> tiredDepartmentIdsLong = AFCollectionUtil.StringToIntList(tiredDepartmentIds);
        List<Department> tiredDepartments = _departmentService.baseRepo.Where(a=>tiredDepartmentIdsLong.Contains(a.Id)).ToList();
        if (tiredDepartments.Count == 0)
        {
            throw new AFBizException($"用户所在组织线上没有领导,请检查用户信息");
        }

        int tiredDepartmentsCount = tiredDepartments.Count;
        int index=assignLevelGrade>tiredDepartmentsCount?tiredDepartmentsCount:assignLevelGrade;
        long? leaderId = tiredDepartments[index-1].LeaderId;
        if(leaderId==null)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织线上第{assignLevelGrade}级领导");
        }

        List<User> users = this.baseRepo.Where(a=>a.Id==leaderId).ToList();
        if(users.Count==0)
        {
            throw new AFBizException($"未能根据用户Id:{startUserId}找到用户所在组织线上第{assignLevelGrade}级领导");
        }
        return users.First().ToBaseIdTranStruVo();
    }

    /**
     * 根据角色Id获取用户信息,用户可以替换为自己的表,根据角色找到用户信息即可
     */
    public Dictionary<string,string> ProvideRoleEmployeeInfo(List<string> roleIds)
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

    public BaseIdTranStruVo GetById(string userId)
    {
        User first = baseRepo.Where(a=>a.Id==Convert.ToInt64(userId)).First();
        return new BaseIdTranStruVo{Id = first.Id.ToString(),Name = first.Name};
    }

    public  ResultAndPage<BaseIdTranStruVo> SelectUserPageList(Page<BaseIdTranStruVo> page, TaskMgmtVO taskMgmtVo)
    {
        Expression<Func<User, bool>> expression = a => 1 == 1;
        if (!string.IsNullOrEmpty(taskMgmtVo?.Description))
        {
            expression=expression.And(a=>a.Name.Contains(taskMgmtVo.Description));
        }

        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        
        List<User> users = baseRepo.Where(expression)
            .Page(basePagingInfo)
            .ToList();
        List<BaseIdTranStruVo> baseIdTranStruVos = users.Select(a=>a.ToBaseIdTranStruVo()).ToList();
        return PageUtils.GetResultAndPage(page.Of(baseIdTranStruVos, basePagingInfo));
    }

    public List<BaseIdTranStruVo> SelectAll()
    {
        List<BaseIdTranStruVo> results = baseRepo.Where(a=>1==1)
            .ToList<BaseIdTranStruVo>(a=>new BaseIdTranStruVo(a.Id.ToString(),a.Name));
        return results;
    }
    public List<Employee> QryLiteEmployeeInfoByIds(IEnumerable<string> ids)
    {
        var baseIdTranStruVos = this.QueryUserByIds(ids);
        return EmployeeUtil.BasicEmployeeInfos(baseIdTranStruVos);
    }

    public Employee QryLiteEmployeeInfoById(string id)
    {
        var baseIdTranStruVo = this.GetById(id);
        return EmployeeUtil.BasicEmployeeInfo(baseIdTranStruVo);
    }

    public Employee GetEmployeeDetailById(string id)
    {
        
        Employee employee = this
            .baseRepo
            .Where(a=>a.Id == Convert.ToInt64(id))
            .ToOne<Employee>(a=>new Employee()
            {
                Id = a.Id.ToString(),
                Username = a.Name,
            });
        return employee;
    }

    public List<Employee> GetEmployeeDetailByIds(IEnumerable<string> ids)
    {
        List<long> longIds = ids.Select(a=>Convert.ToInt64(a)).ToList();
        List<User> users = this.baseRepo
            .Where(a => longIds.Contains(a.Id))
            .ToList();
        List<Employee> employees = users.Select(a=>new Employee()
            {
                Id = a.Id.ToString(),
                Username = a.Name,
            })
            .ToList();
        return employees;
    }

   

    public List<BaseIdTranStruVo> GetLevelLeadersByEmployeeIdAndTier(string employeeId, int tier)
    {
        //todo 
        return new List<BaseIdTranStruVo>();
    }

    /// <summary>
    /// 查询有效的用户,如果有效则返回数量
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public long CheckEmployeeEffective(string userId)
    {
        return this.baseRepo.Where(a=>a.Id==Convert.ToInt64(userId)).Count();
    }
}