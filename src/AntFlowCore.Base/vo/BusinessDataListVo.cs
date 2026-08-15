namespace AntFlowCore.Base.vo;

/// <summary>
/// 业务数据动态列表返回. 对应 Java BusinessDataListVo.
/// columns + rows + total 结构,前端直接渲染.
/// </summary>
public class BusinessDataListVo
{
    /// <summary>
    /// 列定义(固定列在前,动态列按配置表id排序在后)
    /// </summary>
    public List<BusinessDataColumnVo> Columns { get; set; } = new();

    /// <summary>
    /// 行数据(key-value)
    /// </summary>
    public List<Dictionary<string, object?>> Rows { get; set; } = new();

    /// <summary>
    /// 总条数
    /// </summary>
    public long Total { get; set; }

    public class BusinessDataColumnVo
    {
        /// <summary>
        /// 列key(固定列用固定字段名,动态列用 field_{fieldId})
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// 是否固定列(流程编号等)
        /// </summary>
        public bool Fixed { get; set; }
    }
}