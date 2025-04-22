using antflowcore.util;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class LFDateConditionJudgeService : AbstractLFDateTimeConditionJudge
{
    public LFDateConditionJudgeService(ILogger<AbstractLFDateTimeConditionJudge> logger) : base(logger)
    {
    }

    protected override string CurrentDateFormatter()
    {
        return DateUtil.DATE_PATTERN;
    }
}