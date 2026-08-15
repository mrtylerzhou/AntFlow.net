using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository
{
    /// <summary>
    /// 流程权限仓储接口. 对应 Java BpmProcessPermissionsMapper.
    /// </summary>
    public interface IProcessPermissionsRepository : IBaseRepository<BpmProcessPermissions>
    {
        /// <summary>
        /// 分页查询权限记录(objectName 已由 Service 解析为三类 id 集合)
        /// </summary>
        List<BpmProcessPermissions> QueryPageList(ProcessPermissionsPageReq req,
            List<string> userIds, List<string> depIds, List<string> roleIds,
            string tenantId, Page<BpmProcessPermissions> page);
    }
}