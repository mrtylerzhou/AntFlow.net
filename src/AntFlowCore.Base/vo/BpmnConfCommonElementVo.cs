using System.Text.Json.Serialization;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.vo
{
    public class BpmnConfCommonElementVo
    {
        [JsonPropertyName("elementId")]
        public string ElementId { get; set; }

        [JsonPropertyName("nodeId")]
        public string NodeId { get; set; }
        [JsonPropertyName(("nodeType"))]
        public int NodeType { get; set; }

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
        public IDictionary<string, string> AssigneeMap { get; set; }

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

        /// <summary>
        /// Node labels carried on the final BPMN element. Converted to BPMN element
        /// extra attributes downstream. Populated from BpmnNodeVo.LabelList at runtime.
        /// </summary>
        [JsonPropertyName("labelList")]
        public List<BpmnNodeLabelVO> LabelList { get; set; }

        [JsonPropertyName("signType")]
        public int SignType { get; set; }

        /// <summary>
        /// Required completed instances for arbitration sign completion condition.
        /// N = ceil(n * ratio / 100), min 1, max n
        /// </summary>
        [JsonPropertyName("requiredCount")]
        public int? RequiredCount { get; set; }

        /// <summary>
        /// Arbitration sign pass ratio (1-100), only used when signType=4.
        /// Persisted into deployment content so it can be retrieved at runtime
        /// to compute the oppose threshold M = ceil(n * (100 - ratio) / 100).
        /// </summary>
        [JsonPropertyName("arbitrationRatio")]
        public int? ArbitrationRatio { get; set; }
        
        public bool? AggregationNode { get; set; }
    }
}
