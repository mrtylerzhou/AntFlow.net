using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service;

[NamedService(nameof(BpmnEmployeeInfoProviderService))]
public class BpmnEmployeeInfoProviderService : IBpmnEmployeeInfoProviderService
{
    private readonly UserService _userService;

    public BpmnEmployeeInfoProviderService(UserService userService)
    {
        _userService = userService;
    }

    public Dictionary<string, string> ProvideEmployeeInfo(IEnumerable<string> empIds)
    {
        List<BaseIdTranStruVo> users = _userService.QueryUserByIds(empIds);
        if (ObjectUtils.IsEmpty(users))
        {
            return new Dictionary<string, string>();
        }

        Dictionary<string, string> empIdAndNameMap = users
            .ToDictionary(user => user.Id, user => user.Name, StringComparer.OrdinalIgnoreCase);

        return empIdAndNameMap;
    }

    public Employee QryLiteEmployeeInfoById(string id)
    {
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryUserById(id);
        Employee employee = new() { Id = baseIdTranStruVo.Id, Username = baseIdTranStruVo.Name };
        return employee;
    }
}