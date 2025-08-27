using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmnNodeVo
{
    private string _nodeFroms;

    [JsonPropertyName("prevId")] private List<string> _prevId = new();

    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("confId")] public long ConfId { get; set; }

    [JsonPropertyName("nodeId")] public string NodeId { get; set; }

    [JsonPropertyName("nodeType")] public int NodeType { get; set; }

    [JsonPropertyName("isParallel")] public bool? IsParallel { get; set; }

    [JsonPropertyName("isDynamicCondition")]
    public bool? IsDynamicCondition { get; set; }

    [JsonPropertyName("aggregationNode")] public bool? AggregationNode { get; set; }

    [JsonPropertyName("nodeProperty")] public int? NodeProperty { get; set; }

    [JsonPropertyName("nodePropertyName")] public string NodePropertyName { get; set; }

    [JsonPropertyName("nodeFrom")] public string NodeFrom { get; set; }

    [JsonPropertyName("nodeFroms")]
    public string NodeFroms
    {
        get => _nodeFroms;
        set
        {
            _nodeFroms = value;
            if (!string.IsNullOrEmpty(value))
            {
                // 设置 NodeFroms 时自动更新 PrevId
                PrevId = new List<string>(value.Split(','));
            }
        }
    }

    public List<string> PrevId
    {
        get => _prevId;
        set
        {
            _prevId = value;
            if (_prevId != null && _prevId.Count > 0)
            {
                // 设置 PrevId 时自动更新 NodeFroms
                NodeFroms = string.Join(",", _prevId);
            }
        }
    }

    [JsonPropertyName("batchStatus")] public int BatchStatus { get; set; }

    [JsonPropertyName("approvalStandard")] public int ApprovalStandard { get; set; }

    [JsonPropertyName("nodeName")] public string NodeName { get; set; }

    [JsonPropertyName("nodeDisplayName")] public string NodeDisplayName { get; set; }

    [JsonPropertyName("annotation")] public string Annotation { get; set; }

    [JsonPropertyName("isDeduplication")] public int IsDeduplication { get; set; }

    [JsonPropertyName("deduplicationExclude")]
    public bool DeduplicationExclude { get; set; }

    [JsonPropertyName("isSignUp")] public int IsSignUp { get; set; }

    [JsonPropertyName("orderedNodeType")] public int? OrderedNodeType { get; set; }

    [JsonPropertyName("remark")] public string Remark { get; set; }

    [JsonPropertyName("isDel")] public int IsDel { get; set; }

    [JsonPropertyName("createUser")] public string CreateUser { get; set; }

    [JsonPropertyName("createTime")] public DateTime? CreateTime { get; set; }

    [JsonPropertyName("updateUser")] public string UpdateUser { get; set; }

    [JsonPropertyName("updateTime")] public DateTime? UpdateTime { get; set; }

    //===============>>ext fields<<===================

    [JsonPropertyName("nodeTo")] public List<string> NodeTo { get; set; }

    [JsonPropertyName("property")] public BpmnNodePropertysVo Property { get; set; }

    [JsonPropertyName("params")] public BpmnNodeParamsVo Params { get; set; }

    [JsonPropertyName("buttons")] public BpmnNodeButtonConfBaseVo Buttons { get; set; }

    [JsonPropertyName("templateVos")] public List<BpmnTemplateVo> TemplateVos { get; set; }

    [JsonPropertyName("approveRemindVo")] public BpmnApproveRemindVo ApproveRemindVo { get; set; }

    //===============>>third party processs service<<===================

    [JsonPropertyName("conditionsUrl")] public string ConditionsUrl { get; set; }

    [JsonPropertyName("formCode")] public string FormCode { get; set; }

    [JsonPropertyName("isOutSideProcess")] public int? IsOutSideProcess { get; set; }

    [JsonPropertyName("isLowCodeFlow")] public int? IsLowCodeFlow { get; set; }

    [JsonPropertyName("lfFieldControlVOs")]
    public List<LFFieldControlVO> LfFieldControlVOs { get; set; }

    [JsonPropertyName("fromNodes")] public List<BpmnNodeVo> FromNodes { get; set; }

    [JsonPropertyName("elementId")] public string ElementId { get; set; }
}