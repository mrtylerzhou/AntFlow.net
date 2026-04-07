using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class BpmFlowrunEntrustVo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("runinfoid")]
        public string RuninfoId { get; set; }

        [JsonPropertyName("runtaskid")]
        public string RunTaskId { get; set; }

        [JsonPropertyName("original")]
        public int Original { get; set; }

        [JsonPropertyName("actual")]
        public int Actual { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("id_del")]
        public int IdDel { get; set; }

        [JsonPropertyName("procDefId")]
        public string ProcDefId { get; set; }

        [JsonPropertyName("isView")]
        public int IsView { get; set; }
    }
}