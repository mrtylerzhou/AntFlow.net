using AntFlowCore.Common.util;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;


namespace AntFlowCore.Engine.Engine.service.biz;

using System.Collections.Generic;
using System.Linq;

public class EmployeeService
{
    private readonly IUserService _userService;


    public EmployeeService(IUserService userService)
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

    public DetailedUser GetEmployeeDetailById(string id)
    {

        return _userService.GetDetailedUserById(id);
    }

    public List<DetailedUser> GetEmployeeDetailByIds(IEnumerable<string> ids)
    {
        return _userService.GetEmployeeDetailByIds(ids);
    }

   

    public List<BaseIdTranStruVo> GetLevelLeadersByEmployeeIdAndTier(string employeeId, int tier)
    {
        //todo 
        return new List<BaseIdTranStruVo>();
    }
}
