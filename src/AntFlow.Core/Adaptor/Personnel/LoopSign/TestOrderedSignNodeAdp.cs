using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel;

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