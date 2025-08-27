using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class LFStringConditionJudge : AbstractLFConditionJudge
{
    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group)
    {
        return LfCommonJudge(conditionsConf, bpmnStartConditionsVo,
            (a, b, c) => a.ToString().Equals(b.ToString(), StringComparison.CurrentCultureIgnoreCase), group);
    }
}