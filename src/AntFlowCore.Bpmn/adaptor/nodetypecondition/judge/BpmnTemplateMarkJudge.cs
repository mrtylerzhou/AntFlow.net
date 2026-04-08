using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Common.util;
using AntFlowCore.Core.exception;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.Extensions.service.processor.filter;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

public class BpmnTemplateMarkJudge: IConditionJudge
{
    public bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,int group)
    {
        int? templateMarkId = bpmnStartConditionsVo.TemplateMarkId;
        if (templateMarkId == null)
        {
            throw new AFBizException("Template mark id is null");
        }
        if (!ObjectUtils.IsEmpty(conditionsConf.TemplateMarks) &&
            conditionsConf.TemplateMarks.Contains(templateMarkId.Value)) {
            return true;
        }
        return false;
    }
}