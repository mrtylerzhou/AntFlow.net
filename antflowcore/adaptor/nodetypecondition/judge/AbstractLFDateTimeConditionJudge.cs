using System.Globalization;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public abstract class AbstractLFDateTimeConditionJudge : AbstractLFConditionJudge
{
    private readonly ILogger<AbstractLFDateTimeConditionJudge> _logger;

    protected AbstractLFDateTimeConditionJudge(ILogger<AbstractLFDateTimeConditionJudge> logger)
    {
        _logger = logger;
    }

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo)
    {
        Func<object, object, bool> predicate = (a, b) =>
        {
            try
            {
                var dateFromDb = (DateTime)a;
                var dateFromUser = DateTime.ParseExact(b.ToString(), CurrentDateFormatter(), CultureInfo.InvariantCulture);
                var dateFromDbTicks = new decimal(dateFromDb.Ticks);
                var dateFromUserTicks = new decimal(dateFromUser.Ticks);
                var numberOperator = conditionsConf.NumberOperator;
                return CompareJudge(dateFromDbTicks, dateFromUserTicks, numberOperator.Value);
            }
            catch (FormatException e)
            {
                _logger.LogError("Date parse exception while condition judging: {Message}", e.Message);
                throw;
            }
        };

        return LfCommonJudge(conditionsConf, bpmnStartConditionsVo, predicate);
    }

    protected abstract string CurrentDateFormatter();
}

