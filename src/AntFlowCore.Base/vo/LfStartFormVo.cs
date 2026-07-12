using System.Text.Json.Serialization;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 发起流程时的表单数据响应 VO
/// 兼容内联表单(单表单)和外部表单(多表单)两种模式
/// </summary>
public class LfStartFormVo
{
    /// <summary>
    /// 是否使用外部表单模式
    /// </summary>
    [JsonPropertyName("useExternalForm")]
    public bool? UseExternalForm { get; set; }

    /// <summary>
    /// 内联表单模式: 单个表单定义 JSON 字符串
    /// 外部表单模式: null
    /// </summary>
    [JsonPropertyName("lfFormData")]
    public string LfFormData { get; set; }

    /// <summary>
    /// 外部表单模式: 引用的表单版本列表(含 formdata JSON 定义)
    /// 内联表单模式: null
    /// </summary>
    [JsonPropertyName("lfFormdataList")]
    public List<BpmnConfLfFormdata> LfFormdataList { get; set; }
}
