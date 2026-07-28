using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.formoperation;

public abstract class AbstractLowFlowSpyFormOperationAdaptor<T> : IFormOperationAdaptor<T> where T : BusinessDataVo
{
    public abstract void PreviewSetCondition(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public BpmnStartConditionsVo PreviewSetCondition(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        PreviewSetCondition(conditionsVo, vo);
        return conditionsVo;
    }

    public abstract void LaunchParameters(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public BpmnStartConditionsVo LaunchParameters(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        LaunchParameters(conditionsVo, vo);
        return conditionsVo;
    }

    public abstract void OnInitData(T vo);

    public abstract void OnQueryData(T vo);

    public abstract void OnSubmitData(T vo);

    public abstract void OnConsentData(T vo);

    public abstract void OnDisagreeData(T vo);

    public abstract void OnBackToModifyData(T vo);

    public abstract void OnCancellationData(T vo);
    public abstract void OnFinishData(BusinessDataVo vo);
}
