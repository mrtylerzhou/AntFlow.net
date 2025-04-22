using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class LFNumberFormatConditionJudgeService : AbstractLFConditionJudge
{
    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo)
    {
        return base.LfCommonJudge(conditionsConf, bpmnStartConditionsVo,
            (a, b) => base.CompareJudge(NumberUtils.ToScaledDecimal(a.ToString(), 2, MidpointRounding.ToEven)
            , NumberUtils.ToScaledDecimal(b.ToString(), 2, MidpointRounding.ToEven), conditionsConf.NumberOperator.Value));
    }
}