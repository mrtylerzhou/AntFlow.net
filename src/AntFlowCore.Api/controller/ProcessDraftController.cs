using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// Controller for process draft (草稿箱) operations.
/// Provides the loadDraft endpoint. Saving a draft goes through the standard
/// ButtonsOperation endpoint with operationType = 30.
/// </summary>
[Route("processDraft")]
public class ProcessDraftController
{
    private readonly IBpmProcessDraftBizService _bpmProcessDraftBizService;

    public ProcessDraftController(IBpmProcessDraftBizService bpmProcessDraftBizService)
    {
        _bpmProcessDraftBizService = bpmProcessDraftBizService;
    }

    /// <summary>
    /// Load the saved draft for the given formCode and the current login user.
    /// Returns null (empty data) if no draft exists or the draft is stale
    /// (template version changed). The frontend can show "无可用草稿" when the
    /// returned data is null.
    /// </summary>
    /// <param name="formCode">the form code (process key)</param>
    /// <returns>the draft as a BusinessDataVo, or null</returns>
    [HttpGet("loadDraft")]
    public Result<BusinessDataVo> LoadDraft([FromQuery] string formCode)
    {
        BusinessDataVo businessDataVo = _bpmProcessDraftBizService.LoadDraft(formCode, SecurityUtils.GetLogInEmpId());
        return ResultHelper.Success(businessDataVo);
    }
}
