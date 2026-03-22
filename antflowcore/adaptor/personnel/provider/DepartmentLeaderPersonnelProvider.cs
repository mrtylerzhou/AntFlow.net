using antflowcore.constant.enums;
using antflowcore.service.interf.repository;
using antflowcore.vo;

namespace antflowcore.adaptor.personnel.provider;

/// <summary>
/// 部门负责人审批人提供者
/// 根据用户所在部门查询部门负责人
/// </summary>
public class DepartmentLeaderPersonnelProvider : AbstractDifferentStandardAssignNodeAssigneeVoProvider
{
    private readonly IUserService _userService;

    public DepartmentLeaderPersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        IUserService userService) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _userService = userService;
    }

    protected override List<BaseIdTranStruVo> QueryUsers(List<string> userIds)
    {
        return _userService.QueryDepartmentLeaderByIds(userIds);
    }
}
