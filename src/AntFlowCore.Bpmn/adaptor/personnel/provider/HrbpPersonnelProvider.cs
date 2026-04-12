using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.vo;
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