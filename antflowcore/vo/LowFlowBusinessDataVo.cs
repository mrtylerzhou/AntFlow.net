namespace antflowcore.vo;

/// <summary>
/// 低代码流程业务数据VO
/// </summary>
public class LowFlowBusinessDataVo
{
    /// <summary>
    /// 主表ID
    /// </summary>
    public long MainId { get; set; }

    /// <summary>
    /// 流程配置ID
    /// </summary>
    public long? ConfId { get; set; }

    /// <summary>
    /// 表单编码
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 创建用户
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// 字段信息列表
    /// </summary>
    public List<FieldInfo> Fields { get; set; }

    /// <summary>
    /// 字段信息
    /// </summary>
    public class FieldInfo
    {
        /// <summary>
        /// 字段ID
        /// </summary>
        public string FieldId { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段标签
        /// </summary>
        public string FieldLabel { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public int? FieldType { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        public object FieldValue { get; set; }
    }
}
