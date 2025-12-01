using System;
using System.Text.Json.Serialization;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entities
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