using antflowcore.vo;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class OutSideBpmBusinessPartyVo
    {
        /// <summary>
        /// auto incr id
        /// </summary>
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        /// <summary>
        /// business party mark
        /// </summary>
        [JsonPropertyName("businessPartyMark")]
        public string BusinessPartyMark { get; set; }

        /// <summary>
        /// business party name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 业务方类型（1-嵌入式；2-接入式）
        /// </summary>
        [JsonPropertyName("accessType")]
        public int? Type { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 0 for normal, 1 for deleted
        /// </summary>
        [JsonPropertyName("isDel")]
        public int IsDel { get; set; }

        /// <summary>
        /// create user
        /// </summary>
        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// create time
        /// </summary>
        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// update user
        /// </summary>
        [JsonPropertyName("updateUser")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// update time
        /// </summary>
        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }

        //===============>>ext fields<<===================

        /// <summary>
        /// type name
        /// </summary>
        [JsonPropertyName("accessTypeName")]
        public string TypeName { get; set; }

        /// <summary>
        /// process admin
        /// </summary>
        [JsonPropertyName("processAdminsStr")]
        public string ProcessAdminsStr { get; set; }

        /// <summary>
        /// process admin list
        /// </summary>
        [JsonPropertyName("processAdmins")]
        public List<BaseIdTranStruVo> ProcessAdmins { get; set; }

        /// <summary>
        /// process admin id list
        /// </summary>
        [JsonPropertyName("processAdminIds")]
        public List<string> ProcessAdminIds { get; set; }

        /// <summary>
        /// application admin
        /// </summary>
        [JsonPropertyName("applicationAdminsStr")]
        public string ApplicationAdminsStr { get; set; }

        /// <summary>
        /// application admin list
        /// </summary>
        [JsonPropertyName("applicationAdmins")]
        public List<BaseIdTranStruVo> ApplicationAdmins { get; set; }

        /// <summary>
        /// application admin id list
        /// </summary>
        [JsonPropertyName("applicationAdminIds")]
        public List<long> ApplicationAdminIds { get; set; }

        /// <summary>
        /// api admin
        /// </summary>
        [JsonPropertyName("interfaceAdminsStr")]
        public string InterfaceAdminsStr { get; set; }

        /// <summary>
        /// api admin list
        /// </summary>
        [JsonPropertyName("interfaceAdmins")]
        public List<BaseIdTranStruVo> InterfaceAdmins { get; set; }

        /// <summary>
        /// api admin id list
        /// </summary>
        [JsonPropertyName("interfaceAdminIds")]
        public List<long> InterfaceAdminIds { get; set; }

        /// <summary>
        /// template admin
        /// </summary>
        [JsonPropertyName("templateAdminsStr")]
        public string TemplateAdminsStr { get; set; }

        /// <summary>
        /// template admin list
        /// </summary>
        [JsonPropertyName("templateAdmins")]
        public List<BaseIdTranStruVo> TemplateAdmins { get; set; }

        /// <summary>
        /// template admin id list
        /// </summary>
        [JsonPropertyName("templateAdminIds")]
        public List<long> TemplateAdminIds { get; set; }

        //===============>>condition<<===================

        /// <summary>
        /// search condition
        /// </summary>
        [JsonPropertyName("search")]
        public string Search { get; set; }
    }
}