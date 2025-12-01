using antflowcore.conf.di;
using antflowcore.exception;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

using System;
using System.Collections.Generic;
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
