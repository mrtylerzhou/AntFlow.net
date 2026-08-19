namespace AntFlowCore.Base.vo;

/// <summary>
/// 发起流程页 分类块. 对应 Java StartFlowCategoryVo.
/// 每块 = 分类标题 + 流程卡片列表, 前端按 3 栏布局展示.
/// </summary>
public class StartFlowCategoryVo
{
    /// <summary>
    /// 分类 id(null 表示未分类)
    /// </summary>
    public long? CategoryId { get; set; }

    /// <summary>
    /// 分类名称(未分类固定为「未分类」)
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 所在栏索引(0/1/2)
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// 分类下的流程卡片列表(按创建时间 asc)
    /// </summary>
    public List<StartFlowVo> Flows { get; set; } = new();

    /// <summary>
    /// 单个流程卡片
    /// </summary>
    public class StartFlowVo
    {
        /// <summary>
        /// 表单编码
        /// </summary>
        public string? FormCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string? BpmnName { get; set; }

        /// <summary>
        /// 类型: OUTSIDE / LF / DIY
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 关联的 app 应用 id(外部流程跳转用)
        /// </summary>
        public long? ApplicationId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
