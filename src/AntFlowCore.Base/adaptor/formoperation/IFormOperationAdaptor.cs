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

    // 不同意审批时的回调
    void OnDisagreeData(T vo);

    // 驳回到修改时的回调
    void OnBackToModifyData(T vo);

    // 取消流程时的回调
    void OnCancellationData(T vo);

    // 流程结束时的回调
    void OnFinishData(BusinessDataVo vo);

    /// <summary>
    /// 用户自定义自动节点条件评估。
    /// 返回非 null 值将覆盖默认的条件评估逻辑;返回 null 则走默认逻辑。
    /// 对应 Java AbstractFormOperationAdaptor.autoCondition.
    /// </summary>
    bool? AutoCondition(T vo) => null;

    /// <summary>
    /// 自动节点条件评估。
    /// 先调用 <see cref="AutoCondition"/> 获取用户自定义结果;
    /// 若为 null,则返回 null,由调用方(NextNodeLabelsProcessor)调用
    /// <c>AutoNodeConditionEvaluator.Evaluate</c> 走默认的 DB 条件评估逻辑。
    /// 对应 Java AbstractFormOperationAdaptor.automaticCondition.
    /// </summary>
    bool? AutomaticCondition(T vo)
    {
        bool? userResult = AutoCondition(vo);
        if (userResult != null)
        {
            return userResult;
        }
        return null;
    }

    /// <summary>
    /// 自动节点动作执行。
    /// conditionResult 来自 AutomaticCondition 的结果。
    /// 默认空实现；用户可重写此方法实现自定义动作。
    /// 对应 Java FormOperationAdaptor.automaticAction.
    /// </summary>
    void AutomaticAction(T vo, bool? conditionResult) { }
}