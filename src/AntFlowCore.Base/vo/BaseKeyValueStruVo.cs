using System.Text.Json.Serialization;
using AntFlowCore.Base.conf.json;
using AntFlowCore.Base.util;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.vo;

public class BaseKeyValueStruVo
{
    [JsonPropertyName("key"),JsonConverter(typeof(IntToStringConverter))] 
    public string Key { get; set; }
    [JsonPropertyName("value")] 
    public string Value { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("remark")] 
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    [JsonPropertyName("createTime")] 
    public DateTime CreateTime { get; set; }

    [JsonPropertyName("hasStarUserChooseModule")]
    public bool HasStarUserChooseModule { get; set; } = false;

    /// <summary>
    /// 流程通知渠道列表(邮件/短信/app推送/企微/钉钉/飞书),active 标记是否启用
    /// </summary>
    [JsonPropertyName("processNotices")]
    public List<BaseNumIdStruVo> ProcessNotices { get; set; }

    /// <summary>
    /// 通知模板配置列表
    /// </summary>
    [JsonPropertyName("templateVos")]
    public List<BpmnTemplateVo> TemplateVos { get; set; }
}