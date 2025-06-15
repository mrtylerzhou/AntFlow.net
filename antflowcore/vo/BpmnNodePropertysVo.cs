using System;
using System.Collections.Generic;
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
        // 默认构造函数
        public BpmnNodePropertysVo() { }
        
    }
}
