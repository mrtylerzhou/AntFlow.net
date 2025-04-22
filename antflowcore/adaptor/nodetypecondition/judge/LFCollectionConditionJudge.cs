using System.Collections;
using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class LFCollectionConditionJudge : AbstractLFConditionJudge
{
    private readonly ILogger<LFCollectionConditionJudge> _logger;

    public LFCollectionConditionJudge(ILogger<LFCollectionConditionJudge> logger)
    {
        _logger = logger;
    }

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo)
    {
        Func<object, object, bool> predicate = (a, b) =>
        {
            if (!(a is IEnumerable iterableValue))
            {
                throw new AFBizException("Value from db is not iterable");
            }

            foreach (var actualValue in iterableValue)
            {
                if (actualValue.ToString() == b.ToString())
                {
                    return true;
                }
            }

            return false;
        };

        return base.LfCommonJudge(conditionsConf, bpmnStartConditionsVo, predicate);
    }
}