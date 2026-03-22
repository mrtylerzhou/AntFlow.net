using antflowcore.adaptor.personnel;
using antflowcore.constant.enums;
using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

/// <summary>
/// 用户自定义规则审批人提供者（Demo）
/// 用户可以继承此类并重写GetAssigneeList方法来实现自己的自定义规则逻辑
/// 
/// 使用说明：
/// - udrAssigneeProperty.Id 为自定义规则类型标识（如zdysp1代表指定人员）
/// - udrValueJson 为自定义规则对应的值JSON（如用户ID列表）
/// - startConditionsVo.StartUserId 为流程发起人ID
/// </summary>
public class UDRPersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    public UDRPersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider) : base(assigneeVoBuildUtils, processAdminProvider)
    {
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        var assignees = new List<BaseIdTranStruVo>();

        // 自定义规则类型标识，如zdysp1代表指定人员
        BaseIdTranStruVo udrAssigneeProperty = bpmnNodeVo.Property?.UdrAssigneeProperty;
        // 自定义规则值JSON，如指定的用户ID列表
        string udrValueJson = bpmnNodeVo.Property?.UdrValueJson;
        string startUserId = startConditionsVo?.StartUserId;

        if (udrAssigneeProperty == null)
        {
            throw new AFBizException("udrAssigneeProperty不能为空");
        }

        // 以下是Demo逻辑，用户应重写此方法实现自己的业务逻辑
        if (udrAssigneeProperty.Id.Equals("zdysp1", StringComparison.OrdinalIgnoreCase))
        {
            // 例如zdysp1代表指定人员，udrValueJson中存储的是用户ID列表
            // 用户应反序列化udrValueJson并查询对应的用户信息
            assignees.Add(new BaseIdTranStruVo { Id = "1", Name = "张三" });
        }
        else
        {
            // 默认逻辑，用户可继续增加自定义规则分支
            assignees.Add(new BaseIdTranStruVo { Id = "1", Name = "张三" });
        }

        return base.ProvideAssigneeList(bpmnNodeVo, assignees);
    }
}
