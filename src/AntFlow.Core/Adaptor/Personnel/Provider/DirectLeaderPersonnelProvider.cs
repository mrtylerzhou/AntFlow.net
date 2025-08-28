using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(DirectLeaderPersonnelProvider))]
public class DirectLeaderPersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly UserService _userService;

    public DirectLeaderPersonnelProvider(UserService userService, AssigneeVoBuildUtils assigneeVoBuildUtils)
        : base(assigneeVoBuildUtils)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        string? startUserId = startConditionsVo.StartUserId;
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryEmployeeDirectLeaderById(startUserId);
        List<string>? userIds = new() { baseIdTranStruVo.Id };
        return ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}