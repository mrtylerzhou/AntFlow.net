using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class LFStringConditionJudge: AbstractLFConditionJudge
{
    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,int index,int group)
    {
        return base.LfCommonJudge(conditionsConf,bpmnStartConditionsVo,(a,b,c)=>a.ToString().Equals(b.ToString(),StringComparison.CurrentCultureIgnoreCase),index,group);
    }
}