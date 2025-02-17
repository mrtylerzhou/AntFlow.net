using antflowcore.conf.di;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.service;
[NamedService(nameof(BpmnEmployeeInfoProviderService))]
public class BpmnEmployeeInfoProviderService: IBpmnEmployeeInfoProviderService
{
    private readonly UserService _userService;

    public BpmnEmployeeInfoProviderService(UserService userService)
    {
        _userService = userService;
    }
    public Dictionary<string, string> ProvideEmployeeInfo(IEnumerable<string> empIds)
    {
        
        List<BaseIdTranStruVo> users = _userService.QueryUserByIds(empIds);
        if(ObjectUtils.IsEmpty(users)){
            return new Dictionary<string, string>();
        }
        Dictionary<String,String>empIdAndNameMap =  users
            .ToDictionary(user => user.Id, user => user.Name, StringComparer.OrdinalIgnoreCase);

        return empIdAndNameMap;
    }
}