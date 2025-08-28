using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(LevelPersonnelProvider))]
public class LevelPersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly UserService _userService;

    public LevelPersonnelProvider(UserService userService, AssigneeVoBuildUtils assigneeVoBuildUtils) : base(
        assigneeVoBuildUtils)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        BpmnNodePropertysVo? propertysVo = bpmnNodeVo.Property;
        string startUserId = startConditionsVo.StartUserId;

        if (propertysVo == null || propertysVo.AssignLevelGrade == null || startUserId == null)
        {
            throw new AFBizException("指定层级审批条件不全，无法找人！");
        }

        int assignLevelType = propertysVo.AssignLevelType.GetValueOrDefault();
        int assignLevelGrade = propertysVo.AssignLevelGrade.GetValueOrDefault();

        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryLeaderByEmployeeIdAndLevel(startUserId, assignLevelGrade);

        if (baseIdTranStruVo == null)
        {
            throw new AFBizException("未能根据发起人和指定层级找到审批人信息");
        }

        List<string>? userIds = new() { baseIdTranStruVo.Id };
        return ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}