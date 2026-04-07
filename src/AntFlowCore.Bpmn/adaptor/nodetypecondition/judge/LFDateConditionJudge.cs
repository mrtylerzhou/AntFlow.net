using AntFlowCore.Common.util;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

public class LFDateConditionJudge: AbstractLFDateTimeConditionJudge
{
    public LFDateConditionJudge(ILogger<AbstractLFDateTimeConditionJudge> logger) : base(logger)
    {
    }

    protected override string CurrentDateFormatter()
    {
       return DateUtil.DATE_PATTERN;
    }
}