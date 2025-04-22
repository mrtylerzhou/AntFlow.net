using System;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class BaseApplicationVo
    {
        /// <summary>
        /// Gets or sets the ID of the application.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the look URL.
        /// </summary>
        [JsonPropertyName("lookUrl")]
        public string LookUrl { get; set; }

        /// <summary>
        /// Gets or sets the submit URL.
        /// </summary>
        [JsonPropertyName("submitUrl")]
        public string SubmitUrl { get; set; }

        /// <summary>
        /// Gets or sets the condition URL.
        /// </summary>
        [JsonPropertyName("conditionUrl")]
        public string ConditionUrl { get; set; }

        /// <summary>
        /// Gets or sets the primary key ID.
        /// </summary>
        [JsonPropertyName("pkId")]
        public int? PkId { get; set; }
    }
}