using antflowcore.conf.di;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;
[NamedService(nameof(DirectLeaderPersonnelProvider))]
public class DirectLeaderPersonnelProvider : AbstractDifferentStandardAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;


    public DirectLeaderPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils, IBpmnProcessAdminProvider processAdminProvider, IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }
    

    protected override List<BaseIdTranStruVo> QueryUsers(List<string> userIds)
    {
        List<BaseIdTranStruVo> employeeDirectLeaderByIds = _userService.QueryEmployeeDirectLeaderByIds(userIds);
        return employeeDirectLeaderByIds;
    }
}