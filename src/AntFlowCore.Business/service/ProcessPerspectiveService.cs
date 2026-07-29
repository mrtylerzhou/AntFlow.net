using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Business.service;

/// <summary>
/// 流程透视搜索服务
/// </summary>
public class ProcessPerspectiveService
{
    private readonly IBpmnConfRepository _bpmnConfRepository;
    private readonly IBpmnNodeRepository _bpmnNodeRepository;
    private readonly ILogger<ProcessPerspectiveService> _logger;

    private const int DEFAULT_BATCH_SIZE = 5;
    private const int USE_EXTERNAL_FORM_FLAG = 64;
    private const int USE_AUXILIARY_FORM_FLAG = 128;

    public ProcessPerspectiveService(
        IBpmnConfRepository bpmnConfRepository,
        IBpmnNodeRepository bpmnNodeRepository,
        ILogger<ProcessPerspectiveService> logger)
    {
        _bpmnConfRepository = bpmnConfRepository;
        _bpmnNodeRepository = bpmnNodeRepository;
        _logger = logger;
    }

    /// <summary>
    /// 分批搜索流程配置
    /// </summary>
    public ProcessPerspectiveResultVo Search(ProcessPerspectiveVo vo)
    {
        var formCodes = vo.FormCodes;
        if (formCodes == null || formCodes.Count == 0)
        {
            return new ProcessPerspectiveResultVo
            {
                Results = new List<ProcessPerspectiveResultVo.FormCodeResult>(),
                HasMore = false,
                ProcessedCount = 0,
                TotalCount = 0
            };
        }

        int batchSize = vo.BatchSize ?? DEFAULT_BATCH_SIZE;
        int offset = vo.Offset ?? 0;
        int totalCount = formCodes.Count;

        int end = Math.Min(offset + batchSize, totalCount);
        var batchFormCodes = formCodes.GetRange(offset, end - offset);
        bool hasMore = end < totalCount;

        var filters = vo.FilterConfig;
        string versionMode = vo.VersionMode ?? "RECENT";
        int recentN = vo.RecentN ?? 1;

        var results = new List<ProcessPerspectiveResultVo.FormCodeResult>();

        foreach (string formCode in batchFormCodes)
        {
            // 1. SQL粗筛
            var candidates = GetCandidates(formCode, versionMode, recentN, filters);
            if (candidates.Count == 0) continue;

            // 2. 内存精筛
            var matches = new List<ProcessPerspectiveResultVo.VersionMatch>();
            foreach (var conf in candidates)
            {
                if (MatchesNodeLevelFilters(conf, filters))
                {
                    matches.Add(new ProcessPerspectiveResultVo.VersionMatch
                    {
                        ConfId = conf.Id,
                        BpmnCode = conf.BpmnCode,
                        BpmnName = conf.BpmnName,
                        EffectiveStatus = conf.EffectiveStatus,
                        CreateTime = conf.CreateTime
                    });
                }
            }

            if (matches.Count > 0)
            {
                var first = candidates[0];
                results.Add(new ProcessPerspectiveResultVo.FormCodeResult
                {
                    FormCode = formCode,
                    DisplayName = first.BpmnName,
                    FlowType = DetermineFlowType(first),
                    LatestMatch = matches[0],
                    AllMatches = matches
                });
            }
        }

        return new ProcessPerspectiveResultVo
        {
            Results = results,
            HasMore = hasMore,
            ProcessedCount = batchFormCodes.Count,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// SQL层粗筛
    /// </summary>
    private List<BpmnConf> GetCandidates(string formCode, string versionMode, int recentN,
        ProcessPerspectiveVo.Filters filters)
    {
        var query = _bpmnConfRepository.GetQueryable()
            .Where(c => c.FormCode == formCode && c.IsDel == 0);

        if ("EFFECTIVE".Equals(versionMode, StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(c => c.EffectiveStatus == 1);
        }

        if (filters != null)
        {
            if (!string.IsNullOrWhiteSpace(filters.BpmnNameLike))
            {
                string keyword = filters.BpmnNameLike;
                query = query.Where(c => c.BpmnName.Contains(keyword));
            }

            if (filters.UseExternalForm == true)
            {
                query = query.Where(c => c.ExtraFlags != null && (c.ExtraFlags & USE_EXTERNAL_FORM_FLAG) == USE_EXTERNAL_FORM_FLAG);
            }

            if (filters.Deduplication == true)
            {
                query = query.Where(c => c.DeduplicationType > 1);
            }
        }

        var list = query.OrderByDescending(c => c.CreateTime).ToList();
        if (list.Count == 0) return list;

        if ("RECENT".Equals(versionMode, StringComparison.OrdinalIgnoreCase) && list.Count > recentN)
        {
            return list.GetRange(0, recentN);
        }

        return list;
    }

    /// <summary>
    /// 节点级条件内存匹配
    /// </summary>
    private bool MatchesNodeLevelFilters(BpmnConf conf, ProcessPerspectiveVo.Filters filters)
    {
        if (filters == null) return true;

        // conf级: 允许撤回/作废/转发
        if (!MatchesViewPageButtons(conf, filters)) return false;

        // conf级: 通知
        var confConfig = JsonConfUtil.ParseConfConfig(conf.ConfConfigJson);

        bool needNodes = NeedLoadNodes(filters);
        if (!needNodes)
        {
            if (filters.HasNotice == true)
            {
                return HasConfLevelNotice(confConfig);
            }
            return true;
        }

        // 加载节点
        List<BpmnNode> nodes;
        try
        {
            nodes = _bpmnNodeRepository.Find(n => n.ConfId == conf.Id && n.IsDel == 0);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "加载节点失败, confId={ConfId}", conf.Id);
            return false;
        }

        if (nodes == null || nodes.Count == 0)
        {
            if (filters.HasNotice == true)
            {
                return HasConfLevelNotice(confConfig);
            }
            return false;
        }

        // 表单字段匹配
        if (!string.IsNullOrWhiteSpace(filters.FormFieldKeyword))
        {
            if (!MatchesFormField(conf, confConfig, filters.FormFieldKeyword)) return false;
        }

        // 字段权限
        if (filters.HasEditableFieldPerm == true)
        {
            if (!MatchesEditableFieldPerm(nodes)) return false;
        }

        // 审批人规则
        if (filters.ApproverRules is { Count: > 0 })
        {
            if (!MatchesApproverRules(nodes, filters.ApproverRules)) return false;
        }

        // 额外增加/排除审批
        if (filters.HasAdditionalSign == true || filters.HasExcludeSign == true)
        {
            if (!MatchesAdditionalSign(nodes, filters)) return false;
        }

        // 审批人为空规则
        if (filters.NoHeaderActions is { Count: > 0 })
        {
            if (!MatchesNoHeaderAction(nodes, filters.NoHeaderActions)) return false;
        }

        // 按钮权限
        if (filters.ButtonTypes is { Count: > 0 })
        {
            if (!MatchesButtonTypes(nodes, filters.ButtonTypes)) return false;
        }

        // 通知(conf级 + node级)
        if (filters.HasNotice == true)
        {
            if (!HasConfLevelNotice(confConfig) && !HasNodeLevelNotice(nodes)) return false;
        }

        // 节点类型
        if (filters.NodeTypes is { Count: > 0 })
        {
            if (!MatchesNodeTypes(nodes, filters.NodeTypes)) return false;
        }

        return true;
    }

    private static bool NeedLoadNodes(ProcessPerspectiveVo.Filters filters)
    {
        return filters.HasEditableFieldPerm == true
               || filters.ApproverRules is { Count: > 0 }
               || filters.HasAdditionalSign == true
               || filters.HasExcludeSign == true
               || filters.NoHeaderActions is { Count: > 0 }
               || filters.ButtonTypes is { Count: > 0 }
               || filters.NodeTypes is { Count: > 0 }
               || filters.HasNotice == true
               || !string.IsNullOrWhiteSpace(filters.FormFieldKeyword);
    }

    private static bool MatchesViewPageButtons(BpmnConf conf, ProcessPerspectiveVo.Filters filters)
    {
        bool needCheck = filters.AllowRevoke == true || filters.AllowCancel == true || filters.AllowForward == true;
        if (!needCheck) return true;

        var confConfig = JsonConfUtil.ParseConfConfig(conf.ConfConfigJson);
        if (confConfig?.ViewPageButtons == null || confConfig.ViewPageButtons.Count == 0) return false;

        var startBtnTypes = confConfig.ViewPageButtons
            .Where(b => b.ViewType == 1)
            .Select(b => b.ButtonType)
            .ToHashSet();

        if (filters.AllowRevoke == true && !startBtnTypes.Contains(29)) return false;
        if (filters.AllowCancel == true && !startBtnTypes.Contains(7)) return false;
        if (filters.AllowForward == true && !startBtnTypes.Contains(15)) return false;
        return true;
    }

    private static bool HasConfLevelNotice(BpmnConfConfigJson confConfig)
    {
        if (confConfig == null) return false;
        return confConfig.NoticeChannelTypes is { Count: > 0 }
               || confConfig.ConfTemplates is { Count: > 0 };
    }

    private static bool HasNodeLevelNotice(List<BpmnNode> nodes)
    {
        foreach (var node in nodes)
        {
            var nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
            if (nodeConfig?.TemplateConf != null) return true;
        }
        return false;
    }

    private static bool MatchesFormField(BpmnConf conf, BpmnConfConfigJson confConfig, string keyword)
    {
        bool hasFormCapability = (conf.IsLowCodeFlow != null && conf.IsLowCodeFlow == 1)
                                 || (conf.ExtraFlags != null && (conf.ExtraFlags & USE_AUXILIARY_FORM_FLAG) == USE_AUXILIARY_FORM_FLAG);
        if (!hasFormCapability) return false;

        if (confConfig?.LowCodeFormConfig?.Fields == null || confConfig.LowCodeFormConfig.Fields.Count == 0)
            return false;

        string lowerKeyword = keyword.ToLower();
        return confConfig.LowCodeFormConfig.Fields.Any(f =>
            (f.FieldName != null && f.FieldName.ToLower().Contains(lowerKeyword))
            || (f.FieldId != null && f.FieldId.ToLower().Contains(lowerKeyword)));
    }

    private static bool MatchesEditableFieldPerm(List<BpmnNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.NodeType == 1) continue; // 跳过发起人节点
            var nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
            var controls = nodeConfig?.LowCodeConf?.FieldControls;
            if (controls == null || controls.Count == 0) continue;
            foreach (var ctrl in controls)
            {
                if ("E".Equals(ctrl.Perm, StringComparison.OrdinalIgnoreCase)
                    || "W".Equals(ctrl.Perm, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool MatchesApproverRules(List<BpmnNode> nodes, List<int> rules)
    {
        var ruleSet = rules.ToHashSet();
        return nodes.Any(n => ruleSet.Contains(n.NodeProperty));
    }

    private static bool MatchesAdditionalSign(List<BpmnNode> nodes, ProcessPerspectiveVo.Filters filters)
    {
        foreach (var node in nodes)
        {
            var nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
            var signList = nodeConfig?.ButtonSignConf?.AdditionalSignConfList;
            if (signList == null || signList.Count == 0) continue;

            foreach (var sign in signList)
            {
                if (filters.HasAdditionalSign == true && sign.SignPropertyType == 1) return true;
                if (filters.HasExcludeSign == true && sign.SignPropertyType == 2) return true;
            }
        }
        return false;
    }

    private static bool MatchesNoHeaderAction(List<BpmnNode> nodes, List<int> actions)
    {
        var actionSet = actions.ToHashSet();
        return nodes.Any(n => n.NoHeaderAction != null && actionSet.Contains(n.NoHeaderAction.Value));
    }

    private static bool MatchesButtonTypes(List<BpmnNode> nodes, List<int> buttonTypes)
    {
        var btnSet = buttonTypes.ToHashSet();
        foreach (var node in nodes)
        {
            var nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
            var btnList = nodeConfig?.ButtonSignConf?.ButtonConfList;
            if (btnList == null || btnList.Count == 0) continue;
            foreach (var btn in btnList)
            {
                if (btn.ButtonType != null && btnSet.Contains(btn.ButtonType.Value)) return true;
            }
        }
        return false;
    }

    private static bool MatchesNodeTypes(List<BpmnNode> nodes, List<int> nodeTypes)
    {
        var typeSet = nodeTypes.ToHashSet();
        return nodes.Any(n => typeSet.Contains(n.NodeType));
    }

    private static string DetermineFlowType(BpmnConf conf)
    {
        if (conf.IsOutSideProcess != null && conf.IsOutSideProcess == 1) return "OUTSIDE";
        if (conf.IsLowCodeFlow != null && conf.IsLowCodeFlow == 1) return "LF";
        return "DIY";
    }
}
