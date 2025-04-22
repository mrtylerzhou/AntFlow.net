using System.Text.Json.Serialization;

namespace antflowcore.entity
{
    public class Role
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }
    }
}