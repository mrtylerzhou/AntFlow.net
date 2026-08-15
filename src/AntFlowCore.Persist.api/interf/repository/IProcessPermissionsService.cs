using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository
{
    /// <summary>
    /// 流程权限管理 业务服务接口. 对应 Java ProcessPermissionsBizServiceImpl.
    /// </summary>
    public interface IProcessPermissionsService
    {
        /// <summary>
        /// 分页列表(后置补全流程名/对象名/创建人名)
        /// </summary>
        ResultAndPage<ProcessPermissionsListVo> ListPage(ProcessPermissionsPageReq req);

        /// <summary>
        /// 批量保存(流程×授权对象×权限类型 笛卡尔积, 已存在跳过)
        /// </summary>
        ProcessPermissionsSaveResult Save(ProcessPermissionsSaveVo vo);

        /// <summary>
        /// 删除(物理)
        /// </summary>
        void Delete(long id);
    }
}