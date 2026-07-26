using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.loopsign;

public class TestOrderedSignNodeAdp : AbstractOrderedSignNodeAdp
{
    public TestOrderedSignNodeAdp(AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
    }

    public override List<List<string>> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        //包法 X:每个 id 独立成一层(每层 1 人),保持链式语义
        var result = new List<List<string>>
        {
            new List<string> { "1" },
            new List<string> { "21" },
            new List<string> { "23" },
            new List<string> { "42" }
        };
        return result;
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(OrderNodeTypeEnum.TEST_ORDERED_SIGN);
    }
}
