using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using antflowcore.vo;

namespace AntFlowCore.Vo
{
    public class BpmProcessDeptVo 
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processCode")]
        public string ProcessCode { get; set; }

        [JsonPropertyName("processType")]
        public int ProcessType { get; set; }

        [JsonPropertyName("processTypeName")]
        public string ProcessTypeName { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("deptId")]
        public long DeptId { get; set; }

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("createUser")]
        public long CreateUser { get; set; }

        [JsonPropertyName("updateUser")]
        public long UpdateUser { get; set; }

        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }

        [JsonPropertyName("processKey")]
        public string ProcessKey { get; set; }

        [JsonPropertyName("deptName")]
        public string DeptName { get; set; }

        [JsonPropertyName("controlDeptIdList")]
        public List<BaseIdTranStruVo> ControlDeptIdList { get; set; }

        [JsonPropertyName("controlUserIdList")]
        public List<BaseIdTranStruVo> ControlUserIdList { get; set; }

        [JsonPropertyName("controlDeptIds")]
        public List<long> ControlDeptIds { get; set; }

        [JsonPropertyName("controlUserIds")]
        public List<string> ControlUserIds { get; set; }

        [JsonPropertyName("createDeptList")]
        public List<BaseIdTranStruVo> CreateDeptList { get; set; }

        [JsonPropertyName("createDeptIds")]
        public List<long> CreateDeptIds { get; set; }

        [JsonPropertyName("createUserList")]
        public List<BaseIdTranStruVo> CreateUserList { get; set; }

        [JsonPropertyName("createUserIds")]
        public List<string> CreateUserIds { get; set; }

        [JsonPropertyName("notifyTypeList")]
        public List<BaseIdTranStruVo> NotifyTypeList { get; set; }

        [JsonPropertyName("notifyTypeIds")]
        public List<int> NotifyTypeIds { get; set; }

        [JsonPropertyName("remindTypeIds")]
        public List<int> RemindTypeIds { get; set; }

        [JsonPropertyName("remindTypeList")]
        public List<BaseIdTranStruVo> RemindTypeList { get; set; }

        [JsonPropertyName("viewUserIds")]
        public List<string> ViewUserIds { get; set; }

        [JsonPropertyName("viewUserList")]
        public List<BaseIdTranStruVo> ViewUserList { get; set; }

        [JsonPropertyName("viewdeptIds")]
        public List<long> ViewDeptIds { get; set; }

        [JsonPropertyName("viewdeptList")]
        public List<BaseIdTranStruVo> ViewDeptList { get; set; }

        [JsonPropertyName("nodeIds")]
        public List<string> NodeIds { get; set; }

        [JsonPropertyName("noticeTime")]
        public int NoticeTime { get; set; }

        [JsonPropertyName("processNodeList")]
        public List<ProcessNodeVo> ProcessNodeList { get; set; }

        [JsonPropertyName("search")]
        public string Search { get; set; }

        [JsonPropertyName("isAll")]
        public int IsAll { get; set; }

        [JsonPropertyName("iconId")]
        public int IconId { get; set; }

        [JsonPropertyName("createOfficeIds")]
        public List<long> CreateOfficeIds { get; set; }

        [JsonPropertyName("createOfficeList")]
        public List<BaseIdTranStruVo> CreateOfficeList { get; set; }

        [JsonPropertyName("viewOfficeIds")]
        public List<long> ViewOfficeIds { get; set; }

        [JsonPropertyName("viewOfficeList")]
        public List<BaseIdTranStruVo> ViewOfficeList { get; set; }
    }
    
}
