using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.nodetypecondition.judge;

public abstract class AbstractLFConditionJudge: AbstractComparableJudge
{
    protected bool LfCommonJudge(
        BpmnNodeConditionsConfBaseVo conditionsConf, 
        BpmnStartConditionsVo bpmnStartConditionsVo, 
        Func<object, object, bool> predicate)
    {
        var lfConditionsFromDb = conditionsConf.LfConditions;
        var lfConditionsFromUser = bpmnStartConditionsVo.LfConditions;

        if (lfConditionsFromDb == null || !lfConditionsFromDb.Any())
        {
            throw new AFBizException("The process has no conditions configuration. Please contact the administrator to add one.");
        }

        if (lfConditionsFromUser == null || !lfConditionsFromUser.Any())
        {
            throw new AFBizException("The process has no conditions form. Please contact the administrator.");
        }

        bool isMatch = false;

        foreach (var kvp in lfConditionsFromDb)
        {
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

            isMatch = predicate(valueFromDb, valueFromUser);
        }

        return isMatch;
    }
}