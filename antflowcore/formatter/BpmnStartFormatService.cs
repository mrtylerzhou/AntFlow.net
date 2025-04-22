using antflowcore.formatter.filter;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter;

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