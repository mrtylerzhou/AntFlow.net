using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(HrbpPersonnelProvider))]
public class HrbpPersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly UserService _userService;

    public HrbpPersonnelProvider(UserService userService, AssigneeVoBuildUtils assigneeVoBuildUtils) : base(
        assigneeVoBuildUtils)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        string startUserId = startConditionsVo.StartUserId;
        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryEmployeeHrpbByEmployeeId(startUserId);

        if (baseIdTranStruVo == null)
        {
            throw new AFBizException("发起人HRBP不存在");
        }

        List<string>? userIds = new() { baseIdTranStruVo.Id };
        return ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}