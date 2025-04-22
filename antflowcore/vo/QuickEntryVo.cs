using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class QuickEntryVo
    {
        /// <summary>
        /// Entry ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Application title
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Effective image URL
        /// </summary>
        [JsonPropertyName("effectiveSource")]
        public string EffectiveSource { get; set; }

        /// <summary>
        /// Is shown (not deleted)
        /// </summary>
        [JsonPropertyName("isDel")]
        public int IsDel { get; set; }

        /// <summary>
        /// Status name
        /// </summary>
        [JsonPropertyName("stateName")]
        public string StateName { get; set; }

        /// <summary>
        /// Request route
        /// </summary>
        [JsonPropertyName("route")]
        public string Route { get; set; }

        /// <summary>
        /// Sort order
        /// </summary>
        [JsonPropertyName("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Types
        /// </summary>
        [JsonPropertyName("types")]
        public List<int> Types { get; set; }

        /// <summary>
        /// Type IDs
        /// </summary>
        [JsonPropertyName("typeIds")]
        public string TypeIds { get; set; }

        /// <summary>
        /// Type name
        /// </summary>
        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

        /// <summary>
        /// 0 for not in use, 1 for in use
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        /// Variable URL flag
        /// </summary>
        [JsonPropertyName("variableUrlFlag")]
        public int VariableUrlFlag { get; set; }
    }
}