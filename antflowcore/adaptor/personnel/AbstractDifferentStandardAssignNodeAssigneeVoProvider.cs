using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel;

public abstract class AbstractDifferentStandardAssignNodeAssigneeVoProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    protected AbstractDifferentStandardAssignNodeAssigneeVoProvider(AssigneeVoBuildUtils assigneeVoBuildUtils, IBpmnProcessAdminProvider processAdminProvider) : base(assigneeVoBuildUtils, processAdminProvider)
    {
    }
    

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        if (startConditionsVo == null)
            throw new ArgumentNullException(nameof(startConditionsVo));

        var users = new List<string>();
        string startUserId = startConditionsVo.StartUserId;

        // 审批标准：被审批人
        if (bpmnNodeVo.ApprovalStandard == ApprovalStandardEnum.APPROVAL.Code)
        {
            if (startConditionsVo.ApprovalEmpls == null || !startConditionsVo.ApprovalEmpls.Any())
            {
                throw new AFBizException(
                    BusinessError.PARAMS_IS_NULL,
                    "审批标准为被审批人,但是无被审批人!");
            }

            users = startConditionsVo.ApprovalEmpls
                .Select(emp => emp.Id)
                .ToList();
        }
        // 审批标准：上一节点审批人
        else if (bpmnNodeVo.ApprovalStandard == ApprovalStandardEnum.FROM_PREV_NODE.Code)
        {
            var baseIdTranStruVos = bpmnNodeVo.Property?.ContextEmplList;
            if (baseIdTranStruVos != null && baseIdTranStruVos.Any())
            {
                users = baseIdTranStruVos
                    .Select(item => item.Id)
                    .ToList();
            }
        }

        // 如果没有指定用户，则使用发起人
        if (!users.Any())
        {
            users.Add(startUserId);
        }

        // 查询用户详细信息
        List<BaseIdTranStruVo> assignees = QueryUsers(users);

        // 调用基类方法生成最终结果
        return base.ProvideAssigneeList(bpmnNodeVo, assignees);
    }
    protected abstract  List<BaseIdTranStruVo> QueryUsers(List<String> userIds);
}