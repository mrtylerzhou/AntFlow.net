using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using antflowcore.vo;

namespace AntFlowCore.Vo
{
    public class BpmnConfCommonElementVo
    {
        [JsonPropertyName("elementId")]
        public string ElementId { get; set; }

        [JsonPropertyName("nodeId")]
        public string NodeId { get; set; }

        [JsonPropertyName("elementName")]
        public string ElementName { get; set; }

        [JsonPropertyName("elementType")]
        public int ElementType { get; set; }

        [JsonPropertyName("elementProperty")]
        public int ElementProperty { get; set; }

        [JsonPropertyName("assigneeParamName")]
        public string AssigneeParamName { get; set; }

        [JsonPropertyName("assigneeParamValue")]
        public string AssigneeParamValue { get; set; }

        [JsonPropertyName("collectionName")]
        public string CollectionName { get; set; }

        [JsonPropertyName("collectionValue")]
        public List<string> CollectionValue { get; set; }

        [JsonPropertyName("assigneeMap")]
        public Dictionary<string, string> AssigneeMap { get; set; }

        [JsonPropertyName("flowFrom")]
        public string FlowFrom { get; set; }

        [JsonPropertyName("flowTo")]
        public string FlowTo { get; set; }

        [JsonPropertyName("sequenceFlowConditions")]
        public string SequenceFlowConditions { get; set; }

        [JsonPropertyName("isLastSequenceFlow")]
        public int IsLastSequenceFlow { get; set; } = 0;

        [JsonPropertyName("isSignUp")]
        public int IsSignUp { get; set; } = 0;

        [JsonPropertyName("afterSignUpWay")]
        public int? AfterSignUpWay { get; set; }

        [JsonPropertyName("signUpType")]
        public int? SignUpType { get; set; }

        [JsonPropertyName("isBackSignUp")]
        public int IsBackSignUp { get; set; } = 0;

        [JsonPropertyName("isSignUpSubElement")]
        public int IsSignUpSubElement { get; set; } = 0;

        [JsonPropertyName("signUpElementId")]
        public string SignUpElementId { get; set; }

        [JsonPropertyName("isSignUpSequenceFlow")]
        public int IsSignUpSequenceFlow { get; set; } = 0;

        [JsonPropertyName("buttons")]
        public BpmnConfCommonButtonsVo Buttons { get; set; }

        [JsonPropertyName("templateVos")]
        public List<BpmnTemplateVo> TemplateVos { get; set; }

        [JsonPropertyName("approveRemindVo")]
        public BpmnApproveRemindVo ApproveRemindVo { get; set; }
    }
}
