using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 版本关联数据保存入参. 对应 Java AppDataSaveVo.
    /// </summary>
    public class AppDataSaveVo
    {
        /// <summary>
        /// 版本id
        /// </summary>
        [JsonPropertyName("versionId")]
        public long VersionId { get; set; }

        /// <summary>
        /// 关联类型(1:图标应用 2:上线流程 3:快捷入口)
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }

        /// <summary>
        /// 关联对象列表, 按sort顺序
        /// </summary>
        [JsonPropertyName("items")]
        public List<AppDataItem> Items { get; set; }

        public class AppDataItem
        {
            /// <summary>
            /// 关联对象id(bpm_process_app_application id 或 quick_entry id)
            /// </summary>
            [JsonPropertyName("id")]
            public string Id { get; set; }

            /// <summary>
            /// 关联对象名称
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; }

            /// <summary>
            /// 排序号, 从1开始
            /// </summary>
            [JsonPropertyName("sort")]
            public int? Sort { get; set; }
        }
    }
}