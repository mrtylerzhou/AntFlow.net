using antflowcore.util;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class LFDateTimeConditionJudge: AbstractLFDateTimeConditionJudge
{
    public LFDateTimeConditionJudge(ILogger<AbstractLFDateTimeConditionJudge> logger) : base(logger)
    {
    }

    protected override string CurrentDateFormatter()
    {
        return DateUtil.DATETIME_PATTERN;
    }
}