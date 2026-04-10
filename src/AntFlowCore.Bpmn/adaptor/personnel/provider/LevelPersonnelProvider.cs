using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

[NamedService(nameof(LevelPersonnelProvider))]
public class LevelPersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;


    public LevelPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils, IBpmnProcessAdminProvider processAdminProvider, IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        var propertysVo = bpmnNodeVo.Property;
        string startUserId = startConditionsVo.StartUserId;

        if (propertysVo == null || propertysVo.AssignLevelGrade == null || startUserId == null)
        {
            throw new AFBizException("指定层级审批条件不全，无法找人！");
        }

        int assignLevelType = propertysVo.AssignLevelType.GetValueOrDefault();
        int assignLevelGrade = propertysVo.AssignLevelGrade.GetValueOrDefault();

        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryLeaderByEmployeeIdAndLevel(startUserId, assignLevelGrade);

        return  base.ProvideAssigneeList(bpmnNodeVo,new List<BaseIdTranStruVo>(){baseIdTranStruVo});
    }
}
