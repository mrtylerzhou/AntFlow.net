using AntFlow.Core.Exception;
using AntFlow.Core.Service.Processor.Filter;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class BpmnTemplateMarkJudge : IConditionJudge
{
    public bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group)
    {
        int? templateMarkId = bpmnStartConditionsVo.TemplateMarkId;
        if (templateMarkId == null)
        {
            throw new AFBizException("Template mark id is null");
        }

        if (!ObjectUtils.IsEmpty(conditionsConf.TemplateMarks) &&
            conditionsConf.TemplateMarks.Contains(templateMarkId.Value))
        {
            return true;
        }

        return false;
    }
}