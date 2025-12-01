using antflowcore.conf.di;
using antflowcore.exception;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;
[NamedService(nameof(HrbpPersonnelProvider))]
public class HrbpPersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;


    public HrbpPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils, IBpmnProcessAdminProvider processAdminProvider, IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        string startUserId = startConditionsVo.StartUserId;
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryEmployeeHrpbByEmployeeId(startUserId);

       
        
        return base.ProvideAssigneeList(bpmnNodeVo, new List<BaseIdTranStruVo>(){baseIdTranStruVo});
    }
}