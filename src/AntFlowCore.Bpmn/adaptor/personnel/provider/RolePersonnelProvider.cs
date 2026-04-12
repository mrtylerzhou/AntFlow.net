using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

[NamedService(nameof(RolePersonnelProvider))]
public class RolePersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly IUserService _roleInfoProvider;
    private readonly ILogger<RolePersonnelProvider> _logger;


    public RolePersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils, IBpmnProcessAdminProvider processAdminProvider, IUserService roleInfoProvider, ILogger<RolePersonnelProvider> logger) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _roleInfoProvider = roleInfoProvider;
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
        Dictionary<string,string> roleEmployeeInfo = _roleInfoProvider.ProvideRoleEmployeeInfo(roleIds);



        List<BaseIdTranStruVo> baseIdTranStruVoList = roleEmployeeInfo.IsEmpty()
            ? new List<BaseIdTranStruVo>()
            : roleEmployeeInfo.Select(a => new BaseIdTranStruVo(a.Key, a.Value)).ToList();
        return base.ProvideAssigneeList(bpmnNodeVo, baseIdTranStruVoList);
    }
}
