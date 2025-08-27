using AntFlow.Core.Exception;
using AntFlow.Core.Vo;
using System.Collections;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class LFCollectionConditionJudge : AbstractLFConditionJudge
{
    private readonly ILogger<LFCollectionConditionJudge> _logger;

    public LFCollectionConditionJudge(ILogger<LFCollectionConditionJudge> logger)
    {
        _logger = logger;
    }

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group)
    {
        Func<object, object, int, bool> predicate = (a, b, c) =>
        {
            if (a is not IEnumerable iterableValue)
            {
                throw new AFBizException("value from db is not iterable");
            }

            foreach (object? actualValue in iterableValue)
            {
                if (b is IEnumerable iterableB && b is not string)
                {
                    foreach (object? bValue in iterableB)
                    {
                        if (actualValue?.ToString() == bValue?.ToString())
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (actualValue?.ToString() == b?.ToString())
                    {
                        return true;
                    }
                }
            }

            return false;
        };

        return LfCommonJudge(conditionsConf, bpmnStartConditionsVo, predicate, group);
    }
}