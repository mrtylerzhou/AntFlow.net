using antflowcore.conf.di;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;
[NamedService(nameof(HrbpPersonnelProvider))]
public class HrbpPersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly UserService _userService;

    public HrbpPersonnelProvider(UserService userService,AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        string startUserId = startConditionsVo.StartUserId;
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryEmployeeHrpbByEmployeeId(startUserId);

        if (baseIdTranStruVo == null)
        {
            throw new AFBizException("发起人HRBP不存在");
        }

        var userIds = new List<string> { baseIdTranStruVo.Id.ToString() };
        return base.ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}