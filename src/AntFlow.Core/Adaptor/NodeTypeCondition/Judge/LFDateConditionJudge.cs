using AntFlow.Core.Util;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class LFDateConditionJudge : AbstractLFDateTimeConditionJudge
{
    public LFDateConditionJudge(ILogger<AbstractLFDateTimeConditionJudge> logger) : base(logger)
    {
    }

    protected override string CurrentDateFormatter()
    {
        return DateUtil.DATE_PATTERN;
    }
}