using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Business.service;

/// <summary>
/// 流程实例效能 Service(实时计算,不入库)
///
/// 口径参见 .scratch/process-instance-efficiency-design.md:
/// - 个人耗时:已完成取 af_hi_taskinst.duration;未完成取 now - start_time
/// - 节点耗时:每轮 max(end_time) - min(start_time),退回多轮累加
/// - 退回轮次:按 execution_id 分组
/// - 节点详情:只返回最后一轮(execution_id 最新)人员
/// - TOP3:进行中节点不参与
/// </summary>
public class ProcessInstanceEfficiencyService
{
    private readonly IBpmAfTaskInstRepository _taskInstRepository;
    private readonly IBpmBusinessProcessRepository _businessProcessRepository;
    private readonly IBpmVariableRepository _variableRepository;
    private readonly IBpmnNodeRepository _nodeRepository;
    private readonly IUserService _userService;
    private readonly ILogger<ProcessInstanceEfficiencyService> _logger;

    public ProcessInstanceEfficiencyService(
        IBpmAfTaskInstRepository taskInstRepository,
        IBpmBusinessProcessRepository businessProcessRepository,
        IBpmVariableRepository variableRepository,
        IBpmnNodeRepository nodeRepository,
        IUserService userService,
        ILogger<ProcessInstanceEfficiencyService> logger)
    {
        _taskInstRepository = taskInstRepository;
        _businessProcessRepository = businessProcessRepository;
        _variableRepository = variableRepository;
        _nodeRepository = nodeRepository;
        _userService = userService;
        _logger = logger;
    }

    // ==================== 1. 顶部汇总 ====================

    public InstanceEfficiencySummaryVo GetSummary(string processNumber)
    {
        BpmBusinessProcess process = _businessProcessRepository
            .Find(a => a.BusinessNumber == processNumber)
            .FirstOrDefault();

        if (process == null)
        {
            return null;
        }

        int processState = process.ProcessState;
        bool finished = processState != (int)ProcessStateEnum.HANDLING_STATE;
        DateTime? createTime = process.CreateTime;
        DateTime now = DateTime.Now;

        long? totalDuration;
        if (createTime == null)
        {
            totalDuration = 0L;
        }
        else if (finished)
        {
            // 已完成:取所有 task 中最晚的 end_time
            DateTime? maxEnd = GetMaxEndTime(process.ProcInstId);
            totalDuration = maxEnd != null
                ? (long)(maxEnd.Value - createTime.Value).TotalMilliseconds
                : (long)(now - createTime.Value).TotalMilliseconds;
        }
        else
        {
            // 进行中:now - createTime
            totalDuration = (long)(now - createTime.Value).TotalMilliseconds;
        }

        return new InstanceEfficiencySummaryVo
        {
            ProcessNumber = processNumber,
            ProcessState = processState,
            ProcessStateName = ProcessStateEnumExtensions.GetDescByCode(processState),
            CreateTime = createTime,
            TotalDuration = totalDuration,
            TotalDurationText = FormatDuration(totalDuration ?? 0),
            Finished = finished
        };
    }

    // ==================== 2. 节点列表 ====================

