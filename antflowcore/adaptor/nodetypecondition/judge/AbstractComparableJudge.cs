using antflowcore.formatter.filter;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.nodetypecondition.judge;

public abstract class AbstractComparableJudge : IConditionJudge
{
    protected bool CompareJudge(decimal confTotal, decimal actual, int operatorType)
    {
        if (confTotal == null)
        {
            throw new ArgumentNullException(nameof(confTotal), "Operator left is null");
        }
        if (actual == null)
        {
            throw new ArgumentNullException(nameof(actual), "Operator right is null");
        }

        // 比较 -1 表示小于, 0 表示等于, 1 表示大于
        bool result = false;
        switch (operatorType)
        {
            case 1: // 大于等于
                if (actual >= confTotal)
                {
                    result = true;
                }
                break;

            case 2: // 大于
                if (actual > confTotal)
                {
                    result = true;
                }
                break;

            case 3: // 小于等于
                if (actual <= confTotal)
                {
                    result = true;
                }
                break;

            case 4: // 小于
                if (actual < confTotal)
                {
                    result = true;
                }
                break;

            case 5: // 等于
                if (actual == confTotal)
                {
                    result = true;
                }
                break;

            default:
                throw new ArgumentException("Invalid operator type", nameof(operatorType));
        }
        return result;
    }

    public abstract bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo);
}