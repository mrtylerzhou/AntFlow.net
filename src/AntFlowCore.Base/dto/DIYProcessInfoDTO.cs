using System.Text.Json.Serialization;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.dto;

public class DIYProcessInfoDTO
{
    [JsonPropertyName("key")]
    public String Key { get; set; }
    [JsonPropertyName("value")]
    public String Value { get; set; }
    [JsonPropertyName("type")]
    public String Type { get; set; }
    [JsonPropertyName("remark")]
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    /**
     * 是否包含发起人自选模块,否为不包含,true为包含
     */
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