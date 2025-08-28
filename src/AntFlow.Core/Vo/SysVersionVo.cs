using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class SysVersionVo
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("create_time")] public DateTime? CreateTime { get; set; }

    [JsonPropertyName("update_time")] public DateTime? UpdateTime { get; set; }

    [JsonPropertyName("is_del")] public int IsDel { get; set; }

    [JsonPropertyName("version")] public string Version { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("index")] public int Index { get; set; }

    [JsonPropertyName("is_force")] public int IsForce { get; set; }

    [JsonPropertyName("is_hide")] public int IsHide { get; set; }

    [JsonPropertyName("android_url")] public string AndroidUrl { get; set; }

    [JsonPropertyName("ios_url")] public string IosUrl { get; set; }

    [JsonPropertyName("create_user")] public string CreateUser { get; set; }

    [JsonPropertyName("update_user")] public string UpdateUser { get; set; }

    [JsonPropertyName("download_code")] public string DownloadCode { get; set; }

    [JsonPropertyName("data")] public List<BaseIdTranStruVo> Data { get; set; }

    [JsonPropertyName("application")] public List<BaseIdTranStruVo> Application { get; set; }

    [JsonPropertyName("data_ids")] public List<long> DataIds { get; set; }

    [JsonPropertyName("app_ids")] public List<long> AppIds { get; set; }

    [JsonPropertyName("effective_time")] public string EffectiveTime { get; set; }

    [JsonPropertyName("quick_entry_ids")] public List<long> QuickEntryIds { get; set; }

    [JsonPropertyName("quick_entry_list")] public List<BaseIdTranStruVo> QuickEntryList { get; set; }
}