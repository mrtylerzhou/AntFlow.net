using AntFlowCore.AspNetCore.AspNetCore.conf.di;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions;
using AntFlowCore.Extensions.Extensions.adaptor.personnel;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;
[NamedService(nameof(HrbpPersonnelProvider))]
public class HrbpPersonnelProvider : AbstractDifferentStandardAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;


    public HrbpPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils, IBpmnProcessAdminProvider processAdminProvider, IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }
    

    protected override List<BaseIdTranStruVo> QueryUsers(List<string> userIds)
    {
        return _userService.QueryEmployeeHrpbsByEmployeeIds(userIds);
    }
}