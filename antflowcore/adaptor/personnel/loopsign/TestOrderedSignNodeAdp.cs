using antflowcore.constant.enums;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.loopsign;

public class TestOrderedSignNodeAdp : AbstractOrderedSignNodeAdp
{
    public TestOrderedSignNodeAdp(AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
    }

    public override List<string> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        return new List<string> { "1", "21", "23", "42" };
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(OrderNodeTypeEnum.TEST_ORDERED_SIGN);
    }
}