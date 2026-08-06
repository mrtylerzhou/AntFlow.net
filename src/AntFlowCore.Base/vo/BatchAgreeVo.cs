using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 批量同意请求VO
/// </summary>
public class BatchAgreeVo
{
    /// <summary>
    /// 要审批的任务ID列表
    /// </summary>
    [JsonPropertyName("taskIds")]
    public List<string> TaskIds { get; set; }

    /// <summary>
    /// 审批意见（所有任务共用）
    /// </summary>
    [JsonPropertyName("batchApprovalComment")]
    public string BatchApprovalComment { get; set; }
}
