using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class InformationTemplateVo
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("num")]
        public string Num { get; set; }

        [JsonPropertyName("systemTitle")]
        public string SystemTitle { get; set; }

        [JsonPropertyName("systemContent")]
        public string SystemContent { get; set; }

        [JsonPropertyName("mailTitle")]
        public string MailTitle { get; set; }

        [JsonPropertyName("mailContent")]
        public string MailContent { get; set; }

        [JsonPropertyName("noteContent")]
        public string NoteContent { get; set; }

        [JsonPropertyName("jumpUrl")]
        public int? JumpUrl { get; set; }

        [JsonPropertyName("jumpUrlValue")]
        public string JumpUrlValue { get; set; }

        [JsonPropertyName("remark")]
        public string Remark { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("statusValue")]
        public string StatusValue { get; set; }

        [JsonPropertyName("isDel")]
        public int? IsDel { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }

        [JsonPropertyName("updateUser")]
        public string UpdateUser { get; set; }

        [JsonPropertyName("wildcardCharacterMap")]
        public Dictionary<int, string> WildcardCharacterMap { get; set; }
    }
}