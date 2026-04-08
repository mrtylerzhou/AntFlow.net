using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    public class OutSideBpmProcesses
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }
    }
}