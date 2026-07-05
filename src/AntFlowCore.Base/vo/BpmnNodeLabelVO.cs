using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// Node label VO. A label is an extra attribute attached to a BPMN node/element.
    /// Labels can be user-defined (saved at design time via /bpmnconf/edit) or
    /// dynamically added at runtime. They are ultimately carried on
    /// <see cref="BpmnConfCommonElementVo.LabelList"/> and converted to BPMN element
    /// extra attributes.
    /// </summary>
    public class BpmnNodeLabelVO
    {
        public BpmnNodeLabelVO() { }

        public BpmnNodeLabelVO(string labelValue, string labelName)
        {
            LabelValue = labelValue;
            LabelName = labelName;
        }

        [JsonPropertyName("labelName")]
        public string LabelName { get; set; }

        [JsonPropertyName("labelValue")]
        public string LabelValue { get; set; }
    }
}
