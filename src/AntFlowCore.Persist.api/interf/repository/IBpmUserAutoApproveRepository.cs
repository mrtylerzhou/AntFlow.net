using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

/// <summary>
/// 用户自动审批设置 仓储接口. 对应 Java BpmUserAutoApproveMapper.
/// </summary>
public interface IBpmUserAutoApproveRepository : IBaseRepository<BpmUserAutoApprove>
{
    /// <summary>
    /// 分页查询(按id倒序)
    /// </summary>
    List<BpmUserAutoApprove> QueryPageList(string ownerUserName, string ownerUserId, string formCode, string tenantId, Page<BpmUserAutoApprove> page);
}
