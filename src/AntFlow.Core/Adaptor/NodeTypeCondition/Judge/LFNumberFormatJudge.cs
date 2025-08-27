using AntFlow.Core.Vo;
using System.Globalization;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class LFNumberFormatJudge : AbstractLFConditionJudge
{
    private readonly ILogger<LFNumberFormatJudge> _logger;

    public LFNumberFormatJudge(ILogger<LFNumberFormatJudge> logger)
    {
        _logger = logger;
    }

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group)
    {
        Func<object, object, int, bool> predicate = (a, b, c) =>
        {
            if (a == null)
            {
                return false;
            }

            string[]? split = a.ToString().Split(',');
            decimal valueInDbBig1 = decimal.Parse(split[0], CultureInfo.InvariantCulture);
            decimal? valueInDbBig2 = null;

            if (split.Length > 1)
            {
                valueInDbBig2 = decimal.Parse(split[1], CultureInfo.InvariantCulture);
            }

            // 保留两位小数，四舍五入
            decimal userValue = Math.Round(decimal.Parse(b.ToString(), CultureInfo.InvariantCulture), 2,
                MidpointRounding.AwayFromZero);

            return CompareJudge(valueInDbBig1, valueInDbBig2, userValue, c);
        };

        return LfCommonJudge(conditionsConf, bpmnStartConditionsVo, predicate, group);
    }
}