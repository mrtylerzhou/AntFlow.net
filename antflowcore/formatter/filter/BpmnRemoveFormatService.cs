using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.filter;

public class BpmnRemoveFormatService: AbstractBpmnRemoveFormat
{
    public new void RemoveBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        base.RemoveBpmnConf(bpmnConfVo, bpmnStartConditions);
    }

    public override List<Func<bool>> TrueFuncs(BpmnNodeVo vo, BpmnStartConditionsVo bpmnStartConditionsVo)
    {
        Func<bool> func1 = () => vo.Params.ParamType == 1 && vo.Params.Assignee.Assignee == "0";
        Func<bool> func2 = () => vo.Params.ParamType == 2 && vo.Params.AssigneeList[0].Assignee == "0";
        return new List<Func<bool>> { func1, func2 };
    }

    public override int Order()
    {
        return 1;
    }
}