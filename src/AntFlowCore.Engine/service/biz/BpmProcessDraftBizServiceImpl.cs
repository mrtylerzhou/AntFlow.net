using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// Implementation of <see cref="IBpmProcessDraftBizService"/>.
/// Saves/loads process form drafts. Only the latest draft per (processKey, createUser)
/// is kept. If the template version (bpmnCode) changes, a previously stored draft is
/// considered stale and is deleted.
/// </summary>
public class BpmProcessDraftBizServiceImpl : IBpmProcessDraftBizService
{
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmBusinessDraftService _bpmBusinessDraftService;
    private readonly IFormFactory _formFactory;
    private readonly ILogger<BpmProcessDraftBizServiceImpl> _logger;

    public BpmProcessDraftBizServiceImpl(
        IBpmnConfService bpmnConfService,
        IBpmBusinessDraftService bpmBusinessDraftService,
        IFormFactory formFactory,
        ILogger<BpmProcessDraftBizServiceImpl> logger)
    {
        _bpmnConfService = bpmnConfService;
        _bpmBusinessDraftService = bpmBusinessDraftService;
        _formFactory = formFactory;
        _logger = logger;
    }

    public void SaveBusinessDraft(BusinessDataVo businessDataVo)
    {
        string formCode = businessDataVo.FormCode;
        List<BpmnConf> bpmnConfs = _bpmnConfService._repository
            .Find(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToList();

        if (bpmnConfs == null || bpmnConfs.Count == 0)
        {
            string errMsg = $"未能根据流程formCode:{formCode}查找到有效的模板配置!";
            _logger.LogError(errMsg);
            throw new AFBizException(errMsg);
        }

        BpmnConf bpmnConf = bpmnConfs[0];
        string bpmnCode = bpmnConf.BpmnCode;

        // 同一个流程只保留最新版本的一个草稿,历史草稿是没有意义的
        List<BpmBusinessDraft> existingDrafts = _bpmBusinessDraftService._repository
            .Find(a => a.ProcessKey == formCode && a.CreateUser == SecurityUtils.GetLogInEmpIdSafe())
            .ToList();

        if (existingDrafts != null && existingDrafts.Count > 0)
        {
            _bpmBusinessDraftService._repository.RemoveRange(existingDrafts);
        }

        var draft = new BpmBusinessDraft
        {
            BpmnCode = bpmnCode,
            ProcessKey = formCode,
            CreateUser = SecurityUtils.GetLogInEmpIdStr(),
            CreateUserName = SecurityUtils.GetLogInEmpNameSafe(),
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            CreateTime = DateTime.Now,
            DraftJson = JsonSerializer.Serialize(businessDataVo)
        };

        _bpmBusinessDraftService._repository.Add(draft);
    }

    public BusinessDataVo LoadDraft(string formCode, string userId)
    {
        List<BpmBusinessDraft> bpmBusinessDrafts = _bpmBusinessDraftService._repository
            .Find(a => a.ProcessKey == formCode && a.CreateUser == userId)
            .ToList();

        if (bpmBusinessDrafts == null || bpmBusinessDrafts.Count == 0)
        {
            return null;
        }

        BpmBusinessDraft draft = bpmBusinessDrafts[0];
        string draftJson = draft.DraftJson;

        BusinessDataVo businessDataVo = _formFactory.DataFormConversion(draftJson, formCode);
        string oldBpmnCode = businessDataVo.BpmnConfVo?.BpmnCode;

        List<BpmnConf> bpmnConfs = _bpmnConfService._repository
            .Find(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToList();

        if (bpmnConfs == null || bpmnConfs.Count == 0)
        {
            string errMsg = $"未能根据流程formCode:{formCode}查找到有效的模板配置!";
            _logger.LogError(errMsg);
            throw new AFBizException(errMsg);
        }

        BpmnConf bpmnConf = bpmnConfs[0];
        string bpmnCode = bpmnConf.BpmnCode;

        // 流程引擎无法感知版本变化时表单是否也发生变化,默认如果版本变化则草稿失效
        if (!string.Equals(oldBpmnCode, bpmnCode))
        {
            List<BpmBusinessDraft> staleDrafts = _bpmBusinessDraftService._repository
                .Find(a => a.ProcessKey == formCode && a.CreateUser == userId)
                .ToList();
            if (staleDrafts != null && staleDrafts.Count > 0)
            {
                _bpmBusinessDraftService._repository.RemoveRange(staleDrafts);
            }
            return null;
        }

        return businessDataVo;
    }
}
