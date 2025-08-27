using AntFlow.Core.Util;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class LFDateTimeConditionJudge : AbstractLFDateTimeConditionJudge
{
    public LFDateTimeConditionJudge(ILogger<AbstractLFDateTimeConditionJudge> logger) : base(logger)
    {
    }

    protected override string CurrentDateFormatter()
    {
        return DateUtil.DATETIME_PATTERN;
    }
}