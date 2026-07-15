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
    /// 自动节点条件判断.
    /// 返回值语义:
    /// - true  : 条件满足,执行 automaticAction
    /// - false : 条件不满足,执行 automaticAction
    /// - null  : 无条件配置,执行 automaticAction
    /// 默认实现返回 null(不做条件判断,交由 automaticAction 自行决定).
    /// 子类可重写以提供自定义条件评估逻辑.
    /// </summary>
    bool? AutomaticCondition(T vo) => null;

    /// <summary>
    /// 自动节点动作执行.
    /// 在 automaticCondition 评估后调用,无论条件结果如何都会执行
    /// (是否执行由具体实现决定).默认实现不做任何操作.
    /// </summary>
    void AutomaticAction(T vo, bool? conditionResult) { }
}
