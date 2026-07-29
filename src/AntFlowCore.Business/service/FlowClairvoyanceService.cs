using System.Data;
using AntFlowCore.Base.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Business.service;

/// <summary>
/// 流程千里眼 - 核心搜索服务
/// </summary>
public class FlowClairvoyanceService
{
    private readonly IFreeSql _freeSql;
    private readonly ILogger<FlowClairvoyanceService> _logger;

    private const int BATCH_SIZE = 100;

    public FlowClairvoyanceService(IFreeSql freeSql, ILogger<FlowClairvoyanceService> logger)
    {
        _freeSql = freeSql;
        _logger = logger;
    }

    /// <summary>
    /// 分批搜索运行中流程的审批人
    /// </summary>
    public async Task<FlowClairvoyanceResultVo> SearchAsync(FlowClairvoyanceVo vo)
    {
        if (vo.UserIds == null || vo.UserIds.Count == 0)
        {
            return EmptyResult(0, false);
        }

        int timeRange = vo.TimeRange ?? 1;
        if (timeRange <= 0) timeRange = 1;

        string nodeScope = string.IsNullOrWhiteSpace(vo.NodeScope) ? "CURRENT_FUTURE" : vo.NodeScope;
        int offset = vo.Offset ?? 0;

        // 计算时间下界
        DateTime timeLowerBound = DateTime.Now.AddDays(-timeRange);

        // 1. 分页查询审批中的流程
        var processBatch = await SelectProcessBatchAsync(timeLowerBound, offset, BATCH_SIZE);

        if (processBatch == null || processBatch.Count == 0)
        {
            return EmptyResult(offset, false);
        }

        bool hasMore = processBatch.Count == BATCH_SIZE;
        int nextOffset = offset + BATCH_SIZE;

        // 构建 processNumber -> processInfo 映射
        var processMap = new Dictionary<string, ProcessBatchItem>();
        var processNumbers = new List<string>();
        var procInstIds = new List<string>();

        foreach (var proc in processBatch)
        {
            processNumbers.Add(proc.ProcessNumber);
            processMap[proc.ProcessNumber] = proc;
            if (!string.IsNullOrEmpty(proc.ProcInstId))
            {
                procInstIds.Add(proc.ProcInstId);
            }
        }

        // 2. 根据nodeScope查询匹配
        List<MatchRawItem> matches;
        if ("CURRENT".Equals(nodeScope, StringComparison.OrdinalIgnoreCase))
        {
            // 仅当前节点: 查bpm_af_task
            matches = procInstIds.Count == 0
                ? new List<MatchRawItem>()
                : await SelectCurrentNodeMatchesAsync(procInstIds, vo.UserIds);

            // 将procInstId映射回processNumber
            var instToPn = new Dictionary<string, string>();
            foreach (var proc in processBatch)
            {
                if (!string.IsNullOrEmpty(proc.ProcInstId))
                {
                    instToPn[proc.ProcInstId] = proc.ProcessNumber;
                }
            }

            foreach (var match in matches)
            {
                if (!string.IsNullOrEmpty(match.ProcInstId) && instToPn.ContainsKey(match.ProcInstId))
                {
                    match.ProcessNumber = instToPn[match.ProcInstId];
                }
            }
        }
        else
        {
            // CURRENT_FUTURE / FUTURE / ALL: 查multiplayer表
            matches = await SelectMultiplayerMatchesAsync(processNumbers, vo.UserIds, nodeScope);
        }

        // 3. 按processNumber -> elementId 分组聚合
        var results = BuildResults(matches, processMap);

        return new FlowClairvoyanceResultVo
        {
            Results = results,
            HasMore = hasMore,
            NextOffset = nextOffset,
            ScannedCount = processBatch.Count
        };
    }

    /// <summary>
    /// 分页查询审批中的流程
    /// </summary>
    private async Task<List<ProcessBatchItem>> SelectProcessBatchAsync(DateTime timeLowerBound, int offset, int batchSize)
    {
        string sql = @"
            SELECT bbp.BUSINESS_NUMBER AS ProcessNumber,
                   bbp.PROCESSINESS_KEY AS ProcessKey,
                   bbp.user_name AS UserName,
                   bbp.create_time AS CreateTime,
                   bbp.process_state AS ProcessState,
                   bbp.PROC_INST_ID_ AS ProcInstId
            FROM bpm_business_process bbp
            WHERE bbp.process_state = 1
              AND bbp.is_del = 0
              AND bbp.create_time >= @timeLowerBound
            ORDER BY bbp.create_time DESC
            LIMIT @batchSize OFFSET @offset";

        return await _freeSql.Ado.QueryAsync<ProcessBatchItem>(sql, new
        {
            timeLowerBound,
            batchSize,
            offset
        });
    }

