using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.adaptor.formoperation;

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

    /// <summary>
    /// 自动节点条件评估。
    /// 返回 true=条件满足，false=不满足，null=无条件（直接执行动作）。
    /// 默认实现返回 null（无条件执行）；用户可重写自定义逻辑。
    /// 对应 Java FormOperationAdaptor.automaticCondition.
    /// </summary>
    bool? AutomaticCondition(T vo) => null;

    /// <summary>
    /// 自动节点动作执行。
    /// conditionResult 来自 AutomaticCondition 的结果。
    /// 默认空实现；用户可重写此方法实现自定义动作。
    /// 对应 Java FormOperationAdaptor.automaticAction.
    /// </summary>
    void AutomaticAction(T vo, bool? conditionResult) { }
}