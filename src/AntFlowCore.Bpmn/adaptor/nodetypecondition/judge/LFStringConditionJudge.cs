using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

public class LFStringConditionJudge: AbstractLFConditionJudge
{
    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,int group)
    {
        return base.LfCommonJudge(conditionsConf,bpmnStartConditionsVo,(a,b,c)=>a.ToString().Equals(b.ToString(),StringComparison.CurrentCultureIgnoreCase),group);
    }
}