using System.Text.Json.Serialization;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.vo
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
        /// Back type for disagree action (migrated from bpm_process_node_back).
        /// 1=退回上一节点提交下一节点; 2=退回发起人提交下一节点;
        /// 3=退回发起人提交回退节点; 4=退回历史节点提交下一节点; 5=退回历史节点提交回退节点
        /// </summary>
        [JsonPropertyName("backType")]
        public int? BackType { get; set; }

        /// <summary>
        /// UDR assignee property (for custom rule approver)
        /// </summary>
        [JsonPropertyName("udrAssigneeProperty")]
        public BaseIdTranStruVo UdrAssigneeProperty { get; set; }

        /// <summary>
        /// UDR value JSON string
        /// </summary>
        [JsonPropertyName("udrValueJson")]
        public string UdrValueJson { get; set; }

        /// <summary>
        /// UDR ext fields
        /// </summary>
        [JsonPropertyName("ext1")]
        public string Ext1 { get; set; }

        [JsonPropertyName("ext2")]
        public string Ext2 { get; set; }

        [JsonPropertyName("ext3")]
        public string Ext3 { get; set; }

        [JsonPropertyName("ext4")]
        public string Ext4 { get; set; }

        /// <summary>
        /// Form related user - form infos (serialized as JSON for valueJson)
        /// </summary>
        [JsonPropertyName("formInfos")]
        public List<BaseIdTranStruVo> FormInfos { get; set; }

        /// <summary>
        /// Form assignee property type
        /// </summary>
        [JsonPropertyName("formAssigneeProperty")]
        public int? FormAssigneeProperty { get; set; }
    }
}
