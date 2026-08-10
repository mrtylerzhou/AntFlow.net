using AntFlowCore.Base.vo;
using System.Collections.Generic;

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

    /// <summary>
    /// 到达前设置(动态审批人): 节点运行期到达时, 由具体业务实现根据业务数据动态查询当前节点的真实审批人.
    /// <para>触发条件: 节点审批人为虚拟人 <c>AFSpecialAssigneeEnum.ARRIVAL_DYNAMIC_ASSIGNEE(-5)</c>.</para>
    /// <para>引擎在 BpmnTaskListener 检测到 assignee==-5 时, 通过 FormFactory.GetFormAdaptor 拿到本适配器并调用本方法,
    /// 将虚拟人任务委托(setAssignee)给查到的真人; 多人时首个承接, 其余加签; 返回 null/空时跳过当前虚拟人节点.</para>
    /// <para>默认返回 null(未实现"到达前设置"), 由引擎按"查不到人"处理(跳过).
    /// DIY 流程在自身适配器里重写; 低代码流程在 ILFFormOperationAdaptor 实现类里重写,
    /// 由 LowFlowApprovalService.ProvideCurrentNodeAssignees 内联分发并返回其结果.</para>
    /// 对应 Java FormOperationAdaptor.provideCurrentNodeAssignees.
    /// </summary>
    List<BaseIdTranStruVo>? ProvideCurrentNodeAssignees(T vo) => null;
}
