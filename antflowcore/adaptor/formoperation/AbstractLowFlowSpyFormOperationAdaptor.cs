using System.Reflection;
using antflowcore.service.repository;
using antflowcore.util.Extension;
using AntFlowCore.Vo;

namespace antflowcore.adaptor;

public abstract class AbstractLowFlowSpyFormOperationAdaptor<T> : IFormOperationAdaptor<T> where T : BusinessDataVo
{
    private readonly BpmnNodeConditionsConfService _bpmnNodeConditionsConfService;

    public AbstractLowFlowSpyFormOperationAdaptor(BpmnNodeConditionsConfService bpmnNodeConditionsConfService)
    {
        _bpmnNodeConditionsConfService = bpmnNodeConditionsConfService;
    }

    public abstract void PreviewSetCondition(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public BpmnStartConditionsVo PreviewSetCondition(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        List<String> fieldNames = _bpmnNodeConditionsConfService.QueryConditionParamNameByProcessNumber(vo);
        IDictionary<string,object> formLfLikeConditions = FormLFLikeConditions(vo,fieldNames);
        conditionsVo.LfConditions = formLfLikeConditions;
        PreviewSetCondition(conditionsVo, vo);
        return conditionsVo;
    }

    public abstract void LaunchParameters(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public BpmnStartConditionsVo LaunchParameters(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        List<String> fieldNames = _bpmnNodeConditionsConfService.QueryConditionParamNameByProcessNumber(vo);
        IDictionary<string,object> formLfLikeConditions = FormLFLikeConditions(vo,fieldNames);
        conditionsVo.LfConditions = formLfLikeConditions;
        LaunchParameters(conditionsVo, vo);
        return conditionsVo;
    }

    public abstract void OnInitData(T vo);

    public abstract void OnQueryData(T vo);

    public abstract void OnSubmitData(T vo);

    public abstract void OnConsentData(T vo);

    public abstract void OnBackToModifyData(T vo);

    public abstract void OnCancellationData(T vo);
    public abstract void OnFinishData(BusinessDataVo vo);

    private IDictionary<String, Object> FormLFLikeConditions(BusinessDataVo businessDataVo, List<String> fieldNames)
    {
        if (fieldNames.IsEmpty())
        {
            return null;
        }

        IDictionary<String, Object> conditions = new Dictionary<string, object>();
        foreach (string fieldName in fieldNames)
        {
            PropertyInfo? prop = businessDataVo.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            object? value = prop.GetValue(businessDataVo);
            if (value != null)
            {
                conditions.Add(fieldName, value);
            }
        }
        return conditions;
    }
}
