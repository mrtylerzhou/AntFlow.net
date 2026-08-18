using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// 流程对比(实例级): 流程监控-更多-流程对比, 与 Java 版 ProcessCompareBizServiceImpl 对等.
/// 仅提供两个薄查询; 节点对齐(alignTrees)与审批人 diff 全部由前端完成,
/// 与版本比较"前端 diff 引擎"架构保持一致。
/// 设计: .scratch/process-instance-compare-design.md §4
/// </summary>
public class ProcessCompareBizService : IProcessCompareBizService
{
    /// <summary>候选实例单次最大返回条数</summary>
    private const int CandidateLimit = 50;

    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProvider;
    private readonly ILogger<ProcessCompareBizService> _logger;

    public ProcessCompareBizService(
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmnConfService bpmnConfService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IBpmnEmployeeInfoProviderService employeeInfoProvider,
        ILogger<ProcessCompareBizService> logger)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmnConfService = bpmnConfService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _employeeInfoProvider = employeeInfoProvider;
        _logger = logger;
    }

    // ==================================================================================
    // CompareCandidates
    // ==================================================================================

    public List<ProcessCompareCandidateVo> CompareCandidates(string formCode, string keyword)
    {
        if (string.IsNullOrEmpty(formCode))
        {
            throw new AFBizException("formCode 不能为空");
        }

        string kw = keyword?.Trim();
        List<BpmBusinessProcess> processes = _bpmBusinessProcessService._repository
            .GetQueryable()
            .Where(a => a.ProcessinessKey == formCode && a.IsDel == 0)
            .Where(a => string.IsNullOrEmpty(kw) || a.BusinessNumber.Contains(kw) || a.UserName.Contains(kw))
            .OrderByDescending(a => a.CreateTime)
            .Take(CandidateLimit)
            .ToList();
        if (processes == null || processes.Count == 0)
        {
            return new List<ProcessCompareCandidateVo>();
        }

        // 批量反查 t_bpmn_conf(bpmn_code → id/bpmn_name)
        List<string> versions = processes
            .Select(a => a.Version)
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct()
            .ToList();
        Dictionary<string, BpmnConf> confByVersion = new Dictionary<string, BpmnConf>();
        if (versions.Count > 0)
        {
            List<BpmnConf> confs = _bpmnConfService._repository
                .GetQueryable()
                .Where(a => versions.Contains(a.BpmnCode))
                .ToList();
            foreach (BpmnConf conf in confs)
            {
                confByVersion.TryAdd(conf.BpmnCode, conf);
            }
        }

        // 批量补全发起人姓名(user_name 为空时)
        List<string> missingUserIds = processes
            .Where(p => string.IsNullOrEmpty(p.UserName) && !string.IsNullOrEmpty(p.CreateUser))
            .Select(p => p.CreateUser)
            .Distinct()
            .ToList();
        Dictionary<string, string> nameMap = new Dictionary<string, string>();
        if (missingUserIds.Count > 0)
        {
            try
            {
                nameMap = _employeeInfoProvider.ProvideEmployeeInfo(missingUserIds)
                          ?? new Dictionary<string, string>();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "compareCandidates: batch resolve user names failed");
            }
        }

        List<ProcessCompareCandidateVo> result = new List<ProcessCompareCandidateVo>(processes.Count);
        foreach (BpmBusinessProcess p in processes)
        {
            confByVersion.TryGetValue(p.Version ?? string.Empty, out BpmnConf conf);
            string userName = !string.IsNullOrEmpty(p.UserName)
                ? p.UserName
                : nameMap.GetValueOrDefault(p.CreateUser);
            result.Add(new ProcessCompareCandidateVo
            {
                ProcessNumber = p.BusinessNumber,
                Version = p.Version,
                CreateUser = p.CreateUser,
                UserName = userName,
                CreateTime = p.CreateTime,
                ProcessState = p.ProcessState,
                ConfId = conf?.Id,
                BpmnName = conf?.BpmnName,
            });
        }
        return result;
    }

    // ==================================================================================
    // CompareEntrusts
    // ==================================================================================

    public List<ProcessCompareEntrustVo> CompareEntrusts(string processNumber)
    {
        if (string.IsNullOrEmpty(processNumber))
        {
            throw new AFBizException("processNumber 不能为空");
        }
        BpmBusinessProcess process = _bpmBusinessProcessService._repository
            .FirstOrDefault(a => a.BusinessNumber == processNumber);
        if (process == null)
        {
            throw new AFBizException($"流程实例不存在: {processNumber}");
        }
        if (string.IsNullOrEmpty(process.ProcInstId))
        {
            return new List<ProcessCompareEntrustVo>();
        }

        string procInstId = process.ProcInstId;
        List<BpmFlowrunEntrust> records = _bpmFlowrunEntrustService._repository
            .GetQueryable()
            .Where(a => a.RunInfoId == procInstId)
            .OrderByDescending(a => a.Id)
            .ToList();
        if (records == null || records.Count == 0)
        {
            return new List<ProcessCompareEntrustVo>();
        }
        return records.Select(r => new ProcessCompareEntrustVo
        {
            NodeId = r.NodeId,
            ActionType = r.ActionType,
            ActionTypeName = GetActionTypeName(r.ActionType),
            OriginalId = r.Original,
            OriginalName = r.OriginalName,
            ActualId = r.Actual,
            ActualName = r.ActualName,
        }).ToList();
    }

    private static string GetActionTypeName(int? actionType)
    {
        if (actionType == null)
        {
            return "未知";
        }
        switch (actionType.Value)
        {
            case 0:
            case 1: return "转办";
            case 2: return "加签";
            case 3: return "减签";
            case 4: return "表单关联刷新";
            default: return $"未知({actionType.Value})";
        }
    }
}
