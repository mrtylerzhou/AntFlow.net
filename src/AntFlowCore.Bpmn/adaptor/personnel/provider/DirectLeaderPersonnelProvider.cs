using AntFlowCore.Abstraction.util;
using AntFlowCore.Core.conf;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;
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