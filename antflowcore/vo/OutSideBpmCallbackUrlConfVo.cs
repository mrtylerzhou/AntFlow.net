using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AntFlowCore.Entity;

namespace AntFlowCore.Vo
{
    public class OutSideBpmCallbackUrlConfVo
    {
        /// <summary>
        /// auto incr id
        /// </summary>
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        /// <summary>
        /// business party id
        /// </summary>
        [JsonPropertyName("businessPartyId")]
        public long? BusinessPartyId { get; set; }
        [JsonPropertyName("applicationId")]
        public long? ApplicationId { get; set; }
        /// <summary>
        /// conf id
        /// </summary>
        [JsonPropertyName("bpmnConfId")]
        public long? BpmnConfId { get; set; }

        /// <summary>
        /// conf call back url
        /// </summary>
        [JsonPropertyName("bpmConfCallbackUrl")]
        public string BpmConfCallbackUrl { get; set; }

        /// <summary>
        /// flow callback url
        /// </summary>
        [JsonPropertyName("bpmFlowCallbackUrl")]
        public string BpmFlowCallbackUrl { get; set; }

        /// <summary>
        /// api id
        /// </summary>
        [JsonPropertyName("apiClientId")]
        public string ApiClientId { get; set; }

        /// <summary>
        /// api-secret
        /// </summary>
        [JsonPropertyName("apiClientSecrent")]
        public string ApiClientSecrent { get; set; }

        /// <summary>
        /// status 1 for enabled, 2 for disabled
        /// </summary>
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        /// <summary>
        /// create user id
        /// </summary>
        [JsonPropertyName("createUserId")]
        public int CreateUserId { get; set; }

        /// <summary>
        /// create user name
        /// </summary>
        [JsonPropertyName("createUserName")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 0 for normal, 1 for deleted
        /// </summary>
        [JsonPropertyName("isDel")]
        public int? IsDel { get; set; }

        /// <summary>
        /// create user
        /// </summary>
        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// createtime
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
        /// status name
        /// </summary>
        [JsonPropertyName("statusName")]
        public string StatusName { get; set; }

        /// <summary>
        /// business party's name
        /// </summary>
        [JsonPropertyName("businessPartyName")]
        public string BusinessPartyName { get; set; }

        /// <summary>
        /// access type 1 for embedded, 2 for api access
        /// </summary>
        [JsonPropertyName("accessType")]
        public int AccessType { get; set; }

        /// <summary>
        /// access type name
        /// </summary>
        [JsonPropertyName("accessTypeName")]
        public string AccessTypeName { get; set; }

        /// <summary>
        /// api admin list
        /// </summary>
        [JsonPropertyName("interfaceAdmins")]
        public List<Employee> InterfaceAdmins { get; set; }

        /// <summary>
        /// bpmn name
        /// </summary>
        [JsonPropertyName("bpmnName")]
        public string BpmnName { get; set; }

        //===============>>search conditions<<===================

        /// <summary>
        /// login user's business party list
        /// </summary>
        [JsonPropertyName("businessPartyIds")]
        public List<long> BusinessPartyIds { get; set; }
        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }
    }
}
