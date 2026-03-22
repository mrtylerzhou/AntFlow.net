using System.Text.Json;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

/// <summary>
/// 流程草稿业务服务
/// </summary>
public class BpmProcessDraftBizService
{
    private readonly BpmBusinessDraftService _draftService;
    private readonly BpmnConfService _bpmnConfService;

    public BpmProcessDraftBizService(
        BpmBusinessDraftService draftService,
        BpmnConfService bpmnConfService)
    {
        _draftService = draftService;
        _bpmnConfService = bpmnConfService;
    }

    /// <summary>
    /// 保存草稿
    /// 同一个流程只保留最新版本的一个草稿，历史草稿是没有意义的
    /// </summary>
    public void SaveBusinessDraft(BusinessDataVo businessDataVo)
    {
        string formCode = businessDataVo.FormCode;

        // 获取当前有效的流程配置
        List<BpmnConf> bpmnConfs = _bpmnConfService.baseRepo
            .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToList();

        if (bpmnConfs.IsEmpty())
        {
            throw new AFBizException($"未能根据流程formCode:{formCode}查找到有效的模板配置!");
        }

        BpmnConf bpmnConf = bpmnConfs[0];
        string bpmnCode = bpmnConf.BpmnCode;
        string userId = SecurityUtils.GetLogInEmpId();

        // 删除同一流程的历史草稿
        _draftService.baseRepo
            .Where(a => a.ProcessKey == formCode && a.CreateUser == userId)
            .ToDelete()
            .ExecuteAffrows();

        // 创建新草稿
        BpmBusinessDraft draft = new BpmBusinessDraft
        {
            BpmnCode = bpmnCode,
            ProcessKey = formCode,
            CreateUser = SecurityUtils.GetLogInEmpIdStr(),
            CreateUserName = SecurityUtils.GetLogInEmpNameSafe(),
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            DraftJson = JsonSerializer.Serialize(businessDataVo),
            CreateTime = DateTime.Now
        };

        _draftService.baseRepo.Insert(draft);
    }

    /// <summary>
    /// 加载草稿
    /// </summary>
    public BusinessDataVo LoadDraft(string formCode, string userId)
    {
        // 查询草稿
        List<BpmBusinessDraft> drafts = _draftService.baseRepo
            .Where(a => a.ProcessKey == formCode && a.CreateUser == userId)
            .ToList();

        if (drafts.IsEmpty())
        {
            return null;
        }

        BpmBusinessDraft draft = drafts[0];
        string draftJson = draft.DraftJson;
        BusinessDataVo businessDataVo = JsonSerializer.Deserialize<BusinessDataVo>(draftJson);
        string oldBpmnCode = businessDataVo.BpmnConfVo?.BpmnCode;

        // 获取当前有效的流程配置
        List<BpmnConf> bpmnConfs = _bpmnConfService.baseRepo
            .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToList();

        if (bpmnConfs.IsEmpty())
        {
            throw new AFBizException($"未能根据流程formCode:{formCode}查找到有效的模板配置!");
        }

        BpmnConf bpmnConf = bpmnConfs[0];
        string bpmnCode = bpmnConf.BpmnCode;

        // 流程引擎无法感知版本变化时表单是否也发生变化，默认如果版本变化则草稿失效
        if (oldBpmnCode != bpmnCode)
        {
            _draftService.baseRepo
                .Where(a => a.ProcessKey == formCode && a.CreateUser == userId)
                .ToDelete()
                .ExecuteAffrows();
            return null;
        }

        return businessDataVo;
    }
}
