namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 流程权限管理列表行. 对应 Java ProcessPermissionsListVo.
    /// </summary>
    public class ProcessPermissionsListVo
    {
        public long Id { get; set; }

        /// <summary>
        /// 流程 formCode
        /// </summary>
        public string ProcessKey { get; set; }

        /// <summary>
        /// 流程名称(后置处理补全)
        /// </summary>
        public string BpmnName { get; set; }

        /// <summary>
        /// 权限类型 1查看 2创建 3监控
        /// </summary>
        public int? PermissionsType { get; set; }

        /// <summary>
        /// 授权对象类型 true=部门 false=人员(兼容旧字段)
        /// </summary>
        public bool? IsDepartment { get; set; }

        /// <summary>
        /// 授权对象类型 1=人员 2=部门 3=角色
        /// </summary>
        public int? ObjectType { get; set; }

        /// <summary>
        /// 授权对象名称(后置处理补全)
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// 创建人 id
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建人名称(后置处理补全)
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}