    public List<InstanceEfficiencyNodeVo> ListNodes(string processNumber)
    {
        BpmBusinessProcess process = _businessProcessRepository
            .Find(a => a.BusinessNumber == processNumber)
            .FirstOrDefault();

        if (process == null || string.IsNullOrEmpty(process.ProcInstId))
        {
            return new List<InstanceEfficiencyNodeVo>();
        }

        List<BpmAfTaskInst> tasks = _taskInstRepository
            .Find(a => a.ProcInstId == process.ProcInstId)
            .ToList();

        if (tasks.Count == 0)
        {
            return new List<InstanceEfficiencyNodeVo>();
        }

        int processState = process.ProcessState;
        bool processFinished = processState != (int)ProcessStateEnum.HANDLING_STATE;
        DateTime now = DateTime.Now;

        // 按 taskDefKey 分组(保持插入顺序)
        var nodeGroup = tasks
            .Where(t => !string.IsNullOrEmpty(t.TaskDefKey))
            .GroupBy(t => t.TaskDefKey)
            .ToDictionary(g => g.Key, g => g.ToList());

        List<InstanceEfficiencyNodeVo> nodes = new List<InstanceEfficiencyNodeVo>();
        foreach (var entry in nodeGroup)
        {
            string taskDefKey = entry.Key;
            List<BpmAfTaskInst> nodeTasks = entry.Value;

            // 按 execution_id 分轮次
            var rounds = nodeTasks
                .GroupBy(t => t.ExecutionId ?? "null")
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            bool hasRollback = rounds.Count > 1;

            // 每轮算 max(end)-min(start),累加
            long totalDuration = 0L;
            bool inProgress = false;
            foreach (var roundEntry in rounds)
            {
                List<BpmAfTaskInst> roundTasks = roundEntry.Value;
                DateTime? minStart = roundTasks
                    .Where(t => t.StartTime != default)
                    .Select(t => (DateTime?)t.StartTime)
                    .Min();
                bool hasUnfinished = roundTasks.Any(t => t.EndTime == null);
                DateTime? maxEnd = hasUnfinished ? null : roundTasks
                    .Where(t => t.EndTime != null)
                    .Select(t => t.EndTime)
                    .Max();

                if (minStart != null)
                {
                    if (maxEnd != null)
                    {
                        totalDuration += (long)(maxEnd.Value - minStart.Value).TotalMilliseconds;
                    }
                    else
                    {
                        // 这一轮有未完成任务
                        totalDuration += (long)(now - minStart.Value).TotalMilliseconds;
                        inProgress = true;
                    }
                }
            }

            // 进行中判定:流程未结束且该节点有未完成 task
            if (!processFinished)
            {
                bool anyUnfinished = nodeTasks.Any(t => t.EndTime == null);
                if (anyUnfinished) inProgress = true;
            }

            InstanceEfficiencyNodeVo vo = new InstanceEfficiencyNodeVo
            {
                TaskDefKey = taskDefKey,
                NodeName = ResolveNodeName(processNumber, taskDefKey, nodeTasks[0].Name),
                Duration = totalDuration,
                DurationText = FormatDuration(totalDuration),
                HasRollback = hasRollback,
                InProgress = inProgress
            };

            // 填充 nodeType
            BpmnNode nodeInfo = ResolveBpmnNode(processNumber, taskDefKey);
            if (nodeInfo != null)
            {
                vo.NodeType = nodeInfo.NodeType;
                vo.NodeTypeName = GetNodeTypeName(nodeInfo.NodeType);
            }

            nodes.Add(vo);
        }

        // 按各节点 min(start_time) 升序排列
        nodes = nodes
            .OrderBy(n =>
            {
                List<BpmAfTaskInst> nt = nodeGroup[n.TaskDefKey];
                return nt
                    .Where(t => t.StartTime != default)
                    .Select(t => (DateTime?)t.StartTime)
                    .Min() ?? DateTime.MinValue;
            })
            .ToList();

        // 编号
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].OrderNo = i + 1;
        }

        // TOP3(进行中节点不参与)
        var rankable = nodes
            .Where(n => !n.InProgress)
            .OrderByDescending(n => n.Duration)
            .Take(3)
            .ToList();
        for (int i = 0; i < rankable.Count; i++)
        {
            rankable[i].TopRank = i + 1;
        }

        return nodes;
    }

    // ==================== 3. 节点详情 ====================

    public InstanceEfficiencyDetailVo GetNodeDetail(string processNumber, string taskDefKey)
    {
        BpmBusinessProcess process = _businessProcessRepository
            .Find(a => a.BusinessNumber == processNumber)
            .FirstOrDefault();

        if (process == null || string.IsNullOrEmpty(process.ProcInstId))
        {
            return null;
        }

        List<BpmAfTaskInst> tasks = _taskInstRepository
            .Find(a => a.ProcInstId == process.ProcInstId)
            .ToList();

        if (tasks.Count == 0)
        {
            return null;
        }

        // 过滤出该节点的 task
        List<BpmAfTaskInst> nodeTasks = tasks
            .Where(t => taskDefKey == t.TaskDefKey)
            .ToList();

        if (nodeTasks.Count == 0)
        {
            return null;
        }

        // 按 execution_id 分轮次,取最后一轮(execution_id 排序最新)
        var rounds = nodeTasks
            .GroupBy(t => t.ExecutionId ?? "null")
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());

        bool hasRollback = rounds.Count > 1;

        // 最后一轮:按 execution_id 字符串排序取最后一个
        string lastExecutionId = rounds.Keys.Max();
        List<BpmAfTaskInst> lastRoundTasks = rounds[lastExecutionId];

        DateTime now = DateTime.Now;
        List<InstanceEfficiencyAssigneeVo> assignees = new List<InstanceEfficiencyAssigneeVo>();
        foreach (BpmAfTaskInst task in lastRoundTasks)
        {
            bool finished = task.EndTime != null;
            long duration;
            if (finished)
            {
                // 优先用 duration 字段;为空时降级用 end-start
                if (task.Duration != null && task.Duration.Value > 0)
                {
                    duration = task.Duration.Value;
                }
                else if (task.StartTime != default)
                {
                    duration = (long)(task.EndTime.Value - task.StartTime).TotalMilliseconds;
                }
                else
                {
                    duration = 0L;
                }
            }
            else
            {
                if (task.StartTime != default)
                {
                    duration = (long)(now - task.StartTime).TotalMilliseconds;
                }
                else
                {
                    duration = 0L;
                }
            }

            assignees.Add(new InstanceEfficiencyAssigneeVo
            {
                Assignee = task.Assignee,
                AssigneeName = ResolveAssigneeName(task),
                StartTime = task.StartTime == default ? null : (DateTime?)task.StartTime,
                EndTime = task.EndTime,
                Finished = finished,
                Duration = duration,
                DurationText = FormatDuration(duration)
            });
        }

        InstanceEfficiencyDetailVo vo = new InstanceEfficiencyDetailVo
        {
            TaskDefKey = taskDefKey,
            NodeName = ResolveNodeName(processNumber, taskDefKey, nodeTasks[0].Name),
            HasRollback = hasRollback,
            Assignees = assignees
        };

        // 签署信息:从 BpmnNode 取 nodeType、nodeProperty、signType
        BpmnNode nodeInfo = ResolveBpmnNode(processNumber, taskDefKey);
        if (nodeInfo != null)
        {
            vo.NodeType = nodeInfo.NodeType;
            vo.NodeTypeName = GetNodeTypeName(nodeInfo.NodeType);
            vo.NodeProperty = nodeInfo.NodeProperty;
            vo.NodePropertyName = GetNodePropertyName(nodeInfo.NodeProperty);

            int? signType = ResolveSignType(nodeInfo);
            vo.SignType = signType;
            vo.SignTypeName = GetSignTypeName(signType);
        }

        return vo;
    }

    // ==================== 辅助方法 ====================

    /// <summary>
    /// 获取流程所有 task 中最晚的 end_time
    /// </summary>
    private DateTime? GetMaxEndTime(string procInstId)
    {
        List<BpmAfTaskInst> tasks = _taskInstRepository
            .Find(a => a.ProcInstId == procInstId)
            .ToList();

        if (tasks.Count == 0) return null;

        return tasks
            .Where(t => t.EndTime != null)
            .Select(t => t.EndTime)
            .Max();
    }

    /// <summary>
    /// 根据 processNumber + taskDefKey 查 BpmnNode
    /// </summary>
    private BpmnNode ResolveBpmnNode(string processNumber, string taskDefKey)
    {
        if (string.IsNullOrEmpty(taskDefKey)) return null;

        try
        {
            NodeElementDto dto = _variableRepository.GetNodeIdByElementId(processNumber, taskDefKey);
            if (dto != null && !string.IsNullOrEmpty(dto.NodeId))
            {
                long nodeId;
                if (long.TryParse(dto.NodeId, out nodeId))
                {
                    return _nodeRepository.GetById(nodeId);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogDebug("效能:获取节点定义失败,processNumber={ProcessNumber},taskDefKey={TaskDefKey},err={Err}",
                processNumber, taskDefKey, e.Message);
        }
        return null;
    }

    /// <summary>
    /// 解析节点名称:优先 t_bpmn_node.node_name,降级用 task.NAME_
    /// </summary>
    private string ResolveNodeName(string processNumber, string taskDefKey, string taskName)
    {
        BpmnNode node = ResolveBpmnNode(processNumber, taskDefKey);
        if (node != null && !string.IsNullOrEmpty(node.NodeName))
        {
            return node.NodeName;
        }
        return taskName;
    }

    /// <summary>
    /// 解析审批人姓名:优先取 AssigneeName,为空则调 UserService
    /// </summary>
    private string ResolveAssigneeName(BpmAfTaskInst task)
    {
        if (!string.IsNullOrEmpty(task.AssigneeName))
        {
            return task.AssigneeName;
        }
        if (!string.IsNullOrEmpty(task.Assignee))
        {
            try
            {
                List<BaseIdTranStruVo> users = _userService.QueryUserByIds(new List<string> { task.Assignee });
                if (users.Count > 0 && users[0] != null)
                {
                    return users[0].Name;
                }
            }
            catch (Exception e)
            {
                _logger.LogDebug("效能:获取审批人姓名失败,assignee={Assignee},err={Err}",
                    task.Assignee, e.Message);
            }
        }
        return null;
    }

    /// <summary>
    /// 从 BpmnNode 解析 signType(按 nodeProperty 分支取对应 Conf)
    /// </summary>
    private int? ResolveSignType(BpmnNode node)
    {
        if (node == null || string.IsNullOrEmpty(node.NodeConfigJson)) return null;

        try
        {
            BpmnNodeConfigJson config = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
            if (config == null || config.ApproverConf == null) return null;

            BpmnNodeApproverConfJson conf = config.ApproverConf;
            int prop = node.NodeProperty;

            // 按 nodeProperty 取对应 Conf 的 signType
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_PERSONNEL && conf.PersonnelConf != null)
            {
                return conf.PersonnelConf.SignType;
            }
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_ROLE && conf.RoleConfList != null && conf.RoleConfList.Count > 0)
            {
                return conf.RoleConfList[0].SignType;
            }
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE && conf.CustomizeConf != null)
            {
                return conf.CustomizeConf.SignType;
            }
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_ZDY_RULES && conf.UdrConfList != null && conf.UdrConfList.Count > 0)
            {
                return conf.UdrConfList[0].SignType;
            }
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_FORM_RELATED && conf.FormRelatedUserConfList != null && conf.FormRelatedUserConfList.Count > 0)
            {
                return conf.FormRelatedUserConfList[0].SignType;
            }
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_PREV_NODE_RELATED && conf.PrevNodeRelatedUserConfList != null && conf.PrevNodeRelatedUserConfList.Count > 0)
            {
                return conf.PrevNodeRelatedUserConfList[0].SignType;
            }
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_OUT_SIDE_ACCESS && conf.OutSideAccessConf != null)
            {
                return conf.OutSideAccessConf.SignType;
            }
            if (prop == (int)NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE && conf.BusinessTableConf != null)
            {
                return conf.BusinessTableConf.SignType;
            }
        }
        catch (Exception e)
        {
            _logger.LogDebug("效能:解析 signType 失败,nodeId={NodeId},err={Err}",
                node?.Id, e.Message);
        }
        return null;
    }

    private string GetNodeTypeName(int? nodeType)
    {
        if (nodeType == null) return null;
        NodeTypeEnum? e = NodeTypeEnumExtensions.GetNodeTypeEnumByCode(nodeType.Value);
        // GetNodeTypeEnumByCode 只返回 hasPropertyTable=1 的节点;降级直接转换
        if (e == null && Enum.IsDefined(typeof(NodeTypeEnum), nodeType.Value))
        {
            e = (NodeTypeEnum)nodeType.Value;
        }
        return e == null ? null : NodeTypeEnumExtensions.GetDesc(e.Value);
    }

    private string GetNodePropertyName(int? nodeProperty)
    {
        if (nodeProperty == null) return null;
        // 优先用 PersonnelEnum(中文更友好),取不到降级 NodePropertyEnum
        NodePropertyEnum? npEnum = NodePropertyEnumExtensions.GetByCode(nodeProperty);
        if (npEnum == null) return null;
        PersonnelEnum? p = PersonnelEnumExtensions.FromNodePropertyEnum(npEnum.Value);
        if (p != null) return p.Value.GetDescription();
        return NodePropertyEnumExtensions.GetDescByCode(nodeProperty);
    }

    private string GetSignTypeName(int? signType)
    {
        if (signType == null) return null;
        if (Enum.IsDefined(typeof(SignTypeEnum), signType.Value))
        {
            return ((SignTypeEnum)signType.Value).GetDescription();
        }
        return null;
    }

    /// <summary>
    /// 耗时格式化:
    /// &lt; 1min → "&lt;1min"
    /// ≥1min → "Xm Xs"
    /// ≥1h → "Xh Xm"
    /// ≥1d → "Xd Xh"
    /// </summary>
    public static string FormatDuration(long ms)
    {
        if (ms < 0) return "0s";
        long totalSeconds = ms / 1000;
        if (totalSeconds < 60)
        {
            return "<1min";
        }
        long days = totalSeconds / 86400;
        long hours = (totalSeconds % 86400) / 3600;
        long minutes = (totalSeconds % 3600) / 60;
        long seconds = totalSeconds % 60;

        if (days > 0)
        {
            return $"{days}d {hours}h";
        }
        if (hours > 0)
        {
            return $"{hours}h {minutes}m";
        }
        return $"{minutes}m {seconds}s";
    }
}
