using AntFlowCore.Constants;

namespace antflowcore.entity
{
    /// <summary>
    /// ThirdPartyAccountApply 实体类
    /// </summary>
    public class ThirdPartyAccountApply
    {
        /// <summary>
        /// 主键 ID，自动递增
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// 账户所有者名称
        /// </summary>
        public string AccountOwnerName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    }
}