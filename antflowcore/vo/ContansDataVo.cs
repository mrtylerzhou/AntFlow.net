using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class ContansDataVo
    {
        /// <summary>
        /// Data ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Data name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Task ID
        /// </summary>
        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }

        // Constructor
        public ContansDataVo() { }
        
    }
}