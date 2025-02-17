using antflowcore.service.processor.filter;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor;

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