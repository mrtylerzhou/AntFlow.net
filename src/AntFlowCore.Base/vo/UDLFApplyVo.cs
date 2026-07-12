using System.Text.Json.Serialization;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;

namespace AntFlowCore.Base.vo;

public class UDLFApplyVo: BusinessDataVo
{
    [JsonPropertyName("remark")]
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    [JsonPropertyName("lfFields")]
    public Dictionary<String,Object> LfFields { get; set; }
    [JsonPropertyName("lfFormData")]
    public String LfFormData { get; set; }

    /// <summary>
    /// 多表单模式: 按表单版本id分组的字段值
    /// Key = formdataId(字符串形式, 保证 JSON 合法), Value = 该表单的字段值Map&lt;fieldId, value&gt;
    /// 仅外部表单模式使用; 内联模式为 null
    /// </summary>
    [JsonPropertyName("lfFieldsMulti")]
    public Dictionary<string, Dictionary<string, object>> LfFieldsMulti { get; set; }

    /// <summary>
    /// 多表单模式: 引用的表单版本列表(含formdata JSON定义),供前端渲染多tab
    /// 由 queryData 在外部表单模式下填充; 内联模式为 null(使用 lfFormData)
    /// </summary>
    [JsonPropertyName("lfFormdataList")]
    public List<BpmnConfLfFormdata> LfFormdataList { get; set; }
}