    /// <summary>
    /// 仅当前节点: 直接查bpm_af_task
    /// </summary>
    private async Task<List<MatchRawItem>> SelectCurrentNodeMatchesAsync(List<string> procInstIds, List<string> userIds)
    {
        string inProcInst = string.Join(",", procInstIds.Select((_, i) => $"@pid{i}"));
        string inUsers = string.Join(",", userIds.Select((_, i) => $"@uid{i}"));

        string sql = $@"
            SELECT t.proc_inst_id AS ProcInstId,
                   t.task_def_key AS ElementId,
                   t.name AS ElementName,
                   t.assignee AS Assignee,
                   t.assignee_name AS AssigneeName
            FROM bpm_af_task t
            WHERE t.proc_inst_id IN ({inProcInst})
              AND t.assignee IN ({inUsers})";

        var parameters = new List<object>();
        for (int i = 0; i < procInstIds.Count; i++)
        {
            parameters.Add(new { Name = $"pid{i}", Value = procInstIds[i] });
        }
        for (int i = 0; i < userIds.Count; i++)
        {
            parameters.Add(new { Name = $"uid{i}", Value = userIds[i] });
        }

        // FreeSql Ado with anonymous object params
        var dynParams = BuildDynamicParams(procInstIds, userIds);
        return await _freeSql.Ado.QueryAsync<MatchRawItem>(sql, dynParams);
    }

    /// <summary>
    /// CURRENT_FUTURE / FUTURE / ALL: 关联multiplayer表
    /// </summary>
    private async Task<List<MatchRawItem>> SelectMultiplayerMatchesAsync(
        List<string> processNumbers, List<string> userIds, string nodeScope)
    {
        string inPn = string.Join(",", processNumbers.Select((_, i) => $"@pn{i}"));
        string inUsers = string.Join(",", userIds.Select((_, i) => $"@uid{i}"));

        string nodeScopeCondition = "";
        if ("CURRENT_FUTURE".Equals(nodeScope, StringComparison.OrdinalIgnoreCase))
        {
            nodeScopeCondition = @"
              AND CAST(REPLACE(bvm.element_id, 'task', '') AS UNSIGNED) >= (
                  SELECT MIN(CAST(REPLACE(t.task_def_key, 'task', '') AS UNSIGNED))
                  FROM bpm_af_task t
                  INNER JOIN bpm_business_process bbp2 ON bbp2.PROC_INST_ID_ = t.proc_inst_id
                  WHERE bbp2.BUSINESS_NUMBER = bv.process_num AND bbp2.process_state = 1
              )";
        }
        else if ("FUTURE".Equals(nodeScope, StringComparison.OrdinalIgnoreCase))
        {
            nodeScopeCondition = @"
              AND CAST(REPLACE(bvm.element_id, 'task', '') AS UNSIGNED) > (
                  SELECT MIN(CAST(REPLACE(t.task_def_key, 'task', '') AS UNSIGNED))
                  FROM bpm_af_task t
                  INNER JOIN bpm_business_process bbp2 ON bbp2.PROC_INST_ID_ = t.proc_inst_id
                  WHERE bbp2.BUSINESS_NUMBER = bv.process_num AND bbp2.process_state = 1
              )";
        }

        string sql = $@"
            SELECT bv.process_num AS ProcessNumber,
                   bvm.element_id AS ElementId,
                   bvm.element_name AS ElementName,
                   bvmp.assignee AS Assignee,
                   bvmp.assignee_name AS AssigneeName
            FROM t_bpm_variable bv
            INNER JOIN t_bpm_variable_multiplayer bvm
                ON bvm.variable_id = bv.id AND bvm.is_del = 0
            INNER JOIN t_bpm_variable_multiplayer_personnel bvmp
                ON bvmp.variable_multiplayer_id = bvm.id AND bvmp.is_del = 0
            WHERE bv.process_num IN ({inPn})
              AND bvmp.assignee IN ({inUsers})
              {nodeScopeCondition}";

        var dynParams = BuildMultiplayerDynamicParams(processNumbers, userIds);
        return await _freeSql.Ado.QueryAsync<MatchRawItem>(sql, dynParams);
    }

