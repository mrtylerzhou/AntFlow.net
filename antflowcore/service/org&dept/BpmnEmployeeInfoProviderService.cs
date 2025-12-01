using antflowcore.conf.di;
using AntFlowCore.Entity;
using antflowcore.entityj;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.service;
[NamedService(nameof(BpmnEmployeeInfoProviderService))]
public class BpmnEmployeeInfoProviderService: IBpmnEmployeeInfoProviderService
{
    private readonly IUserService _userService;

    public BpmnEmployeeInfoProviderService(IUserService userService)
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

    public DetailedUser QryLiteEmployeeInfoById(string id)
    {
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryUserById(id);
        DetailedUser employee = new DetailedUser()
        {
            Id = baseIdTranStruVo.Id,
            UserName = baseIdTranStruVo.Name
        };
        return employee;
    }
}