using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

public interface IFormOperationAdaptor<in T> where T : BusinessDataVo
{
    // 设置预览条件
    BpmnStartConditionsVo PreviewSetCondition(T vo);

    // 启动参数
    BpmnStartConditionsVo LaunchParameters(T vo);

    // 初始化数据
    void OnInitData(T vo);

    // 查询数据
    void OnQueryData(T vo);

    // 提交数据
    void OnSubmitData(T vo);

    // 同意审批时的回调
    void OnConsentData(T vo);

    // 驳回到修改时的回调
    void OnBackToModifyData(T vo);

    // 取消流程时的回调
    void OnCancellationData(T vo);

    // 流程结束时的回调
    void OnFinishData(BusinessDataVo vo);
}