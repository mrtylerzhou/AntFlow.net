namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 流程权限批量保存结果(新增/跳过统计). 对应 Java ProcessPermissionsSaveResult.
    /// </summary>
    public class ProcessPermissionsSaveResult
    {
        /// <summary>
        /// 新增条数
        /// </summary>
        public int InsertCount { get; set; }

        /// <summary>
        /// 重复跳过条数
        /// </summary>
        public int SkipCount { get; set; }
    }
}