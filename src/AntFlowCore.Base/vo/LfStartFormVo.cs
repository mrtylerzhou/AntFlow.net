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

    /// <summary>
    /// 发起人节点的表单字段权限控制列表
    /// 从发起人节点(nodeType=1)的 node_config_json.lowCodeConf 中提取
    /// </summary>
    [JsonPropertyName("lfFieldControlVOs")]
    public List<LFFieldControlVO> LfFieldControlVOs { get; set; }

    /// <summary>
    /// 发起人节点的外部表单整表隐藏标记
    /// Key = formdataId, Value = true 表示该表单在发起时整表隐藏
    /// </summary>
    [JsonPropertyName("formHidden")]
    public Dictionary<string, bool> FormHidden { get; set; }
}
