using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.util;

public static class EmployeeUtil
{
    public static List<Employee> BasicEmployeeInfos(List<BaseIdTranStruVo> liteEmployeeList)
    {
        if (liteEmployeeList == null || !liteEmployeeList.Any())
        {
            return new List<Employee>();
        }

        return liteEmployeeList
            .Select(BasicEmployeeInfo)
            .ToList();
    }

    public static Employee BasicEmployeeInfo(BaseIdTranStruVo liteEmployee)
    {
        if (liteEmployee == null)
            return null;

        return new Employee
        {
            Id = liteEmployee.Id,
            Username = liteEmployee.Name
        };
    }
}