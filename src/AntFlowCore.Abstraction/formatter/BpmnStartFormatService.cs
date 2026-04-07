using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.Extensions.service.processor;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.formatter;

public class BpmnStartFormatService: IBpmnStartFormat
{
    private readonly ConditionFilterService _conditionFilterService;

    public BpmnStartFormatService(ConditionFilterService conditionFilterService)
    {
        _conditionFilterService = conditionFilterService;
    }
    public void FormatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        _conditionFilterService.ConditionfilterNode(bpmnConfVo, bpmnStartConditions);
    }
}