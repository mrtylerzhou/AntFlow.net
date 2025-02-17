using System.Text.Json.Serialization;

namespace antflowcore.vo
{
    public class LFFieldControlVO
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        [JsonPropertyName("nodeId")]
        public long NodeId { get; set; }

        /// <summary>
        /// 表单数据ID
        /// </summary>
        [JsonPropertyName("formdataId")]
        public long FormdataId { get; set; }

        /// <summary>
        /// 字段id
        /// </summary>
        [JsonPropertyName("fieldId")]
        public string FieldId { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }

        /// <summary>
        /// 字段权限
        /// </summary>
        [JsonPropertyName("perm")]
        public string Perm { get; set; }
    }
}