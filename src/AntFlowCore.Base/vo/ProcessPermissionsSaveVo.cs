namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 流程权限管理批量保存请求. 对应 Java ProcessPermissionsSaveVo.
    /// 三层笛卡尔积: processKeys × 授权对象(objectIds) × permissionsTypes
    /// </summary>
    public class ProcessPermissionsSaveVo
    {
        /// <summary>
        /// 流程 formCode 集合
        /// </summary>
        public List<string> ProcessKeys { get; set; }

        /// <summary>
        /// 权限类型集合(1查看 2创建 3监控)
        /// </summary>
        public List<int> PermissionsTypes { get; set; }

        /// <summary>
        /// 授权对象类型 true=部门权限 false=人员权限(兼容旧调用, 新调用请使用 ObjectType)
        /// </summary>
        public bool? IsDepartment { get; set; }

        /// <summary>
        /// 授权对象类型 1=人员 2=部门 3=角色(优先于 IsDepartment)
        /// </summary>
        public int? ObjectType { get; set; }

        /// <summary>
        /// 授权对象 id 集合(人员id/部门id/角色id, 与 ObjectType 对应)
        /// </summary>
        public List<string> ObjectIds { get; set; }
    }
}