using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class LFStringConditionJudgeService: AbstractLFConditionJudge
{
    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo)
    {
        return base.LfCommonJudge(conditionsConf,bpmnStartConditionsVo,(a,b)=>a.ToString().Equals(b.ToString()));
    }
}