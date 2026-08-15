using AntFlowCore.Base.dto;

namespace AntFlowCore.Base.dto
{
    /// <summary>
    /// 流程权限管理列表查询请求. 对应 Java ProcessPermissionsPageReq.
    /// </summary>
    public class ProcessPermissionsPageReq
    {
        public PageDto PageDto { get; set; }

        /// <summary>
        /// 流程 formCode(模糊)
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 权限类型 1查看 2创建 3监控
        /// </summary>
        public int? PermissionsType { get; set; }

        /// <summary>
        /// 授权对象名称(人员姓名/部门名称,模糊, 未传ObjectId时生效)
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// 授权对象类型 1=人员 2=部门 3=角色(与ObjectId配合精确过滤)
        /// </summary>
        public int? ObjectType { get; set; }

        /// <summary>
        /// 授权对象 id(精确过滤, 优先于 ObjectName)
        /// </summary>
        public string ObjectId { get; set; }
    }
}