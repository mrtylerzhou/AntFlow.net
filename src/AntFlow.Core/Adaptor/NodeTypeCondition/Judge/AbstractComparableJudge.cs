using AntFlow.Core.Exception;
using AntFlow.Core.Service.Processor.Filter;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

/// <summary>
///     抽象的比较判断器，用于数字区间和单值比较
/// </summary>
public abstract class AbstractComparableJudge : IConditionJudge
{
    public abstract bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group);

    protected bool CompareJudge(decimal? confTotal, decimal? confTotal2, decimal? actual, int? operatorType)
    {
        if (confTotal == null)
        {
            throw new AFBizException("operator left is null");
        }

        if (actual == null)
        {
            throw new AFBizException("operator right is null");
        }

        if (operatorType == null)
        {
            throw new AFBizException("operator is null");
        }

        if (confTotal2 == null)
        {
            confTotal2 = 0;
        }

        bool flag = false;

        switch (operatorType)
        {
            case 1:
                if (actual >= confTotal)
                {
                    flag = true;
                }

                break;
            case 2:
                if (actual > confTotal)
                {
                    flag = true;
                }

                break;
            case 3:
                if (actual <= confTotal)
                {
                    flag = true;
                }

                break;
            case 4:
                if (actual < confTotal)
                {
                    flag = true;
                }

                break;
            case 5:
                if (actual == confTotal)
                {
                    flag = true;
                }

                break;
            case 6:
                if (actual > confTotal && actual < confTotal2)
                {
                    flag = true;
                }

                break;
            case 7:
                if (actual >= confTotal && actual < confTotal2)
                {
                    flag = true;
                }

                break;
            case 8:
                if (actual > confTotal && actual <= confTotal2)
                {
                    flag = true;
                }

                break;
            case 9:
                if (actual >= confTotal && actual <= confTotal2)
                {
                    flag = true;
                }

                break;
            default:
                throw new AFBizException("operator is not supported at the moment yet");
        }

        return flag;
    }
}