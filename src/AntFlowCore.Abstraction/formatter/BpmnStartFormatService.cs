using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

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