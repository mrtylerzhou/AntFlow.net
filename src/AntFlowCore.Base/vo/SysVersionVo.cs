using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 系统版本VO. JSON命名与Java版(Jackson默认camelCase)对齐.
    /// </summary>
    public class SysVersionVo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }

        [JsonPropertyName("isDel")]
        public int? IsDel { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("indx")]
        public int? Index { get; set; }

        [JsonPropertyName("isForce")]
        public int? IsForce { get; set; }

        [JsonPropertyName("isHide")]
        public int? IsHide { get; set; }

        [JsonPropertyName("androidUrl")]
        public string AndroidUrl { get; set; }

        [JsonPropertyName("iosUrl")]
        public string IosUrl { get; set; }

        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        [JsonPropertyName("updateUser")]
        public string UpdateUser { get; set; }

        [JsonPropertyName("downloadCode")]
        public string DownloadCode { get; set; }

        [JsonPropertyName("data")]
        public List<BaseIdTranStruVo> Data { get; set; }

        [JsonPropertyName("application")]
        public List<BaseIdTranStruVo> Application { get; set; }

        [JsonPropertyName("dataIds")]
        public List<long> DataIds { get; set; }

        [JsonPropertyName("appIds")]
        public List<long> AppIds { get; set; }

        [JsonPropertyName("effectiveTime")]
        public string EffectiveTime { get; set; }

        [JsonPropertyName("quickEntryIds")]
        public List<long> QuickEntryIds { get; set; }

        [JsonPropertyName("quickEntryList")]
        public List<BaseIdTranStruVo> QuickEntryList { get; set; }

        /// <summary>
        /// 新建时是否继承上一最大index版本的全部关联数据(图标应用/上线流程/快捷入口, 含sort)
        /// </summary>
        [JsonPropertyName("inheritFromLast")]
        public bool? InheritFromLast { get; set; }
    }
}