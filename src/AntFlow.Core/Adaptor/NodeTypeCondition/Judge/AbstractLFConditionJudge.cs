using AntFlow.Core.Exception;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public abstract class AbstractLFConditionJudge : AbstractComparableJudge
{
    protected bool LfCommonJudge(
        BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo,
        Func<object, object, int, bool> predicate, int currentGroup)
    {
        IDictionary<int, IDictionary<string, object>> groupedLfConditionsMap = conditionsConf.GroupedLfConditionsMap;
        IDictionary<int, List<int>> groupedNumberOperatorListMap = conditionsConf.GroupedNumberOperatorListMap;
        IDictionary<string, object> lfConditionsFromDb = null;
        if (groupedLfConditionsMap != null && groupedLfConditionsMap.ContainsKey(currentGroup))
        {
            lfConditionsFromDb = groupedLfConditionsMap[currentGroup];
        }
        else
        {
            throw new AFBizException(
                "the process has no no code conditions conf,please contact the administrator to add one");
        }

        IDictionary<string, object> lfConditionsFromUser = bpmnStartConditionsVo.LfConditions;

        if (lfConditionsFromDb == null || !lfConditionsFromDb.Any())
        {
            throw new AFBizException(
                "The process has no conditions configuration. Please contact the administrator to add one.");
        }

        if (lfConditionsFromUser == null || !lfConditionsFromUser.Any())
        {
            throw new AFBizException("The process has no conditions form. Please contact the administrator.");
        }

        bool isMatch = false;
        int iterIndex = 0;
        List<int> numberOperatorList = groupedNumberOperatorListMap[currentGroup];
        //operator type
        foreach (KeyValuePair<string, object> kvp in lfConditionsFromDb)
        {
            string key = kvp.Key;
            if (!lfConditionsFromUser.TryGetValue(key, out object? valueFromUser) || valueFromUser == null)
            {
                throw new AFBizException($"Condition field from user '{key}' cannot be null.");
            }

            object? valueFromDb = kvp.Value;
            if (valueFromDb == null)
            {
                throw new AFBizException($"Condition field from database '{key}' cannot be null.");
            }

            int numberOperator = numberOperatorList[iterIndex];
            isMatch = predicate(valueFromDb, valueFromUser, numberOperator);
        }

        return isMatch;
    }
}