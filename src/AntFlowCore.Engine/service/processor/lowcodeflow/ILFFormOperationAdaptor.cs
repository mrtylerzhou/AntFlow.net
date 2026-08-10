using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using System.Collections.Generic;

namespace AntFlowCore.Engine.service.processor.lowcodeflow;

public interface ILFFormOperationAdaptor{
    // 初始化数据
    void OnInitData(UDLFApplyVo vo);

    // 查询数据
    void OnQueryData(UDLFApplyVo vo);

    // 提交数据
    void OnSubmitData(UDLFApplyVo vo);

    // 同意审批时的回调
    void OnConsentData(UDLFApplyVo vo);

    // 不同意审批时的回调
    void OnDisagreeData(UDLFApplyVo vo);

    // 驳回到修改时的回调
    void OnBackToModifyData(UDLFApplyVo vo);

    // 取消流程时的回调
    void OnCancellationData(UDLFApplyVo vo);

    // 流程结束时的回调
    void OnFinishData(BusinessDataVo vo);

    /// <summary>
    /// 到达前设置(动态审批人): 低代码流程具体实现动态查询当前节点真实审批人.
    /// 由 LowFlowApprovalService.ProvideCurrentNodeAssignees 内联分发(按 LFFormServiceAnnoAttribute.SvcName==formCode 匹配)并返回其结果.
    /// 默认返回 null(未实现), 引擎按"查不到人"跳过. 对应 Java LFFormOperationAdaptor.provideCurrentNodeAssignees.
    /// </summary>
    List<BaseIdTranStruVo>? ProvideCurrentNodeAssignees(UDLFApplyVo vo) => null;
}