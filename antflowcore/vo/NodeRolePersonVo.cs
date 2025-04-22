using antflowcore.vo;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class NodeRolePersonVo
    {
        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        [JsonPropertyName("roleId")]
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the user list.
        /// </summary>
        [JsonPropertyName("userList")]
        public List<BaseIdTranStruVo> UserList { get; set; }
    }
}