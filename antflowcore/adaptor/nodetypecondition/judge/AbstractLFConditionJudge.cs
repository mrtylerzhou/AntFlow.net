using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.nodetypecondition.judge;

public abstract class AbstractLFConditionJudge: AbstractComparableJudge
{
    protected bool LfCommonJudge(
        BpmnNodeConditionsConfBaseVo conditionsConf, 
        BpmnStartConditionsVo bpmnStartConditionsVo, 
        Func<object, object,int, bool> predicate,int currentIndex)
    {
        Dictionary<string,object> lfConditionsFromDb = conditionsConf.LfConditions;
        Dictionary<string,object> lfConditionsFromUser = bpmnStartConditionsVo.LfConditions;

        if (lfConditionsFromDb == null || !lfConditionsFromDb.Any())
        {
            throw new AFBizException("The process has no conditions configuration. Please contact the administrator to add one.");
        }

        if (lfConditionsFromUser == null || !lfConditionsFromUser.Any())
        {
            throw new AFBizException("The process has no conditions form. Please contact the administrator.");
        }

        bool isMatch = false;
        int iterIndex=0;
        List<int> numberOperatorList = conditionsConf.NumberOperatorList;
        //operator type
        foreach (var kvp in lfConditionsFromDb)
        {
            if(iterIndex!=currentIndex){
                iterIndex++;
                continue;
            }
            string key = kvp.Key;
            if (!lfConditionsFromUser.TryGetValue(key, out var valueFromUser) || valueFromUser == null)
            {
                throw new AFBizException($"Condition field from user '{key}' cannot be null.");
            }
            
            var valueFromDb = kvp.Value;
            if (valueFromDb == null)
            {
                throw new AFBizException($"Condition field from database '{key}' cannot be null.");
            }
            int numberOperator = numberOperatorList[iterIndex];
            isMatch = predicate(valueFromDb, valueFromUser,numberOperator);
        }

        return isMatch;
    }
}