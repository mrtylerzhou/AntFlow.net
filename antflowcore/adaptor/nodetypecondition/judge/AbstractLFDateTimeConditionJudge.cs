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

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,int group)
    {
        Func<object, object,int, bool> predicate = (a, b,c) =>
        {
            try
            {
                String[] split = a.ToString().Split(",");
                DateTime dateFromDb1 =DateTime.ParseExact(split[0], CurrentDateFormatter(), CultureInfo.InvariantCulture);
                DateTime? dateFromDb2 = null;
                if (split.Length > 1)
                {
                    DateTime.ParseExact(split[1], CurrentDateFormatter(), CultureInfo.InvariantCulture);
                }
                var dateFromUser = DateTime.ParseExact(b.ToString(), CurrentDateFormatter(), CultureInfo.InvariantCulture);
                decimal dateFromDb1Ticks = new decimal(dateFromDb1.Ticks);
                decimal? dateFromDb2Ticks =dateFromDb2==null?null: new decimal(dateFromDb1.Ticks);
                var dateFromUserTicks = new decimal(dateFromUser.Ticks);
                
                return CompareJudge(dateFromDb1Ticks, dateFromDb2Ticks,dateFromUserTicks, c);
            }
            catch (FormatException e)
            {
                _logger.LogError("Date parse exception while condition judging: {Message}", e.Message);
                throw;
            }
        };

        return LfCommonJudge(conditionsConf, bpmnStartConditionsVo, predicate,group);
    }

    protected abstract string CurrentDateFormatter();
}