    /// <summary>
    /// 将扁平匹配结果聚合为嵌套结构
    /// </summary>
    private List<FlowClairvoyanceResultVo.ProcessMatchResult> BuildResults(
        List<MatchRawItem> matches, Dictionary<string, ProcessBatchItem> processMap)
    {
        if (matches == null || matches.Count == 0)
        {
            return new List<FlowClairvoyanceResultVo.ProcessMatchResult>();
        }

        // processNumber -> elementId -> List<MatchedPerson>
        var grouped = new Dictionary<string, Dictionary<string, List<FlowClairvoyanceResultVo.MatchedPerson>>>();
        var elementNames = new Dictionary<string, string>();

        foreach (var match in matches)
        {
            if (string.IsNullOrEmpty(match.ProcessNumber) || string.IsNullOrEmpty(match.ElementId))
                continue;

            if (!elementNames.ContainsKey(match.ElementId))
            {
                elementNames[match.ElementId] = match.ElementName ?? "";
            }

            if (!grouped.ContainsKey(match.ProcessNumber))
            {
                grouped[match.ProcessNumber] = new Dictionary<string, List<FlowClairvoyanceResultVo.MatchedPerson>>();
            }

            var nodeMap = grouped[match.ProcessNumber];
            if (!nodeMap.ContainsKey(match.ElementId))
            {
                nodeMap[match.ElementId] = new List<FlowClairvoyanceResultVo.MatchedPerson>();
            }

            nodeMap[match.ElementId].Add(new FlowClairvoyanceResultVo.MatchedPerson
            {
                Assignee = match.Assignee ?? "",
                AssigneeName = match.AssigneeName ?? ""
            });
        }

        // 组装结果
        var results = new List<FlowClairvoyanceResultVo.ProcessMatchResult>();
        foreach (var entry in grouped)
        {
            string pn = entry.Key;
            var nodeMap = entry.Value;

            if (!processMap.ContainsKey(pn)) continue;
            var procInfo = processMap[pn];

            var matchedNodes = new List<FlowClairvoyanceResultVo.MatchedNode>();
            int totalPersons = 0;

            foreach (var nodeEntry in nodeMap)
            {
                string elementId = nodeEntry.Key;
                // 去重
                var persons = nodeEntry.Value
                    .GroupBy(p => p.Assignee)
                    .Select(g => g.First())
                    .ToList();

                totalPersons += persons.Count;
                matchedNodes.Add(new FlowClairvoyanceResultVo.MatchedNode
                {
                    ElementId = elementId,
                    ElementName = elementNames.ContainsKey(elementId) ? elementNames[elementId] : "",
                    MatchedPersons = persons
                });
            }

            results.Add(new FlowClairvoyanceResultVo.ProcessMatchResult
            {
                ProcessNumber = pn,
                ProcessKey = procInfo.ProcessKey,
                UserName = procInfo.UserName,
                CreateTime = procInfo.CreateTime,
                ProcessState = procInfo.ProcessState,
                MatchedNodeCount = matchedNodes.Count,
                MatchedPersonCount = totalPersons,
                MatchedNodes = matchedNodes
            });
        }

        // 按创建时间倒序
        results = results.OrderByDescending(r => r.CreateTime ?? DateTime.MinValue).ToList();
        return results;
    }

    private static FlowClairvoyanceResultVo EmptyResult(int offset, bool hasMore)
    {
        return new FlowClairvoyanceResultVo
        {
            Results = new List<FlowClairvoyanceResultVo.ProcessMatchResult>(),
            HasMore = hasMore,
            NextOffset = offset + BATCH_SIZE,
            ScannedCount = 0
        };
    }

    /// <summary>
    /// 构建当前节点查询的动态参数
    /// </summary>
    private static object BuildDynamicParams(List<string> procInstIds, List<string> userIds)
    {
        var dict = new Dictionary<string, object>();
        for (int i = 0; i < procInstIds.Count; i++)
        {
            dict[$"pid{i}"] = procInstIds[i];
        }
        for (int i = 0; i < userIds.Count; i++)
        {
            dict[$"uid{i}"] = userIds[i];
        }
        return dict;
    }

    /// <summary>
    /// 构建multiplayer查询的动态参数
    /// </summary>
    private static object BuildMultiplayerDynamicParams(List<string> processNumbers, List<string> userIds)
    {
        var dict = new Dictionary<string, object>();
        for (int i = 0; i < processNumbers.Count; i++)
        {
            dict[$"pn{i}"] = processNumbers[i];
        }
        for (int i = 0; i < userIds.Count; i++)
        {
            dict[$"uid{i}"] = userIds[i];
        }
        return dict;
    }

    #region Internal DTOs

    /// <summary>
    /// 流程批次查询结果
    /// </summary>
    internal class ProcessBatchItem
    {
        public string ProcessNumber { get; set; }
        public string ProcessKey { get; set; }
        public string UserName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ProcessState { get; set; }
        public string ProcInstId { get; set; }
    }

    /// <summary>
    /// 匹配原始结果
    /// </summary>
    internal class MatchRawItem
    {
        public string ProcInstId { get; set; }
        public string ProcessNumber { get; set; }
        public string ElementId { get; set; }
        public string ElementName { get; set; }
        public string Assignee { get; set; }
        public string AssigneeName { get; set; }
    }

    #endregion
}
