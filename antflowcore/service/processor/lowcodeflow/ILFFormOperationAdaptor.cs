using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.lowcodeflow;

public interface ILFFormOperationAdaptor{
    // 初始化数据
    void OnInitData(UDLFApplyVo vo);

    // 查询数据
    void OnQueryData(UDLFApplyVo vo);

    // 提交数据
    void OnSubmitData(UDLFApplyVo vo);

    // 同意审批时的回调
    void OnConsentData(UDLFApplyVo vo);

    // 驳回到修改时的回调
    void OnBackToModifyData(UDLFApplyVo vo);

    // 取消流程时的回调
    void OnCancellationData(UDLFApplyVo vo);

    // 流程结束时的回调
    void OnFinishData(BusinessDataVo vo);
}