using antflowcore.conf.di;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

using Microsoft.Extensions.Logging;
using System.Collections.Generic;

[NamedService(nameof(RolePersonnelProvider))]
public class RolePersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly UserService _roleInfoProvider;
    private readonly ILogger<RolePersonnelProvider> _logger;

    public RolePersonnelProvider(UserService userService, ILogger<RolePersonnelProvider> logger, AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
        _roleInfoProvider = userService;
        _logger = logger;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        var propertysVo = bpmnNodeVo.Property;

        if (propertysVo == null || propertysVo.RoleIds == null || propertysVo.RoleIds.Count == 0)
        {
            throw new AFBizException("指定角色找人条件不全，无法找人！");
        }

        if (bpmnNodeVo.IsOutSideProcess != null && bpmnNodeVo.IsOutSideProcess == 1)
        {
            var emplList = bpmnNodeVo.Property.EmplList;
            if (emplList == null || emplList.Count == 0)
            {
                throw new AFBizException("thirdy party process role node has no employee info");
            }
            return base.ProvideAssigneeList(bpmnNodeVo, null);
        }

        var roleIds = propertysVo.RoleIds;
        Dictionary<string, string> roleEmployeeInfo = _roleInfoProvider.ProvideRoleEmployeeInfo(roleIds);

        if (roleEmployeeInfo == null || roleEmployeeInfo.Count == 0)
        {
            _logger.LogWarning("can not find specified roles info via roleIds:{roleIds}", string.Join(",", roleIds));
            throw new AFBizException("can not find specified roles info via roleIds");
        }

        var userIds = roleEmployeeInfo.Keys.ToList();
        return base.ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}