using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessAuditService : IAntFlowRepositoryMix<BpmProcessAudit, IBpmProcessAuditRepository>
{
    /// <summary>
    /// 保存审批过程中表单字段的审计记录.
    /// 约定: 即使字段未发生变化也记录一条(便于前端无差别展示当时状态), 不做 diff 过滤.
    /// 低代码流程遍历 LfFields / LfFieldsMulti 所有 key; DIY 流程反射遍历 vo 子类自己声明的属性.
    /// 必须在 <see cref="adaptor.formoperation.IFormOperationAdaptor{T}.OnConsentData"/> 之前调用,
    /// 方法内部会调 OnQueryData 拿旧值, 结束后恢复 vo 的前端新值, 不影响后续 OnConsentData 写入.
    /// </summary>
    /// <param name="vo">业务数据 vo(含前端提交的新值)</param>
    /// <param name="task">当前审批任务(用于 taskDefKey/taskName)</param>
    void SaveChanges(BusinessDataVo vo, BpmAfTask task);

    /// <summary>
    /// 按 processNumber 查询所有审计记录, 按 taskDefKey + createTime 升序.
    /// </summary>
    List<BpmProcessAudit> GetProcessAudits(string processNumber);
}
