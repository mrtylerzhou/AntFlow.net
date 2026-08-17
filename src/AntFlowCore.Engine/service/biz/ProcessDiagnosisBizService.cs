using System.Reflection;
using System.Text.Json;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// 流程诊断 (流程管理-流程监控-更多-流程诊断), 与 Java 版 ProcessDiagnosisBizServiceImpl 对等.
/// 归因短路矩阵: ①present(hi∪ru node_id) ②NOT_REACHED(先序序) ③CONDITION_MISS(全分支横评)
/// ④SIGN_SKIP(entrust actionType=3) ⑤UNKNOWN; 节点存在时附带人员维度(4.3, 应审人/实际审批人/加减签标记)
/// </summary>
public class ProcessDiagnosisBizService : IProcessDiagnosisBizService
{
    private const int NodeTypeStart = (int)NodeTypeEnum.NODE_TYPE_START;
    private const int NodeTypeGateway = (int)NodeTypeEnum.NODE_TYPE_GATEWAY;
    private const int NodeTypeConditionBranch = (int)NodeTypeEnum.NODE_TYPE_CONDITIONS;
    /// <summary>加批按钮 buttonType (approvalButtonConf addApproval)</summary>
    private const int ButtonTypeAddApproval = 19;
    /// <summary>bpm_verify_info.verify_status = 9 加批</summary>
    private const int VerifyStatusAddApproval = 9;

    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmnConfBizService _bpmnConfBizService;
    private readonly IBpmnConfCommonService _bpmnConfCommonService;
    private readonly IFormFactory _formFactory;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly IAFTaskService _afTaskService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProvider;
    private readonly ILogger<ProcessDiagnosisBizService> _logger;

