using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(RolePersonnelProvider))]
public class RolePersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly ILogger<RolePersonnelProvider> _logger;
    private readonly UserService _roleInfoProvider;

    public RolePersonnelProvider(UserService userService, ILogger<RolePersonnelProvider> logger,
        AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
        _roleInfoProvider = userService;
        _logger = logger;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        BpmnNodePropertysVo? propertysVo = bpmnNodeVo.Property;

        if (propertysVo == null || propertysVo.RoleIds == null || propertysVo.RoleIds.Count == 0)
        {
            throw new AFBizException("指定角色找人条件不全，无法找人！");
        }

        if (bpmnNodeVo.IsOutSideProcess != null && bpmnNodeVo.IsOutSideProcess == 1)
        {
            List<BaseIdTranStruVo>? emplList = bpmnNodeVo.Property.EmplList;
            if (emplList == null || emplList.Count == 0)
            {
                throw new AFBizException("thirdy party process role node has no employee info");
            }

            return ProvideAssigneeList(bpmnNodeVo, null);
        }

        List<string>? roleIds = propertysVo.RoleIds;
        Dictionary<string, string> roleEmployeeInfo = _roleInfoProvider.ProvideRoleEmployeeInfo(roleIds);

        if (roleEmployeeInfo == null || roleEmployeeInfo.Count == 0)
        {
            _logger.LogWarning("can not find specified roles info via roleIds:{roleIds}", string.Join(",", roleIds));
            throw new AFBizException("can not find specified roles info via roleIds");
        }

        List<string>? userIds = roleEmployeeInfo.Keys.ToList();
        return ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}