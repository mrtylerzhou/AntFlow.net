using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class AppVersionVo
    {
        /// <summary>
        /// Gets or sets the latest version ID.
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        [JsonPropertyName("curVersion")]
        public string CurVersion { get; set; }

        /// <summary>
        /// Gets or sets the latest version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the latest version (0 for no, 1 for yes).
        /// </summary>
        [JsonPropertyName("isLatest")]
        public int IsLatest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether force update is required (0 for no, 1 for yes).
        /// </summary>
        [JsonPropertyName("isForce")]
        public int IsForce { get; set; }

        /// <summary>
        /// Gets or sets the download URL for the latest version.
        /// </summary>
        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the description of the latest version.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}