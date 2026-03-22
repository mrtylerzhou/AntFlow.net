
using System.Text.Json.Serialization;
using antflowcore.constant.enums;
using Antflowcore.Vo;

namespace antflowcore.vo
{
    public class BpmnNodePropertysVo
    {
        [JsonPropertyName("loopEndType")]
        public int? LoopEndType { get; set; }

        [JsonPropertyName("loopNumberPlies")]
        public int? LoopNumberPlies { get; set; }

        [JsonPropertyName("loopEndGrade")]
        public int? LoopEndGrade { get; set; }

        [JsonPropertyName("loopEndPersonList")]
        public List<string> LoopEndPersonList { get; set; }

        [JsonPropertyName("loopEndPersonObjList")]
        public List<BaseIdTranStruVo> LoopEndPersonObjList { get; set; }

        [JsonPropertyName("assignLevelType")]
        public int? AssignLevelType { get; set; }

        [JsonPropertyName("assignLevelGrade")]
        public int? AssignLevelGrade { get; set; }

        [JsonPropertyName("hrbpConfType")]
        public int? HrbpConfType { get; set; }

        [JsonPropertyName("roleIds")]
        public List<string> RoleIds { get; set; }

        [JsonPropertyName("roleList")]
        public List<BaseIdTranStruVo> RoleList { get; set; }

        [JsonPropertyName("emplIds")]
        public List<string> EmplIds { get; set; }

        [JsonPropertyName("emplList")]
        public List<BaseIdTranStruVo> EmplList { get; set; }

        [JsonPropertyName("signType")]
        public int? SignType { get; set; }

        [JsonPropertyName("conditionsConf")]
        public BpmnNodeConditionsConfBaseVo ConditionsConf { get; set; }

        [JsonPropertyName("conditionList")]
        public List<List<BpmnNodeConditionsConfVueVo>> ConditionList { get; set; }

        [JsonPropertyName("configurationTableType")]
        public int? ConfigurationTableType { get; set; }

        [JsonPropertyName("tableFieldType")]
        public int? TableFieldType { get; set; }

        [JsonPropertyName("isMultiPeople")]
        public int? IsMultiPeople { get; set; }

        [JsonPropertyName("noparticipatingStaffIds")]
        public List<string> NoparticipatingStaffIds { get; set; }

        [JsonPropertyName("noparticipatingStaffs")]
        public List<BaseIdTranStruVo> NoparticipatingStaffs { get; set; }

        [JsonPropertyName("functionId")]
        public long? FunctionId { get; set; }

        [JsonPropertyName("functionName")]
        public string FunctionName { get; set; }

        [JsonPropertyName("afterSignUpWay")]
        public int AfterSignUpWay { get; set; }

        [JsonPropertyName("signUpType")]
        public int SignUpType { get; set; }

        [JsonPropertyName("nodeMark")]
        public string NodeMark { get; set; }

        [JsonPropertyName("isDefault")]
        public int? IsDefault { get; set; }

        [JsonPropertyName("sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// <see cref="ConditionRelationShipEnum"/>
        /// </summary>
        [JsonPropertyName("groupRelation")]
        public bool GroupRelation { get; set; } = true;
        [JsonPropertyName("additionalSignInfoList")]
        public List<ExtraSignInfoVo> AdditionalSignInfoList { get; set; }
        
        [JsonPropertyName("contextEmplList")]
        public List<BaseIdTranStruVo> ContextEmplList { get; set; }

        /// <summary>
        /// 表单关联人员属性类型
        /// </summary>
        [JsonPropertyName("formAssigneeProperty")]
        public int? FormAssigneeProperty { get; set; }

        /// <summary>
        /// 表单元素信息列表
        /// </summary>
        [JsonPropertyName("formInfos")]
        public List<BaseIdTranStruVo> FormInfos { get; set; }

        /// <summary>
        /// 自定义规则审批人属性（自定义规则类型标识，如zdysp1表示指定人员）
        /// </summary>
        [JsonPropertyName("udrAssigneeProperty")]
        public BaseIdTranStruVo UdrAssigneeProperty { get; set; }

        /// <summary>
        /// 自定义规则值JSON（如指定的用户ID列表等，运行时由用户自定义）
        /// </summary>
        [JsonPropertyName("udrValueJson")]
        public string UdrValueJson { get; set; }

        /// <summary>
        /// 自定义扩展字段1
        /// </summary>
        [JsonPropertyName("ext1")]
        public string Ext1 { get; set; }

        /// <summary>
        /// 自定义扩展字段2
        /// </summary>
        [JsonPropertyName("ext2")]
        public string Ext2 { get; set; }

        /// <summary>
        /// 自定义扩展字段3
        /// </summary>
        [JsonPropertyName("ext3")]
        public string Ext3 { get; set; }

        /// <summary>
        /// 自定义扩展字段4
        /// </summary>
        [JsonPropertyName("ext4")]
        public string Ext4 { get; set; }

    }
}