    public ProcessDiagnosisBizService(
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmnConfBizService bpmnConfBizService,
        IBpmnConfCommonService bpmnConfCommonService,
        IFormFactory formFactory,
        IAfTaskInstService afTaskInstService,
        IAFTaskService afTaskService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IBpmVerifyInfoService bpmVerifyInfoService,
        IBpmVariableService bpmVariableService,
        IBpmnEmployeeInfoProviderService employeeInfoProvider,
        ILogger<ProcessDiagnosisBizService> logger)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmnConfBizService = bpmnConfBizService;
        _bpmnConfCommonService = bpmnConfCommonService;
        _formFactory = formFactory;
        _afTaskInstService = afTaskInstService;
        _afTaskService = afTaskService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _bpmVariableService = bpmVariableService;
        _employeeInfoProvider = employeeInfoProvider;
        _logger = logger;
    }

    // ==================================================================================
    // diagnosisInit
    // ==================================================================================

    public ProcessDiagnosisInitVo DiagnosisInit(string processNumber)
    {
        BpmBusinessProcess process = QueryProcess(processNumber);
        BpmnConfVo confVo = _bpmnConfBizService.Detail(process.Version);
        if (confVo == null || confVo.Id == 0)
        {
            throw new AFBizException($"未找到流程版本对应的模板配置, bpmnCode={process.Version}");
        }

        return new ProcessDiagnosisInitVo
        {
            ProcessNumber = processNumber,
            ConfId = confVo.Id,
            BpmnCode = process.Version,
            FormCode = confVo.FormCode,
            IsLowCodeFlow = process.IsLowCodeFlow,
            ProcessFinished = process.ProcessState != (int)ProcessStateEnum.HANDLING_STATE,
            InitiatorUserId = process.CreateUser,
            InitiatorUserName = ResolveUserName(process),
            FormValues = LoadFormValues(process, confVo.FormCode),
        };
    }

    // ==================================================================================
    // diagnoseNode
    // ==================================================================================

    public NodeDiagnosisVo DiagnoseNode(NodeDiagnosisRequestVo request)
    {
        if (request == null || string.IsNullOrEmpty(request.ProcessNumber) || request.NodeId == 0)
        {
            throw new AFBizException("processNumber / nodeId 不能为空");
        }
        string processNumber = request.ProcessNumber;
        long targetId = request.NodeId;

        BpmBusinessProcess process = QueryProcess(processNumber);
        BpmnConfVo confVo = _bpmnConfBizService.Detail(process.Version);
        bool finished = process.ProcessState != (int)ProcessStateEnum.HANDLING_STATE;

        // ---- 设计树索引 ----
        List<BpmnNodeVo> nodes = confVo?.Nodes ?? new List<BpmnNodeVo>();
        Dictionary<string, BpmnNodeVo> byUuid = nodes
            .Where(n => !string.IsNullOrEmpty(n.NodeId))
            .ToDictionary(n => n.NodeId);
        Dictionary<long, BpmnNodeVo> byId = nodes
            .Where(n => n.Id > 0)
            .ToDictionary(n => n.Id);
        BpmnNodeVo? target = byId.TryGetValue(targetId, out var t) ? t : null;
        string targetName = target != null && !string.IsNullOrEmpty(target.NodeName)
            ? target.NodeName : targetId.ToString();

        // ---- 审批真实路径 ----
        string procInstId = process.ProcInstId;
        List<BpmAfTaskInst> hiTasks = _afTaskInstService._repository
            .Find(a => a.ProcInstId == procInstId).ToList();
        List<BpmAfTask> ruTasks = _afTaskService._repository
            .Find(a => a.ProcInstId == procInstId).ToList();

        string idStr = targetId.ToString();
        bool present = hiTasks.Any(t => t.NodeId == idStr) || ruTasks.Any(t => t.NodeId == idStr);

        // 当前停留节点: 优先运行中任务, 否则最近完成的历史任务
        string? currentNodeId = ruTasks.Where(t => !string.IsNullOrEmpty(t.NodeId))
            .Select(t => t.NodeId).FirstOrDefault();
        if (string.IsNullOrEmpty(currentNodeId))
        {
            currentNodeId = hiTasks.Where(t => !string.IsNullOrEmpty(t.NodeId))
                .OrderByDescending(t => t.EndTime)
                .FirstOrDefault()?.NodeId;
        }
        BpmnNodeVo? currentNode = !string.IsNullOrEmpty(currentNodeId) && byId.TryGetValue(
            long.TryParse(currentNodeId, out var cid) ? cid : 0, out var cnode) ? cnode : null;
        string? currentNodeName = currentNode != null && !string.IsNullOrEmpty(currentNode.NodeName)
            ? currentNode.NodeName : currentNodeId;

        var vo = new NodeDiagnosisVo
        {
            Present = present,
            ExpectationMismatch = request.ExpectedPresent != null && request.ExpectedPresent != present,
            NodeName = targetName,
            CurrentNodeId = currentNodeId,
            CurrentNodeName = currentNodeName,
            PrevNodeName = null,
        };

        // ---- 公共明细: 加减签/委托 (4.3), 加批记录 (4.2), 兜底 task ----
        List<NodeDiagnosisVo.EntrustRecordVo> entrustRecords = LoadEntrustRecords(procInstId, targetId);
        vo.EntrustRecords = entrustRecords;
        vo.SignupRecords = LoadSignupRecords(processNumber, targetId);

        // 前驱真实节点(跳过网关/分支头)及其加批按钮配置
        BpmnNodeVo? prevNode = FindPrevRealNode(target, byUuid);
        bool prevHasAddApproval = false;
        if (prevNode?.Buttons?.ApprovalPage != null)
        {
            prevHasAddApproval = prevNode.Buttons.ApprovalPage.Any(
                b => b.ButtonType != null && b.ButtonType == ButtonTypeAddApproval);
        }
        vo.PrevNodeHasAddApproval = prevHasAddApproval;
        vo.PrevNodeName = prevNode?.NodeName;

        vo.RawTasks = LoadRawTasks(hiTasks, ruTasks, targetId);

        // ---- 短路矩阵 ----
        if (present)
        {
            // 人员维度(4.3): 前提=节点存在
            string ruleDesc = RuleDescOf(target);
            List<NodeDiagnosisVo.ApproverVo> expected = EvaluateExpectedApprovers(process, confVo, targetId);
            List<NodeDiagnosisVo.ApproverVo> actual = LoadActualApprovers(hiTasks, ruTasks, targetId);
            vo.RuleDesc = ruleDesc;
            vo.ExpectedApprovers = expected;
            vo.ActualApprovers = actual;
            if (!string.IsNullOrEmpty(request.PersonId))
            {
                vo.PersonDiagnosis = BuildPersonDiagnosis(request, expected, actual, entrustRecords, ruleDesc, targetName);
            }
            vo.ConclusionType = "EXISTS";
            vo.Message = "该节点在流程审批路径中实际存在过。";
            return vo;
        }

        // ② 尚未到达 (仅流程未结束时判断, 排在条件求值之前防伪结论)
        if (!finished)
        {
            Dictionary<long, int> orderMap = BuildPreOrderIndex(nodes);
            int? targetIdx = orderMap.TryGetValue(targetId, out var ti) ? ti : (int?)null;
            int? currentIdx = !string.IsNullOrEmpty(currentNodeId) && long.TryParse(currentNodeId, out var ccid)
                && orderMap.TryGetValue(ccid, out var ci) ? ci : (int?)null;
            if (targetIdx != null && currentIdx != null && targetIdx > currentIdx)
            {
                vo.ConclusionType = "NOT_REACHED";
                vo.Message = $"流程尚未执行到该节点, 当前停留节点: {(string.IsNullOrEmpty(currentNodeName) ? "未知" : currentNodeName)}。";
                return vo;
            }
        }

        // ③ 条件分支横评
        Dictionary<string, object> formValues = LoadFormValues(process, confVo?.FormCode);
        BpmnNodeVo? branchHead = FindAncestorBranchHead(target, byUuid);
        if (branchHead != null)
        {
            var branches = EvaluateBranchFamily(branchHead, targetId, nodes, formValues);
            vo.Branches = branches;
            var targetBranch = branches.FirstOrDefault(b => b.ContainsTarget);
            bool someBranchHit = branches.Any(b => b.Hit == true);
            NodeDiagnosisVo.BranchEvaluation? hitBranch = branches.FirstOrDefault(b => b.Hit == true);
            hitBranch ??= branches.FirstOrDefault(b => b.IsDefault);
            bool targetBranchNotHit = targetBranch != null
                && targetBranch.Hit != true
                && !targetBranch.IsDefault
                && (someBranchHit || hitBranch != null);
            if (targetBranchNotHit)
            {
                string hitName = hitBranch != null && !string.IsNullOrEmpty(hitBranch.BranchName)
                    ? hitBranch.BranchName : "其他分支";
                vo.ConclusionType = "CONDITION_MISS";
                vo.Message = $"目标节点所在分支的条件未命中, 实际命中分支: {hitName} (条件按当前表单值求值)。";
                return vo;
            }
        }

        // ④ 减签跳过
        bool hasRemoveSign = entrustRecords.Any(r => r.ActionType != null && r.ActionType == 3);
        if (hasRemoveSign)
        {
            vo.ConclusionType = "SIGN_SKIP";
            vo.Message = "该节点存在减签记录, 节点可能因人员被减签而跳过, 详见下方加减签记录。";
            return vo;
        }

        // ⑤ 兜底
        vo.ConclusionType = "UNKNOWN";
        vo.Message = "无法自动归因, 以下为该节点相关的原始记录, 请结合审批记录与表单变更记录人工分析。";
        return vo;
    }

    // ==================================================================================
    // helpers: business process / form values
    // ==================================================================================

    private BpmBusinessProcess QueryProcess(string processNumber)
    {
        BpmBusinessProcess? process = _bpmBusinessProcessService._repository
            .FirstOrDefault(a => a.BusinessNumber == processNumber);
        if (process == null)
        {
            throw new AFBizException($"流程实例不存在: {processNumber}");
        }
        return process;
    }

    private string? ResolveUserName(BpmBusinessProcess process)
    {
        if (!string.IsNullOrEmpty(process.UserName))
        {
            return process.UserName;
        }
        try
        {
            var map = _employeeInfoProvider.ProvideEmployeeInfo(new List<string> { process.CreateUser });
            return map.TryGetValue(process.CreateUser, out var name) ? name : null;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "query initiator name failed, user={}", process.CreateUser);
            return null;
        }
    }

    /// <summary>
    /// 当前业务表单真实值: LF → lfFields + lfFieldsMulti(反射) 合并; DIY → 实体属性(排除 BusinessDataVo 基类)。
    /// 查询失败不阻塞诊断, 返回空 map。
    /// </summary>
    private Dictionary<string, object> LoadFormValues(BpmBusinessProcess process, string? formCode)
    {
        var values = new Dictionary<string, object>();
        try
        {
            var paramObj = new
            {
                processNumber = process.BusinessNumber,
                isLowCodeFlow = process.IsLowCodeFlow,
                isOutSideAccessProc = false,
            };
            BusinessDataVo vo = _formFactory.DataFormConversion(JsonSerializer.Serialize(paramObj), formCode);
            vo.BusinessId = process.BusinessId;
            IFormOperationAdaptor<BusinessDataVo> adaptor = _formFactory.GetFormAdaptor(vo);
            adaptor.OnQueryData(vo);

            if (vo.IsLowCodeFlow == 1)
            {
                if (vo.LfFields != null)
                {
                    foreach (var kv in vo.LfFields)
                    {
                        values[kv.Key] = kv.Value;
                    }
                }
                var multi = ReadLfFieldsMulti(vo);
                if (multi != null)
                {
                    foreach (var inner in multi.Values)
                    {
                        if (inner != null)
                        {
                            foreach (var kv in inner)
                            {
                                values[kv.Key] = kv.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                Type type = vo.GetType();
                while (type != null && type != typeof(BusinessDataVo) && type != typeof(object))
                {
                    foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        if (p.GetIndexParameters().Length > 0)
                        {
                            continue;
                        }
                        try
                        {
                            values[p.Name] = p.GetValue(vo);
                        }
                        catch (Exception)
                        {
                            // 忽略单个属性读取失败
                        }
                    }
                    type = type.BaseType;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "load form values failed, processNumber={}", process.BusinessNumber);
        }
        return values;
    }

    /// <summary>反射读取 vo.lfFieldsMulti (UDLFApplyVo 字段), 不存在返回 null。</summary>
    private Dictionary<string, Dictionary<string, object>>? ReadLfFieldsMulti(BusinessDataVo vo)
    {
        try
        {
            PropertyInfo? p = vo.GetType().GetProperty("LfFieldsMulti");
            return p?.GetValue(vo) as Dictionary<string, Dictionary<string, object>>;
        }
        catch (Exception)
        {
            return null;
        }
    }

    // ==================================================================================
    // helpers: design tree
    // ==================================================================================

    /// <summary>从发起节点先序遍历, 建立节点主键 id → 全序 index (分支按优先级序在前, 主干在后)。</summary>
    private Dictionary<long, int> BuildPreOrderIndex(List<BpmnNodeVo> nodes)
    {
        var order = new Dictionary<long, int>();
        var childrenByFrom = new Dictionary<string, List<BpmnNodeVo>>();
        foreach (BpmnNodeVo n in nodes)
        {
            if (!string.IsNullOrEmpty(n.NodeFrom))
            {
                if (!childrenByFrom.TryGetValue(n.NodeFrom, out var list))
                {
                    list = new List<BpmnNodeVo>();
                    childrenByFrom[n.NodeFrom] = list;
                }
                list.Add(n);
            }
        }
        foreach (var children in childrenByFrom.Values)
        {
            children.Sort((x, y) =>
            {
                int xBranch = x.NodeType == NodeTypeConditionBranch ? 0 : 1;
                int yBranch = y.NodeType == NodeTypeConditionBranch ? 0 : 1;
                if (xBranch != yBranch)
                {
                    return xBranch.CompareTo(yBranch);
                }
                int xs = x.Property?.Sort ?? int.MaxValue;
                int ys = y.Property?.Sort ?? int.MaxValue;
                return xs.CompareTo(ys);
            });
        }
        BpmnNodeVo? start = nodes.FirstOrDefault(n => n.NodeType == NodeTypeStart);
        int counter = 0;
        if (start != null)
        {
            DfsOrder(start, childrenByFrom, order, ref counter);
        }
        return order;
    }

    private void DfsOrder(BpmnNodeVo node, Dictionary<string, List<BpmnNodeVo>> childrenByFrom,
        Dictionary<long, int> order, ref int counter)
    {
        if (node.Id <= 0 || order.ContainsKey(node.Id))
        {
            return;
        }
        order[node.Id] = counter++;
        if (childrenByFrom.TryGetValue(node.NodeId, out var children))
        {
            foreach (var child in children)
            {
                DfsOrder(child, childrenByFrom, order, ref counter);
            }
        }
    }

    /// <summary>沿 nodeFrom 向上找最近的"真实"前驱节点(跳过条件分支头与网关)。</summary>
    private BpmnNodeVo? FindPrevRealNode(BpmnNodeVo? target, Dictionary<string, BpmnNodeVo> byUuid)
    {
        if (target == null || string.IsNullOrEmpty(target.NodeFrom))
        {
            return null;
        }
        BpmnNodeVo? p = byUuid.TryGetValue(target.NodeFrom, out var v) ? v : null;
        int guard = 0;
        while (p != null && guard++ < 100)
        {
            if (p.NodeType == NodeTypeConditionBranch || p.NodeType == NodeTypeGateway)
            {
                p = !string.IsNullOrEmpty(p.NodeFrom) && byUuid.TryGetValue(p.NodeFrom, out var pv) ? pv : null;
            }
            else
            {
                return p;
            }
        }
        return null;
    }

    /// <summary>沿 nodeFrom 向上找目标节点所在的分支头(type3); 不在条件分支上返回 null。</summary>
    private BpmnNodeVo? FindAncestorBranchHead(BpmnNodeVo? target, Dictionary<string, BpmnNodeVo> byUuid)
    {
        BpmnNodeVo? q = target;
        int guard = 0;
        while (q != null && guard++ < 100)
        {
            if (q.NodeType == NodeTypeConditionBranch)
            {
                return q;
            }
            q = !string.IsNullOrEmpty(q.NodeFrom) && byUuid.TryGetValue(q.NodeFrom, out var v) ? v : null;
        }
        return null;
    }

    /// <summary>目标节点所在网关的全分支横评: 每分支 条件/当前实际值/求值结果。</summary>
    private List<NodeDiagnosisVo.BranchEvaluation> EvaluateBranchFamily(BpmnNodeVo branchHead, long targetId,
        List<BpmnNodeVo> nodes, Dictionary<string, object> formValues)
    {
        List<BpmnNodeVo> siblings = nodes
            .Where(n => n.NodeType == NodeTypeConditionBranch && n.NodeFrom == branchHead.NodeFrom)
            .OrderBy(n => n.Property?.Sort ?? int.MaxValue)
            .ToList();

        var childrenByFrom = new Dictionary<string, List<BpmnNodeVo>>();
        foreach (BpmnNodeVo n in nodes)
        {
            if (!string.IsNullOrEmpty(n.NodeFrom))
            {
                if (!childrenByFrom.TryGetValue(n.NodeFrom, out var list))
                {
                    list = new List<BpmnNodeVo>();
                    childrenByFrom[n.NodeFrom] = list;
                }
                list.Add(n);
            }
        }

        var result = new List<NodeDiagnosisVo.BranchEvaluation>();
        foreach (BpmnNodeVo branch in siblings)
        {
            var subtreeIds = new HashSet<long>();
            CollectSubtreeIds(branch, childrenByFrom, subtreeIds);

            bool isDefault = branch.Property?.IsDefault == 1;
            var conditionList = branch.Property?.ConditionList;
            bool groupRelation = branch.Property?.GroupRelation ?? false;

            var items = new List<NodeDiagnosisVo.ConditionItemResult>();
            bool? hit = null;
            if (!isDefault && conditionList != null && conditionList.Count > 0)
            {
                hit = EvaluateConditions(conditionList, groupRelation, formValues);
                foreach (var group in conditionList)
                {
                    if (group == null)
                    {
                        continue;
                    }
                    foreach (var cond in group)
                    {
                        bool? single = EvaluateConditions(
                            new List<List<BpmnNodeConditionsConfVueVo>> { new List<BpmnNodeConditionsConfVueVo> { cond } },
                            false, formValues);
                        items.Add(ToConditionItemResult(cond, formValues, single == true));
                    }
                }
            }

            result.Add(new NodeDiagnosisVo.BranchEvaluation
            {
                BranchName = branch.NodeName,
                Priority = branch.Property?.Sort,
                IsDefault = isDefault,
                Hit = hit,
                ContainsTarget = subtreeIds.Contains(targetId),
                Conditions = items,
            });
        }
        return result;
    }

    private NodeDiagnosisVo.ConditionItemResult ToConditionItemResult(BpmnNodeConditionsConfVueVo cond,
        Dictionary<string, object> formValues, bool pass)
    {
        object? actual = !string.IsNullOrEmpty(cond.ColumnDbname) && formValues.TryGetValue(cond.ColumnDbname, out var fv)
            ? fv : null;
        string expect = string.IsNullOrEmpty(cond.Zdy1) ? "" : cond.Zdy1;
        if (!string.IsNullOrEmpty(cond.Zdy2))
        {
            expect = expect + (string.IsNullOrEmpty(cond.Opt2) ? "~" : cond.Opt2) + cond.Zdy2;
        }
        return new NodeDiagnosisVo.ConditionItemResult
        {
            Label = cond.ShowName,
            FieldName = cond.ColumnDbname,
            FieldTypeName = cond.FieldTypeName,
            OpText = OpText(cond),
            ExpectText = expect,
            ActualValue = actual == null ? "" : actual.ToString(),
            Pass = pass,
        };
    }

    private static string OpText(BpmnNodeConditionsConfVueVo cond)
    {
        switch (cond.OptType)
        {
            case 1: return ">=";
            case 2: return ">";
            case 3: return "<=";
            case 4: return "<";
            case 5: return "==";
            case 6:
            case 7:
            case 8:
            case 9: return "介于";
            default: return "=";
        }
    }

    private void CollectSubtreeIds(BpmnNodeVo node, Dictionary<string, List<BpmnNodeVo>> childrenByFrom, HashSet<long> acc)
    {
        if (node == null || node.Id <= 0 || !acc.Add(node.Id))
        {
            return;
        }
        if (childrenByFrom.TryGetValue(node.NodeId, out var children))
        {
            foreach (var child in children)
            {
                CollectSubtreeIds(child, childrenByFrom, acc);
            }
        }
    }

    /// <summary>条件求值 (与 AutoNodeConditionEvaluator 同规则): conditionList 二级列表 + groupRelation。</summary>
    private static bool? EvaluateConditions(List<List<BpmnNodeConditionsConfVueVo>> conditionList,
        bool groupRelation, Dictionary<string, object> formFields)
    {
        if (conditionList == null || conditionList.Count == 0)
        {
            return null;
        }
        bool isOrBetweenGroups = groupRelation;
        bool overallResult = !isOrBetweenGroups;
        foreach (var group in conditionList)
        {
            if (group == null || group.Count == 0)
            {
                continue;
            }
            bool groupResult = EvaluateConditionGroup(group, formFields);
            if (isOrBetweenGroups)
            {
                overallResult = overallResult || groupResult;
                if (overallResult)
                {
                    break;
                }
            }
            else
            {
                overallResult = overallResult && groupResult;
                if (!overallResult)
                {
                    break;
                }
            }
        }
        return overallResult;
    }

    private static bool EvaluateConditionGroup(List<BpmnNodeConditionsConfVueVo> group, Dictionary<string, object> formFields)
    {
        bool isOrWithinGroup = group[0].CondRelation;
        bool groupResult = !isOrWithinGroup;
        foreach (var item in group)
        {
            bool itemResult = EvaluateSingleCondition(item, formFields);
            if (isOrWithinGroup)
            {
                groupResult = groupResult || itemResult;
                if (groupResult)
                {
                    break;
                }
            }
            else
            {
                groupResult = groupResult && itemResult;
                if (!groupResult)
                {
                    break;
                }
            }
        }
        return groupResult;
    }

    private static bool EvaluateSingleCondition(BpmnNodeConditionsConfVueVo item, Dictionary<string, object> formFields)
    {
        string fieldName = item.ColumnDbname;
        if (string.IsNullOrEmpty(fieldName))
        {
            return false;
        }
        formFields.TryGetValue(fieldName, out object? formValue);
        string formValueStr = formValue != null ? formValue.ToString() : "";
        string targetValue = item.Zdy1 ?? "";
        string fieldTypeName = item.FieldTypeName ?? "";
        int? optType = item.OptType;

        if (fieldTypeName == "switch")
        {
            return ("1" == formValueStr) == ("1" == targetValue);
        }
        if (fieldTypeName == "select" || fieldTypeName == "radio")
        {
            return targetValue == formValueStr;
        }
        if (fieldTypeName == "checkbox")
        {
            if (string.IsNullOrEmpty(formValueStr) || string.IsNullOrEmpty(targetValue))
            {
                return false;
            }
            return formValueStr.Split(',').Contains(targetValue);
        }
        if (fieldTypeName == "number" || fieldTypeName == "date" || fieldTypeName == "time")
        {
            try
            {
                return CompareNumeric(formValueStr, targetValue, optType, item.Zdy2, item.Opt1, item.Opt2);
            }
            catch (FormatException)
            {
                return false;
            }
        }
        return targetValue == formValueStr;
    }

    private static bool CompareNumeric(string formValueStr, string targetValue, int? optType,
        string zdy2, string opt1, string opt2)
    {
        if (string.IsNullOrEmpty(formValueStr) || string.IsNullOrEmpty(targetValue))
        {
            return false;
        }
        double formVal = double.Parse(formValueStr);
        double target = double.Parse(targetValue);
        if (optType == null)
        {
            return formVal == target;
        }
        switch (optType.Value)
        {
            case 1: return formVal >= target;
            case 2: return formVal > target;
            case 3: return formVal <= target;
            case 4: return formVal < target;
            case 5: return formVal == target;
            case 6:
            case 7:
            case 8:
            case 9:
                if (string.IsNullOrEmpty(zdy2))
                {
                    return false;
                }
                double target2 = double.Parse(zdy2);
                bool leftBound = opt1 == "<" ? formVal > target : formVal >= target;
                bool rightBound = opt2 == "<" ? formVal < target2 : formVal <= target2;
                return leftBound && rightBound;
            default:
                return formVal == target;
        }
    }

    // ==================================================================================
    // helpers: records
    // ==================================================================================

    private List<NodeDiagnosisVo.EntrustRecordVo> LoadEntrustRecords(string procInstId, long nodeId)
    {
        List<BpmFlowrunEntrust> records = _bpmFlowrunEntrustService._repository
            .Find(a => a.RunInfoId == procInstId && a.NodeId == nodeId.ToString())
            .OrderByDescending(a => a.Id)
            .ToList();
        return records.Select(r => new NodeDiagnosisVo.EntrustRecordVo
        {
            ActionType = r.ActionType,
            ActionTypeName = ActionTypeName(r.ActionType),
            OriginalId = r.Original,
            OriginalName = r.OriginalName,
            ActualId = r.Actual,
            ActualName = r.ActualName,
            NodeId = r.NodeId,
        }).ToList();
    }

    private static string ActionTypeName(int? actionType)
    {
        switch (actionType)
        {
            case 0:
            case 1: return "转办";
            case 2: return "加签";
            case 3: return "减签";
            case 4: return "表单关联刷新";
            default: return actionType == null ? "未知" : $"未知({actionType})";
        }
    }

    /// <summary>加批记录: bpm_verify_info(verifyStatus=9) + variableConfigJson.signUps。</summary>
    private List<NodeDiagnosisVo.SignupRecordVo> LoadSignupRecords(string processNumber, long nodeId)
    {
        var result = new List<NodeDiagnosisVo.SignupRecordVo>();
        try
        {
            List<BpmVerifyInfo> verifies = _bpmVerifyInfoService._repository
                .Find(a => a.ProcessCode == processNumber && a.VerifyStatus == VerifyStatusAddApproval)
                .ToList();
            foreach (var v in verifies)
            {
                result.Add(new NodeDiagnosisVo.SignupRecordVo
                {
                    UserName = v.VerifyUserName,
                    VerifyDate = v.VerifyDate,
                    VerifyDesc = v.VerifyDesc,
                    Source = "verify_info",
                });
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "query signup verify info failed, processNumber={}", processNumber);
        }
        try
        {
            BpmVariable? variable = _bpmVariableService._repository
                .Find(a => a.ProcessNum == processNumber && a.IsDel == 0)
                .FirstOrDefault();
            if (variable != null && !string.IsNullOrEmpty(variable.VariableConfigJson))
            {
                VariableConfigJson? config = JsonConfUtil.ParseVariableConfig(variable.VariableConfigJson);
                if (config?.SignUps != null)
                {
                    foreach (var signUp in config.SignUps)
                    {
                        if (signUp.NodeId == nodeId.ToString())
                        {
                            result.Add(new NodeDiagnosisVo.SignupRecordVo
                            {
                                UserName = ExtractSignUpNames(signUp),
                                Source = "sign_up_config",
                            });
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "query signUps config failed, processNumber={}", processNumber);
        }
        return result;
    }

    private static string? ExtractSignUpNames(VariableSignUpItem signUp)
    {
        if (signUp.PersonnelByElement == null)
        {
            return null;
        }
        var names = new HashSet<string>();
        foreach (var personnel in signUp.PersonnelByElement.Values)
        {
            if (personnel == null)
            {
                continue;
            }
            foreach (var p in personnel)
            {
                string n = !string.IsNullOrEmpty(p.AssigneeName) ? p.AssigneeName : p.Assignee;
                if (!string.IsNullOrEmpty(n))
                {
                    names.Add(n);
                }
            }
        }
        return names.Count > 0 ? string.Join(",", names) : null;
    }

    private List<NodeDiagnosisVo.RawTaskVo> LoadRawTasks(List<BpmAfTaskInst> hiTasks, List<BpmAfTask> ruTasks, long nodeId)
    {
        var result = new List<NodeDiagnosisVo.RawTaskVo>();
        string idStr = nodeId.ToString();
        foreach (var t in hiTasks)
        {
            if (t.NodeId == idStr)
            {
                result.Add(new NodeDiagnosisVo.RawTaskVo
                {
                    TaskId = t.Id,
                    TaskName = t.Name,
                    AssigneeName = t.AssigneeName,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    DeleteReason = t.DeleteReason,
                    NodeId = t.NodeId,
                    Source = "hi",
                });
            }
        }
        foreach (var t in ruTasks)
        {
            if (t.NodeId == idStr)
            {
                result.Add(new NodeDiagnosisVo.RawTaskVo
                {
                    TaskId = t.Id,
                    TaskName = t.Name,
                    AssigneeName = t.AssigneeName,
                    NodeId = t.NodeId,
                    Source = "ru",
                });
            }
        }
        return result;
    }

    // ==================================================================================
    // helpers: 人员维度 (4.3)
    // ==================================================================================

    private static string? RuleDescOf(BpmnNodeVo? node)
    {
        if (node == null)
        {
            return null;
        }
        string? desc = NodePropertyEnumExtensions.GetDescByCode(node.NodeProperty);
        return !string.IsNullOrEmpty(desc) ? desc : node.NodePropertyName;
    }

    /// <summary>
    /// 应审人: 复用 preview 链路(引擎同源规则评估, 不带 processNumber 走当前表单值)后,
    /// 自行应用该节点加减签/委托标记(name 后缀 +加签 / -减签 / *转办)。
    /// </summary>
    private List<NodeDiagnosisVo.ApproverVo> EvaluateExpectedApprovers(BpmBusinessProcess process,
        BpmnConfVo? confVo, long targetId)
    {
        try
        {
            Dictionary<string, object> formValues = LoadFormValues(process, confVo?.FormCode);
            var paramObj = new Dictionary<string, object?>
            {
                ["isStartPreview"] = false,
                ["formCode"] = confVo?.FormCode,
                ["isLowCodeFlow"] = process.IsLowCodeFlow == 1,
                ["isOutSideAccessProc"] = false,
                ["bpmnCode"] = process.Version,
                ["startUserId"] = process.CreateUser,
            };
            if (process.IsLowCodeFlow == 1)
            {
                paramObj["lfFields"] = formValues;
                paramObj["lfConditions"] = new Dictionary<string, object>();
            }
            else
            {
                paramObj["lfFields"] = new Dictionary<string, object>();
                var conds = new Dictionary<string, object>();
                foreach (string f in CollectConditionFieldNames(confVo))
                {
                    if (formValues.TryGetValue(f, out var fv))
                    {
                        conds[f] = fv;
                    }
                }
                paramObj["lfConditions"] = conds;
            }

            string paramsJson = JsonSerializer.Serialize(paramObj);
            PreviewNode previewNode = _bpmnConfCommonService.TaskPagePreviewNode(paramsJson);
            if (previewNode?.BpmnNodeList == null || previewNode.BpmnNodeList.Count == 0)
            {
                return new List<NodeDiagnosisVo.ApproverVo>();
            }
            BpmnNodeVo? node = previewNode.BpmnNodeList.FirstOrDefault(n => n.Id == targetId);
            if (node?.Property?.EmplList == null || node.Property.EmplList.Count == 0)
            {
                return new List<NodeDiagnosisVo.ApproverVo>();
            }

            var result = node.Property.EmplList
                .Select(e => new NodeDiagnosisVo.ApproverVo
                {
                    UserId = e.Id,
                    Name = e.Name,
                    Source = "config",
                })
                .ToList();

            List<BpmFlowrunEntrust> entrusts = _bpmFlowrunEntrustService._repository
                .Find(a => a.RunInfoId == process.ProcInstId && a.NodeId == targetId.ToString())
                .ToList();
            foreach (var r in entrusts)
            {
                if (r.ActionType == 0 || r.ActionType == 1)
                {
                    // 转办: original → actual, 标记 *
                    foreach (var a in result)
                    {
                        if (a.UserId == r.Original)
                        {
                            a.UserId = r.Actual;
                            a.Name = (string.IsNullOrEmpty(r.ActualName) ? r.Actual : r.ActualName) + "*";
                            a.Mark = "*";
                        }
                    }
                }
                else if (r.ActionType == 2)
                {
                    // 加签: 追加, 标记 +
                    result.Add(new NodeDiagnosisVo.ApproverVo
                    {
                        UserId = r.Actual,
                        Name = (string.IsNullOrEmpty(r.ActualName) ? r.Actual : r.ActualName) + "+",
                        Mark = "+",
                        Source = "addSign",
                    });
                }
                else if (r.ActionType == 3)
                {
                    // 减签: 原列表标记 -
                    foreach (var a in result)
                    {
                        if (a.UserId == r.Actual)
                        {
                            a.Name = a.Name + "-";
                            a.Mark = "-";
                        }
                    }
                }
            }
            return result;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "evaluate expected approvers failed, processNumber={}, nodeId={}",
                process.BusinessNumber, targetId);
            return new List<NodeDiagnosisVo.ApproverVo>();
        }
    }

    /// <summary>收集设计树上所有条件分支字段名(type==2, columnDbname), DIY 模式 lfConditions 用。</summary>
    private HashSet<string> CollectConditionFieldNames(BpmnConfVo? confVo)
    {
        var names = new HashSet<string>();
        if (confVo?.Nodes == null)
        {
            return names;
        }
        foreach (BpmnNodeVo n in confVo.Nodes)
        {
            if (n.NodeType != NodeTypeConditionBranch || n.Property?.ConditionList == null)
            {
                continue;
            }
            foreach (var group in n.Property.ConditionList)
            {
                if (group == null)
                {
                    continue;
                }
                foreach (var c in group)
                {
                    if (c.Type == 2 && !string.IsNullOrEmpty(c.ColumnDbname))
                    {
                        names.Add(c.ColumnDbname);
                    }
                }
            }
        }
        return names;
    }

    /// <summary>实际审批人: 该节点 task assignee (hi ∪ ru), 按 userId 去重。</summary>
    private List<NodeDiagnosisVo.ApproverVo> LoadActualApprovers(List<BpmAfTaskInst> hiTasks,
        List<BpmAfTask> ruTasks, long nodeId)
    {
        var map = new Dictionary<string, NodeDiagnosisVo.ApproverVo>();
        string idStr = nodeId.ToString();
        foreach (var t in hiTasks)
        {
            if (t.NodeId == idStr && !string.IsNullOrEmpty(t.Assignee) && !map.ContainsKey(t.Assignee))
            {
                map[t.Assignee] = new NodeDiagnosisVo.ApproverVo
                {
                    UserId = t.Assignee,
                    Name = t.AssigneeName,
                    Source = "hi",
                    Time = t.EndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                };
            }
        }
        foreach (var t in ruTasks)
        {
            if (t.NodeId == idStr && !string.IsNullOrEmpty(t.Assignee) && !map.ContainsKey(t.Assignee))
            {
                map[t.Assignee] = new NodeDiagnosisVo.ApproverVo
                {
                    UserId = t.Assignee,
                    Name = t.AssigneeName,
                    Source = "ru",
                };
            }
        }
        return map.Values.ToList();
    }

    /// <summary>
    /// 人员维度结论: 前提=节点存在。判定 该人是否实际审批 + 与预期对账 + 归因。
    /// 存在侧: 加签引入 > 转办引入 > 配置命中 > 应审人命中 > 推断; 缺失侧: 减签移除 > 转办给他人 > 配置不含 > 推断。
    /// </summary>
    private NodeDiagnosisVo.PersonDiagnosisVo BuildPersonDiagnosis(NodeDiagnosisRequestVo request,
        List<NodeDiagnosisVo.ApproverVo> expected, List<NodeDiagnosisVo.ApproverVo> actual,
        List<NodeDiagnosisVo.EntrustRecordVo> entrusts, string? ruleDesc, string nodeName)
    {
        string personId = request.PersonId;
        bool presentPerson = actual.Any(a => a.UserId == personId);
        bool mismatch = request.ExpectedPersonPresent != null && request.ExpectedPersonPresent != presentPerson;
        string? personName = ResolvePersonName(personId, expected, actual, entrusts);

        bool fromAddSign = entrusts.Any(r => r.ActionType == 2 && r.ActualId == personId);
        bool fromRemoveSign = entrusts.Any(r => r.ActionType == 3 && r.ActualId == personId);
        bool fromDelegateOut = entrusts.Any(r => (r.ActionType == 0 || r.ActionType == 1) && r.OriginalId == personId);
        bool fromDelegateIn = entrusts.Any(r => (r.ActionType == 0 || r.ActionType == 1) && r.ActualId == personId);
        bool inConfig = expected.Any(a => a.UserId == personId && string.IsNullOrEmpty(a.Mark));
        bool inExpected = expected.Any(a => a.UserId == personId);

        string reason;
        bool inference = false;
        string rule = string.IsNullOrEmpty(ruleDesc) ? "未知" : ruleDesc;
        if (presentPerson)
        {
            if (fromAddSign)
            {
                reason = "由加签引入: 该节点存在加签记录, 此人被加签为审批人";
            }
            else if (fromDelegateIn)
            {
                reason = "由转办引入: 原审批人将该节点转办给此人(代审)";
            }
            else if (inConfig)
            {
                reason = $"配置规则命中: 节点规则「{rule}」评估的应审人包含此人";
            }
            else if (inExpected)
            {
                reason = "应审人评估命中(含运行期人员调整)";
            }
            else
            {
                inference = true;
                reason = "实际审批出现此人, 但配置规则与应审人评估均未直接命中, 可能为动态评估差异(如角色/表单相关人在审批时变更)";
            }
        }
        else
        {
            if (fromRemoveSign)
            {
                reason = "被减签移除: 该节点存在减签记录, 此人被移出审批人";
            }
            else if (fromDelegateOut)
            {
                reason = "转办给他人: 此人将该节点转办给其他审批人";
            }
            else if (!inConfig)
            {
                reason = $"配置规则不含此人: 节点规则「{rule}」评估的应审人中无此人";
            }
            else
            {
                inference = true;
                reason = "配置规则含此人但实际未审批, 可能为动态评估差异(如角色成员/表单相关人在审批时变更)";
            }
        }

        string head = presentPerson ? "该节点实际有此审批人" : "该节点实际无此审批人";
        string msg = head + (mismatch ? " (与你的预期相反)" : " (与你的预期一致)") + "。原因: " + reason;
        return new NodeDiagnosisVo.PersonDiagnosisVo
        {
            PersonId = personId,
            PersonName = personName,
            PresentPerson = presentPerson,
            ExpectationMismatch = mismatch,
            Message = msg,
            Inference = inference,
            InferenceNote = inference ? reason : null,
        };
    }

    private string? ResolvePersonName(string personId, List<NodeDiagnosisVo.ApproverVo> expected,
        List<NodeDiagnosisVo.ApproverVo> actual, List<NodeDiagnosisVo.EntrustRecordVo> entrusts)
    {
        foreach (var a in actual)
        {
            if (a.UserId == personId && !string.IsNullOrEmpty(a.Name))
            {
                return a.Name;
            }
        }
        foreach (var a in expected)
        {
            if (a.UserId == personId && !string.IsNullOrEmpty(a.Name))
            {
                string n = a.Name;
                if (n.EndsWith("+") || n.EndsWith("-") || n.EndsWith("*"))
                {
                    n = n[..^1];
                }
                return n;
            }
        }
        foreach (var r in entrusts)
        {
            if (r.ActualId == personId && !string.IsNullOrEmpty(r.ActualName))
            {
                return r.ActualName;
            }
            if (r.OriginalId == personId && !string.IsNullOrEmpty(r.OriginalName))
            {
                return r.OriginalName;
            }
        }
        try
        {
            var map = _employeeInfoProvider.ProvideEmployeeInfo(new List<string> { personId });
            return map.TryGetValue(personId, out var name) ? name : personId;
        }
        catch (Exception)
        {
            return personId;
        }
    }
}
