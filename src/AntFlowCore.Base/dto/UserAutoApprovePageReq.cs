using AntFlowCore.Base.dto;

namespace AntFlowCore.Base.dto
{
    /// <summary>
    /// 自动审批设置列表查询请求. 对应 Java UserAutoApprovePageReq.
    /// </summary>
    public class UserAutoApprovePageReq
    {
        public PageDto PageDto { get; set; }

        /// <summary>
        /// 归属人姓名(模糊)
        /// </summary>
        public string OwnerUserName { get; set; }

        /// <summary>
        /// formCode(模糊)
        /// </summary>
        public string FormCode { get; set; }
    }
}
