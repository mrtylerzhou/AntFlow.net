namespace AntFlowCore.Entity
{
    /// <summary>
    /// 用户角色关联表
    /// </summary>
    public class UserRole
    {
        public long? Id { get; set; }

        public long? UserId { get; set; }

        public long? RoleId { get; set; }
    }
}