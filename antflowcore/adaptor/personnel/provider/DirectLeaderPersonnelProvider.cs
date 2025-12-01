using antflowcore.conf.di;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;
[NamedService(nameof(DirectLeaderPersonnelProvider))]
public class DirectLeaderPersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;


    public DirectLeaderPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils, IBpmnProcessAdminProvider processAdminProvider, IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        var startUserId = startConditionsVo.StartUserId;
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryEmployeeDirectLeaderById(startUserId);
      
        return base.ProvideAssigneeList(bpmnNodeVo, new List<BaseIdTranStruVo>(){baseIdTranStruVo});
    }
}