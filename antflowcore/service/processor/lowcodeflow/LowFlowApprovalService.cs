using antflowcore.adaptor;
using antflowcore.factory;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.lowcodeflow;

[AfFormServiceAnno(SvcName = "LF",Desc = "低(无)代码流程")]
public class LowFlowApprovalService: IFormOperationAdaptor<UDLFApplyVo>
{
    public BpmnStartConditionsVo PreviewSetCondition(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public BpmnStartConditionsVo LaunchParameters(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnInitData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnQueryData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnSubmitData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnConsentData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnBackToModifyData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnCancellationData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnFinishData(BusinessDataVo vo)
    {
        throw new NotImplementedException();
    }
}
