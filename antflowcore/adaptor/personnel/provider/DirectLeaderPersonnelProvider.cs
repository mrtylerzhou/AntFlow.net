using antflowcore.conf.di;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

[NamedService(nameof(DirectLeaderPersonnelProvider))]
public class DirectLeaderPersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly UserService _userService;

    public DirectLeaderPersonnelProvider(UserService userService, AssigneeVoBuildUtils assigneeVoBuildUtils)
        : base(assigneeVoBuildUtils)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        var startUserId = startConditionsVo.StartUserId;
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryEmployeeDirectLeaderById(startUserId);
        var userIds = new List<string> { baseIdTranStruVo.Id };
        return base.ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}