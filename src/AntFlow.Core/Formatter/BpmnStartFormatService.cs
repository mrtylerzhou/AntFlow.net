using AntFlow.Core.Service.Processor.Filter;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor;

public class BpmnStartFormatService : IBpmnStartFormat
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