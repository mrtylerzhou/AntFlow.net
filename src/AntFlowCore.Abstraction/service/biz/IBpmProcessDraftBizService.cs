using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

/// <summary>
/// Business service for process drafts (草稿箱).
/// Handles saving a form as a draft and loading an existing draft.
/// </summary>
public interface IBpmProcessDraftBizService
{
    /// <summary>
    /// Save the given business data vo as a draft for the current user and formCode.
    /// The latest draft replaces any previous draft for the same (processKey, createUser).
    /// </summary>
    void SaveBusinessDraft(BusinessDataVo businessDataVo);

    /// <summary>
    /// Load the draft for the given formCode and userId.
    /// Returns null if no draft exists, or if the draft's bpmnCode no longer matches
    /// the current effective template (i.e. the template version has changed — the draft
    /// is considered stale and is deleted).
    /// </summary>
    BusinessDataVo LoadDraft(string formCode, string userId);
}
