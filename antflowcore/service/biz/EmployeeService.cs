using AntFlowCore.Entities;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.service.biz;

using System.Collections.Generic;
using System.Linq;

public class EmployeeService
{
    private readonly UserService _userService;


    public EmployeeService(UserService userService)
    {
        _userService = userService;
    }

    public List<Employee> QryLiteEmployeeInfoByIds(IEnumerable<string> ids)
    {
        var baseIdTranStruVos = _userService.QueryUserByIds(ids);
        return EmployeeUtil.BasicEmployeeInfos(baseIdTranStruVos);
    }

    public Employee QryLiteEmployeeInfoById(string id)
    {
        var baseIdTranStruVo = _userService.GetById(id);
        return EmployeeUtil.BasicEmployeeInfo(baseIdTranStruVo);
    }

    public Employee GetEmployeeDetailById(string id)
    {
        
        Employee employee = _userService
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
        List<User> users = _userService.baseRepo
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
}
