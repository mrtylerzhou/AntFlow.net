using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace antflowcore.vo
{
    public class BpmnTemplateVo
    {
        public long Id { get; set; }

        [JsonPropertyName("confId")]
        public long ConfId { get; set; }

        [JsonPropertyName("nodeId")]
        public long NodeId { get; set; }

        public int Event { get; set; }

        [JsonPropertyName("eventValue")]
        public string EventValue { get; set; }

        [JsonPropertyName("informs")]
        public string Informs { get; set; }

        [JsonPropertyName("informIdList")]
        public List<string> InformIdList { get; set; }

        [JsonPropertyName("informList")]
        public List<BaseIdTranStruVo> InformList { get; set; }

        [JsonPropertyName("emps")]
        public string Emps { get; set; }

        [JsonPropertyName("empIdList")]
        public List<string> EmpIdList { get; set; }

        [JsonPropertyName("empList")]
        public List<BaseIdTranStruVo> EmpList { get; set; }

        [JsonPropertyName("roles")]
        public string Roles { get; set; }

        [JsonPropertyName("roleIdList")]
        public List<string> RoleIdList { get; set; }

        [JsonPropertyName("roleList")]
        public List<BaseIdTranStruVo> RoleList { get; set; }

        [JsonPropertyName("funcs")]
        public string Funcs { get; set; }

        [JsonPropertyName("funcIdList")]
        public List<string> FuncIdList { get; set; }

        [JsonPropertyName("funcList")]
        public List<BaseIdTranStruVo> FuncList { get; set; }

        [JsonPropertyName("templateId")]
        public long TemplateId { get; set; }

        [JsonPropertyName("templateName")]
        public string TemplateName { get; set; }

        public int IsDel { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }

        [JsonPropertyName("updateUser")]
        public string UpdateUser { get; set; }

        // 默认构造函数
        public BpmnTemplateVo() { }
    }
}
