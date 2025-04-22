using antflowcore.util;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class LFDateTimeConditionJudgeService: AbstractLFDateTimeConditionJudge
{
    public LFDateTimeConditionJudgeService(ILogger<AbstractLFDateTimeConditionJudge> logger) : base(logger)
    {
    }

    protected override string CurrentDateFormatter()
    {
        return DateUtil.DATETIME_PATTERN;
    }
}