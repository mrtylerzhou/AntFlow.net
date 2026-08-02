using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service.processor;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Bpmn.adaptor.processoperation;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Core.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.processor.nextnode;

/// <summary>
/// 下一节点标签处理器:对节点标签逐一评估所有处理方法,每个方法内部判断标签是否匹配,
/// 不匹配直接返回,匹配则处理.对应 Java NextNodeLabelsProcessor.
/// Order=0,先于 NextNodeForwardProcessor(委托,order=1) 和 NextNodeProcessNoticeSendProcessor(消息,order=2) 执行.
/// </summary>
public class NextNodeLabelsProcessor : INextNodeTaskProcessor
{
    private readonly IBpmProcessForwardService _bpmProcessForwardService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IFormFactory _formFactory;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly AutoNodeConditionEvaluator _conditionEvaluator;
    private readonly IBpmnNodeService _bpmnNodeService;
    private readonly ForwardToNodeService _forwardToNodeService;
    private readonly BackToModifyService _backToModifyService;
    private readonly ILogger<NextNodeLabelsProcessor> _logger;

    public NextNodeLabelsProcessor(
        IBpmProcessForwardService bpmProcessForwardService,
        IBpmVariableService bpmVariableService,
        IFormFactory formFactory,
        IBpmVerifyInfoService bpmVerifyInfoService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        AutoNodeConditionEvaluator conditionEvaluator,
        IBpmnNodeService bpmnNodeService,
        ForwardToNodeService forwardToNodeService,
        BackToModifyService backToModifyService,
        ILogger<NextNodeLabelsProcessor> logger)
    {
        _bpmProcessForwardService = bpmProcessForwardService;
        _bpmVariableService = bpmVariableService;
        _formFactory = formFactory;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _conditionEvaluator = conditionEvaluator;
        _bpmnNodeService = bpmnNodeService;
        _forwardToNodeService = forwardToNodeService;
        _backToModifyService = backToModifyService;
        _logger = logger;
    }

    public int Order() => 0;

    public void PostProcess(BpmNextTaskDto dto)
    {
        List<BpmnNodeLabelVO>? nodeLabelVOS = dto.NodeLabels;
        if (nodeLabelVOS == null || nodeLabelVOS.Count == 0)
        {
            return;
        }

        BpmAfTask delegateTask = dto.DelegateTask;
        string procInstId = dto.ProcessInstanceId;
        string elementId = dto.TaskDefKey;
        string processNumber = dto.ProcessNumber;
        string assignee = dto.Assignee;
        string assigneeName = dto.TaskName;
        BusinessDataVo? businessDataVo = dto.BusinessDataVo;
        string formCode = dto.FormCode;
        bool isOutSide = dto.IsOutSide ?? false;

        // 按 labelValue 去重,与 Java 一致
        var distinctLabels = nodeLabelVOS
            .GroupBy(l => l.LabelValue)
            .Select(g => g.First())
            .ToList();

        foreach (var nodeLabelVO in distinctLabels)
        {
            ProcessCopy(elementId, processNumber, procInstId, nodeLabelVO);
            ProcessCopyV2(nodeLabelVO, procInstId, assignee, assigneeName, processNumber, delegateTask);
            ProcessAutomaticNode(nodeLabelVO, processNumber, elementId, formCode, businessDataVo, isOutSide, delegateTask);
            ProcessAutoAdvanceNode(nodeLabelVO, processNumber, elementId, formCode, businessDataVo, isOutSide, procInstId, delegateTask);
            ProcessAutoReturnNode(nodeLabelVO, processNumber, elementId, formCode, businessDataVo, isOutSide, procInstId, delegateTask);
            ProcessConditionReturnNode(nodeLabelVO, processNumber, elementId, formCode, businessDataVo, isOutSide, procInstId, delegateTask);
            ProcessAutoSkipNode(nodeLabelVO, assignee, procInstId, assigneeName, processNumber, delegateTask);
            ProcessConditionApproveNode(nodeLabelVO, processNumber, elementId, formCode, businessDataVo, isOutSide, delegateTask);
            ProcessConditionAdvanceNode(nodeLabelVO, processNumber, elementId, formCode, businessDataVo, isOutSide, procInstId, delegateTask);
            ProcessConditionCopyNode(nodeLabelVO, processNumber, elementId, formCode, businessDataVo, isOutSide, procInstId, delegateTask);
            ProcessPrevNodeAppointed(nodeLabelVO, businessDataVo, delegateTask, formCode, processNumber);
        }
    }

    /// <summary>
    /// 抄送节点v1处理:恢复抄送记录可见性(最后一个节点在 ExecutionListener 处理,此处跳过).
    /// </summary>
    private void ProcessCopy(string elementId, string processNumber, string procInstId, BpmnNodeLabelVO nodeLabelVO)
    {
        if (!StringConstants.COPY_NODE.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }
        // 如果是最后一个节点通知,在 BpmnExecutionListener 里面处理,这里跳过,减少数据库查询
        if (StringConstants.LASTNODE_COPY.Equals(elementId))
        {
            return;
        }
        List<string> nodeIdsByElementId = _bpmVariableService.GetNodeIdsByeElementId(processNumber, elementId);
        if (nodeIdsByElementId != null && nodeIdsByElementId.Count > 0)
        {
            string nodeId = nodeIdsByElementId[0];
            var processForwards = _bpmProcessForwardService._repository
                .Find(a => a.ProcessNumber == processNumber && a.NodeId == nodeId);
            foreach (var pf in processForwards)
            {
                pf.ProcessInstanceId = procInstId;
                pf.IsDel = 0; // recover the default state, so that the forward record can be visible
                _bpmProcessForwardService._repository.Update(pf);
            }
        }
    }

    /// <summary>
    /// 抄送节点v2处理:节点以普通审批人身份进入引擎,通过标签识别后自动完成.
    /// </summary>
    private void ProcessCopyV2(BpmnNodeLabelVO nodeLabelVO, string procInstId, string assignee,
        string assigneeName, string processNumber, BpmAfTask delegateTask)
    {
        if (!StringConstants.COPY_NODEV2.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }

        var existingForwards = _bpmProcessForwardService._repository
            .Find(a => a.ProcessInstanceId == procInstId && a.ForwardUserId == assignee);

        delegateTask.Assignee = AFSpecialAssigneeEnum.CC_NODE.Id;
        string ccAssigneeName = AFSpecialAssigneeEnum.CC_NODE.Desc + "(" + assigneeName + ")";
        delegateTask.AssigneeName = ccAssigneeName;

        var taskService = ServiceProviderUtils.GetService<ITaskService>();
        taskService?.Complete(delegateTask);

        if (existingForwards == null || existingForwards.Count == 0)
        {
            BpmProcessForward bpmProcessForward = new BpmProcessForward
            {
                CreateTime = DateTime.Now,
                CreateUserId = assignee,
                ForwardUserId = assignee,
                ForwardUserName = assigneeName,
                ProcessInstanceId = procInstId,
                ProcessNumber = processNumber,
                IsRead = 0,
                IsDel = 0,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmProcessForwardService.AddProcessForward(bpmProcessForward);
        }

        BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = delegateTask.Name,
            TaskId = delegateTask.Id,
            RunInfoId = procInstId,
            VerifyUserId = delegateTask.Assignee,
            VerifyUserName = ccAssigneeName,
            TaskDefKey = delegateTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = "(抄送给" + assigneeName + ")自动通过",
            ProcessCode = processNumber,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
    }

    /// <summary>
    /// 自动节点处理:识别 auto_node 标签后,评估条件、执行动作、自动完成.
    /// 条件评估先调用 formAdaptor.AutomaticCondition(用户可重写);返回 null 时回退到默认 DB 条件评估.
    /// </summary>
    private void ProcessAutomaticNode(BpmnNodeLabelVO nodeLabelVO, string processNumber, string elementId,
        string formCode, BusinessDataVo? businessDataVo, bool isOutSide, BpmAfTask delegateTask)
    {
        if (!StringConstants.AUTOMATIC_NODE.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }

        if (businessDataVo == null)
        {
            _logger.LogError("自动节点处理失败:businessDataVo 为空,processNumber={}", processNumber);
            return;
        }

        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsOutSideAccessProc = isOutSide;

        var formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
        {
            throw new AFBizException($"未能根据流程formcode找到流程适配器信息! formCode={formCode}");
        }

        // 低代码流程:lfConditions 为空时用 lfFields 填充
        if ((businessDataVo.LfConditions == null || businessDataVo.LfConditions.Count == 0)
            && businessDataVo.IsLowCodeFlow == 1)
        {
            businessDataVo.LfConditions = businessDataVo.LfFields;
        }

        string assigneeName = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc;
        bool? conditionResult = null;

        try
        {
            conditionResult = formAdaptor.AutomaticCondition(businessDataVo);
            formAdaptor.AutomaticAction(businessDataVo, conditionResult);

            // 用户未重写 AutomaticCondition(默认返回 null),回退到默认 DB 条件评估
            if (conditionResult == null)
            {
                conditionResult = _conditionEvaluator.Evaluate(businessDataVo);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "自动节点条件判断或动作执行异常, processNumber={}, elementId={}", processNumber, elementId);
        }
        finally
        {
            // 无论条件评估或动作执行是否异常,都自动完成任务
            delegateTask.Assignee = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id;
            delegateTask.AssigneeName = assigneeName;

            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService?.Complete(delegateTask);

            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = delegateTask.Name,
                TaskId = delegateTask.Id,
                RunInfoId = delegateTask.ProcInstId,
                VerifyUserId = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id,
                VerifyUserName = assigneeName,
                TaskDefKey = delegateTask.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.Format(StringConstants.AF_AUTO_EVALUATE_SKIP_COMMENT, conditionResult),
                ProcessCode = processNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
        }
    }

    /// <summary>
    /// 自动推进节点处理:复用自动节点条件评估;条件满足时推进到指定目标节点,
    /// 不满足时和自动节点一样 complete(不跳跃).保留 automaticAction 钩子.
    /// </summary>
    private void ProcessAutoAdvanceNode(BpmnNodeLabelVO nodeLabelVO, string processNumber, string elementId,
        string formCode, BusinessDataVo? businessDataVo, bool isOutSide, string procInstId, BpmAfTask delegateTask)
    {
        if (!StringConstants.AUTO_ADVANCE_NODE.Equals(nodeLabelVO.LabelValue)) return;
        if (businessDataVo == null)
        {
            _logger.LogError("自动推进节点处理失败:businessDataVo 为空,processNumber={}", processNumber);
            return;
        }

        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsOutSideAccessProc = isOutSide;

        var formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
            throw new AFBizException($"未能根据流程formcode找到流程适配器信息! formCode={formCode}");

        if ((businessDataVo.LfConditions == null || businessDataVo.LfConditions.Count == 0)
            && businessDataVo.IsLowCodeFlow == 1)
            businessDataVo.LfConditions = businessDataVo.LfFields;

        string assigneeName = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc;
        string assigneeId = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id;

        bool? conditionResult = null;
        try
        {
            conditionResult = formAdaptor.AutomaticCondition(businessDataVo);
            if (conditionResult == null)
                conditionResult = _conditionEvaluator.Evaluate(businessDataVo);
            formAdaptor.AutomaticAction(businessDataVo, conditionResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "自动推进条件评估或动作执行异常,视为条件不满足, processNumber={}, elementId={}", processNumber, elementId);
            conditionResult = false;
        }

        if (conditionResult == true)
        {
            // 推进路径:读 forwardNodeIds → UUID 转主键 → 主键转 elementId → 调 AdvanceToTargetNode
            // 1. 获取 confId
            BpmnConfVo? bpmnConfVo = businessDataVo.BpmnConfVo;
            if (bpmnConfVo == null || bpmnConfVo.Id == 0)
                throw new AFBizException($"自动推进节点配置读取失败: bpmnConfVo 为空, processNumber={processNumber}, elementId={elementId}");
            long confId = bpmnConfVo.Id;

            // 2. 从 BpmnNode.NodeConfigJson 读 forwardType + forwardNodeIds
            // 通过 elementId 找 nodeId(主键), 再找 BpmnNode
            NodeElementDto? nodeElementDto = _bpmVariableService.GetNodeIdByElementId(processNumber, elementId);
            if (nodeElementDto == null || string.IsNullOrEmpty(nodeElementDto.NodeId))
                throw new AFBizException($"自动推进:无法根据 elementId 找到 nodeId, processNumber={processNumber}, elementId={elementId}");
            long nodePrimaryKey = Convert.ToInt64(nodeElementDto.NodeId);

            // 查 BpmnNode(confId + 主键)
            BpmnNode? bpmnNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.Id == nodePrimaryKey && a.IsDel == 0);
            if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
                throw new AFBizException($"自动推进节点配置读取失败: BpmnNode 不存在或 NodeConfigJson 为空, confId={confId}, nodePrimaryKey={nodePrimaryKey}");

            BpmnNodeConfigJson? configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
            if (configJson == null)
                throw new AFBizException($"自动推进节点配置解析失败, processNumber={processNumber}, elementId={elementId}");

            int? forwardType = configJson.ForwardType;
            List<string>? forwardNodeIds = configJson.ForwardNodeIds;
            if (forwardType == null || forwardType != 2 || forwardNodeIds == null || forwardNodeIds.Count == 0)
                throw new AFBizException($"自动推进节点配置异常: 未配置固定目标节点, processNumber={processNumber}, elementId={elementId}");

            string targetNodeUuid = forwardNodeIds[0];

            // 3. UUID → 主键: 用 confId + node_id(UUID) 查 t_bpmn_node 主键 id
            BpmnNode? targetNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.NodeId == targetNodeUuid && a.IsDel == 0);
            if (targetNode == null)
                throw new AFBizException($"自动推进目标节点不存在, confId={confId}, nodeUuid={targetNodeUuid}");
            string targetNodeName = targetNode.NodeName ?? "";
            string targetPrimaryKey = targetNode.Id.ToString();

            // 4. 主键 → elementId(taskDefKey)
            List<string> targetElementIds = _bpmVariableService.GetElementIdsdByNodeId(processNumber, targetPrimaryKey);
            if (targetElementIds == null || targetElementIds.Count == 0)
                throw new AFBizException($"自动推进:未能根据nodeId获取目标节点taskDefKey, processNumber={processNumber}, targetNodeId={targetPrimaryKey}");
            string targetElementId = targetElementIds[0];

            _logger.LogInformation("自动推进:条件满足,开始推进, processNumber={}, elementId={}, targetElementId={}, targetNodeName={}",
                processNumber, elementId, targetElementId, targetNodeName);

            try
            {
                _forwardToNodeService.AdvanceToTargetNode(delegateTask, procInstId,
                    delegateTask.TaskDefKey, targetElementId, targetNodeName, assigneeId, assigneeName, processNumber);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "自动推进失败, processNumber={}, elementId={}, targetElementId={}, targetNodeName={}",
                    processNumber, elementId, targetElementId, targetNodeName);
                throw new AFBizException($"自动推进失败, processNumber={processNumber}, elementId={elementId}, targetNodeId={targetElementId}, targetNodeName={targetNodeName}", e);
            }
        }
        else
        {
            // 跳过路径:和自动节点一样 complete(不跳跃)
            _logger.LogInformation("自动推进:条件不满足,执行自动跳过, processNumber={}, elementId={}", processNumber, elementId);
            delegateTask.Assignee = assigneeId;
            delegateTask.AssigneeName = assigneeName;
            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService?.Complete(delegateTask);

            BpmVerifyInfo verifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = delegateTask.Name,
                TaskId = delegateTask.Id,
                RunInfoId = procInstId,
                VerifyUserId = assigneeId,
                VerifyUserName = assigneeName,
                TaskDefKey = delegateTask.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.Format(StringConstants.AF_AUTO_EVALUATE_SKIP_COMMENT, conditionResult),
                ProcessCode = processNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmVerifyInfoService.AddVerifyInfo(verifyInfo);
        }
    }

    /// <summary>
    /// 自动退回节点处理:与 ProcessAutoAdvanceNode 对称, 但方向相反(向后退回).
    /// 满足条件 → 退回到 drawBackNodeIds 指定的目标节点(FOUR_DISAGREE)
    /// 不满足条件 → 和自动节点一样 complete(不跳跃)
    /// UUID → 主键 → elementId 转换链路与自动推进一致
    /// </summary>
    private void ProcessAutoReturnNode(BpmnNodeLabelVO nodeLabelVO, string processNumber, string elementId,
        string formCode, BusinessDataVo? businessDataVo, bool isOutSide, string procInstId, BpmAfTask delegateTask)
    {
        if (!StringConstants.AUTO_RETURN_NODE.Equals(nodeLabelVO.LabelValue)) return;
        if (businessDataVo == null)
        {
            _logger.LogError("自动退回节点处理失败:businessDataVo 为空,processNumber={}", processNumber);
            return;
        }

        _logger.LogInformation("自动退回节点处理开始, processNumber={}, elementId={}", processNumber, elementId);

        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsOutSideAccessProc = isOutSide;

        var formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
            throw new AFBizException($"未能根据流程formcode找到流程适配器信息! formCode={formCode}");

        if ((businessDataVo.LfConditions == null || businessDataVo.LfConditions.Count == 0)
            && businessDataVo.IsLowCodeFlow == 1)
            businessDataVo.LfConditions = businessDataVo.LfFields;

        string assigneeName = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc;
        string assigneeId = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id;

        bool? conditionResult = null;
        try
        {
            conditionResult = formAdaptor.AutomaticCondition(businessDataVo);
            if (conditionResult == null)
                conditionResult = _conditionEvaluator.Evaluate(businessDataVo);
            formAdaptor.AutomaticAction(businessDataVo, conditionResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "自动退回条件评估或动作执行异常,视为条件不满足, processNumber={}, elementId={}", processNumber, elementId);
            conditionResult = false;
        }

        if (conditionResult == true)
        {
            // === 退回路径: 退回到指定目标节点 ===
            BpmnConfVo? bpmnConfVo = businessDataVo.BpmnConfVo;
            if (bpmnConfVo == null || bpmnConfVo.Id == 0)
                throw new AFBizException($"自动退回节点配置读取失败: bpmnConfVo 为空, processNumber={processNumber}, elementId={elementId}");
            long confId = bpmnConfVo.Id;

            // 从 BpmnNode.NodeConfigJson 读 drawBackType + drawBackNodeIds
            NodeElementDto? nodeElementDto = _bpmVariableService.GetNodeIdByElementId(processNumber, elementId);
            if (nodeElementDto == null || string.IsNullOrEmpty(nodeElementDto.NodeId))
                throw new AFBizException($"自动退回:无法根据 elementId 找到 nodeId, processNumber={processNumber}, elementId={elementId}");
            long nodePrimaryKey = Convert.ToInt64(nodeElementDto.NodeId);

            BpmnNode? bpmnNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.Id == nodePrimaryKey && a.IsDel == 0);
            if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
                throw new AFBizException($"自动退回节点配置读取失败: BpmnNode 不存在或 NodeConfigJson 为空, confId={confId}, nodePrimaryKey={nodePrimaryKey}");

            BpmnNodeConfigJson? configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
            if (configJson == null)
                throw new AFBizException($"自动退回节点配置解析失败, processNumber={processNumber}, elementId={elementId}");

            int? drawBackType = configJson.DrawBackType;
            List<string>? drawBackNodeIds = configJson.DrawBackNodeIds;
            if (drawBackType == null || (drawBackType != 4 && drawBackType != 2) || drawBackNodeIds == null || drawBackNodeIds.Count == 0)
                throw new AFBizException($"自动退回节点配置异常: 未配置退回目标节点, processNumber={processNumber}, elementId={elementId}");

            string targetNodeUuid = drawBackNodeIds[0];

            // UUID → 主键
            BpmnNode? targetNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.NodeId == targetNodeUuid && a.IsDel == 0);
            if (targetNode == null)
                throw new AFBizException($"自动退回目标节点不存在, confId={confId}, nodeUuid={targetNodeUuid}");
            string targetNodeName = targetNode.NodeName ?? "";
            string targetPrimaryKey = targetNode.Id.ToString();

            // 主键 → elementId(taskDefKey)
            List<string> targetElementIds = _bpmVariableService.GetElementIdsdByNodeId(processNumber, targetPrimaryKey);
            if (targetElementIds == null || targetElementIds.Count == 0)
                throw new AFBizException($"自动退回:未能根据nodeId获取目标节点taskDefKey, processNumber={processNumber}, targetNodeId={targetPrimaryKey}");
            string targetElementId = targetElementIds[0];

            _logger.LogInformation("自动退回:条件满足,开始退回, processNumber={}, elementId={}, targetElementId={}, targetNodeName={}",
                processNumber, elementId, targetElementId, targetNodeName);

            try
            {
                _backToModifyService.ReturnToTargetNode(delegateTask, procInstId, processNumber,
                    delegateTask.TaskDefKey, targetElementId, targetNodeName,
                    assigneeId, "自动退回节点自动退回");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "自动退回失败, processNumber={}, elementId={}, targetElementId={}, targetNodeName={}",
                    processNumber, elementId, targetElementId, targetNodeName);
                throw new AFBizException($"自动退回失败, processNumber={processNumber}, elementId={elementId}, targetNodeId={targetElementId}, targetNodeName={targetNodeName}", e);
            }
        }
        else
        {
            // === 跳过路径: 和自动节点一样 complete(不跳跃) ===
            _logger.LogInformation("自动退回:条件不满足,执行自动跳过, processNumber={}, elementId={}", processNumber, elementId);
            delegateTask.Assignee = assigneeId;
            delegateTask.AssigneeName = assigneeName;
            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService?.Complete(delegateTask);

            BpmVerifyInfo verifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = delegateTask.Name,
                TaskId = delegateTask.Id,
                RunInfoId = procInstId,
                VerifyUserId = assigneeId,
                VerifyUserName = assigneeName,
                TaskDefKey = delegateTask.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.Format(StringConstants.AF_AUTO_EVALUATE_SKIP_COMMENT, conditionResult),
                ProcessCode = processNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmVerifyInfoService.AddVerifyInfo(verifyInfo);
        }
    }

    /// <summary>
    /// 相邻节点去重自动跳过:当前任务审批人在 skippedAssignees 标签的 labelName(逗号分隔ID列表)中,
    /// 则自动完成任务.
    /// </summary>
    private void ProcessAutoSkipNode(BpmnNodeLabelVO nodeLabelVO, string assignee, string procInstId,
        string assigneeName, string processNumber, BpmAfTask delegateTask)
    {
        if (!StringConstants.SKIPPED_ASSIGNEE.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }

        string labelName = nodeLabelVO.LabelName;
        if (string.IsNullOrEmpty(labelName))
        {
            return;
        }

        var skippedAssigneeIds = labelName.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        var currentSkippedAssignee = skippedAssigneeIds.Where(a => a.Contains(assignee)).ToList();
        if (currentSkippedAssignee.Count > 0)
        {
            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService?.Complete(delegateTask);

            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = delegateTask.Name,
                TaskId = delegateTask.Id,
                RunInfoId = procInstId,
                VerifyUserId = delegateTask.Assignee,
                VerifyUserName = assigneeName,
                TaskDefKey = delegateTask.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = StringConstants.AF_AUTO_SKIP_COMMENT,
                ProcessCode = processNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
        }
    }

    /// <summary>
    /// 条件推进节点处理: 条件审批(nodeType=12)子类型, 自动勾选推进按钮(42,别名"同意"), 强制 forwardType=2(固定目标).
    /// - 满足条件: 自动推进到固定目标节点(用虚拟人-3标识系统自动推进), 复用自动推进的推进逻辑(AdvanceToTargetNode)
    /// - 不满足: 不 complete, 留给真实审批人人工处理(审批人点"同意"=推进按钮, 推进到配置的固定目标)
    /// </summary>
    private void ProcessConditionAdvanceNode(BpmnNodeLabelVO nodeLabelVO, string processNumber, string elementId,
        string formCode, BusinessDataVo? businessDataVo, bool isOutSide, string procInstId, BpmAfTask delegateTask)
    {
        // 条件完成节点复用此处理器(与条件推进运行时逻辑完全一致, 仅设计时目标来源不同)
        if (!StringConstants.CONDITION_ADVANCE_NODE.Equals(nodeLabelVO.LabelValue)
            && !StringConstants.CONDITION_FINISH_NODE.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }

        if (businessDataVo == null)
        {
            _logger.LogError("条件推进节点处理失败:businessDataVo 为空,processNumber={}", processNumber);
            return;
        }

        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsOutSideAccessProc = isOutSide;

        var formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
        {
            throw new AFBizException($"未能根据流程formcode找到流程适配器信息! formCode={formCode}");
        }

        if ((businessDataVo.LfConditions == null || businessDataVo.LfConditions.Count == 0)
            && businessDataVo.IsLowCodeFlow == 1)
        {
            businessDataVo.LfConditions = businessDataVo.LfFields;
        }

        string assigneeName = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc;
        string assigneeId = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id;

        bool? conditionResult = null;
        try
        {
            conditionResult = formAdaptor.AutomaticCondition(businessDataVo);
            if (conditionResult == null)
            {
                conditionResult = _conditionEvaluator.Evaluate(businessDataVo);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "条件推进节点条件判断异常, processNumber={}, elementId={}", processNumber, elementId);
        }

        // 满足条件: 自动推进到固定目标节点 (用虚拟人-3标识系统自动推进)
        if (conditionResult == true)
        {
            BpmnConfVo? bpmnConfVo = businessDataVo.BpmnConfVo;
            if (bpmnConfVo == null || bpmnConfVo.Id == 0)
                throw new AFBizException($"条件推进节点配置读取失败: bpmnConfVo 为空, processNumber={processNumber}, elementId={elementId}");
            long confId = bpmnConfVo.Id;

            NodeElementDto? nodeElementDto = _bpmVariableService.GetNodeIdByElementId(processNumber, elementId);
            if (nodeElementDto == null || string.IsNullOrEmpty(nodeElementDto.NodeId))
                throw new AFBizException($"条件推进:无法根据 elementId 找到 nodeId, processNumber={processNumber}, elementId={elementId}");
            long nodePrimaryKey = Convert.ToInt64(nodeElementDto.NodeId);

            BpmnNode? bpmnNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.Id == nodePrimaryKey && a.IsDel == 0);
            if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
                throw new AFBizException($"条件推进节点配置读取失败: BpmnNode 不存在或 NodeConfigJson 为空, confId={confId}, nodePrimaryKey={nodePrimaryKey}");

            BpmnNodeConfigJson? configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
            if (configJson == null)
                throw new AFBizException($"条件推进节点配置解析失败, processNumber={processNumber}, elementId={elementId}");

            int? forwardType = configJson.ForwardType;
            List<string>? forwardNodeIds = configJson.ForwardNodeIds;
            if (forwardType == null || forwardType != 2 || forwardNodeIds == null || forwardNodeIds.Count == 0)
                throw new AFBizException($"条件推进节点配置异常: 未配置固定目标节点, processNumber={processNumber}, elementId={elementId}");

            string targetNodeUuid = forwardNodeIds[0];

            BpmnNode? targetNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.NodeId == targetNodeUuid && a.IsDel == 0);
            if (targetNode == null)
                throw new AFBizException($"条件推进目标节点不存在, confId={confId}, nodeUuid={targetNodeUuid}");
            string targetNodeName = targetNode.NodeName ?? "";
            string targetPrimaryKey = targetNode.Id.ToString();

            List<string> targetElementIds = _bpmVariableService.GetElementIdsdByNodeId(processNumber, targetPrimaryKey);
            if (targetElementIds == null || targetElementIds.Count == 0)
                throw new AFBizException($"条件推进:未能根据nodeId获取目标节点taskDefKey, processNumber={processNumber}, targetNodeId={targetPrimaryKey}");
            string targetElementId = targetElementIds[0];

            _logger.LogInformation("条件推进:条件满足,开始推进, processNumber={}, elementId={}, targetElementId={}, targetNodeName={}",
                processNumber, elementId, targetElementId, targetNodeName);

            try
            {
                _forwardToNodeService.AdvanceToTargetNode(delegateTask, procInstId,
                    delegateTask.TaskDefKey, targetElementId, targetNodeName, assigneeId, assigneeName, processNumber);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "条件推进失败, processNumber={}, elementId={}, targetElementId={}, targetNodeName={}",
                    processNumber, elementId, targetElementId, targetNodeName);
                throw new AFBizException($"条件推进失败, processNumber={processNumber}, elementId={elementId}, targetNodeId={targetElementId}, targetNodeName={targetNodeName}", e);
            }
        }
        // conditionResult == false 或 null: 不 complete, 留给真实审批人(点"同意"=推进按钮, 推进到配置的固定目标)
    }

    /// <summary>
    /// 条件退回节点处理:满足条件时自动退回到不同意按钮配置的目标节点,不满足时留给真实审批人.
    /// </summary>
    private void ProcessConditionReturnNode(BpmnNodeLabelVO nodeLabelVO, string processNumber, string elementId,
        string formCode, BusinessDataVo? businessDataVo, bool isOutSide, string procInstId, BpmAfTask delegateTask)
    {
        if (!StringConstants.CONDITION_RETURN_NODE.Equals(nodeLabelVO.LabelValue)) return;
        _logger.LogInformation("条件退回节点处理开始, processNumber={PN}, elementId={E}", processNumber, elementId);
        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsOutSideAccessProc = isOutSide;
        var formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null) throw new AFBizException("未能根据流程formcode找到流程适配器信息!");
        bool? conditionResult = null;
        try { conditionResult = formAdaptor.AutomaticCondition(businessDataVo); }
        catch (Exception e) { _logger.LogError(e, "条件退回条件评估异常"); conditionResult = false; }
        if (conditionResult == true)
        {
            BpmnConfVo? bpmnConfVo = businessDataVo.BpmnConfVo;
            if (bpmnConfVo == null || bpmnConfVo.Id == 0)
                throw new AFBizException($"条件退回节点配置读取失败: bpmnConfVo 为空, processNumber={processNumber}, elementId={elementId}");
            long confId = bpmnConfVo.Id;

            // 从 BpmnNode.NodeConfigJson 读取 backType + backToNodeId
            NodeElementDto? nodeElementDto = _bpmVariableService.GetNodeIdByElementId(processNumber, elementId);
            if (nodeElementDto == null || string.IsNullOrEmpty(nodeElementDto.NodeId))
                throw new AFBizException($"条件退回: 无法根据 elementId 找到 nodeId, processNumber={processNumber}, elementId={elementId}");
            long nodePrimaryKey = Convert.ToInt64(nodeElementDto.NodeId);

            BpmnNode? bpmnNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.Id == nodePrimaryKey && a.IsDel == 0);
            if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
                throw new AFBizException($"条件退回节点配置读取失败: BpmnNode 不存在或 NodeConfigJson 为空, confId={confId}, nodePrimaryKey={nodePrimaryKey}");

            BpmnNodeConfigJson? configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
            if (configJson == null)
                throw new AFBizException($"条件退回节点配置解析失败, processNumber={processNumber}, elementId={elementId}");

            int? backType = configJson.BackType;
            string? backToNodeId = configJson.BackToNodeId;
            if (backType == null || (backType != 4 && backType != 5) || string.IsNullOrEmpty(backToNodeId))
                throw new AFBizException($"条件退回节点配置异常: 未配置退回目标节点, processNumber={processNumber}, elementId={elementId}");

            // UUID → 主键
            BpmnNode? targetNode = _bpmnNodeService._repository
                .FirstOrDefault(a => a.ConfId == confId && a.NodeId == backToNodeId && a.IsDel == 0);
            if (targetNode == null)
                throw new AFBizException($"条件退回目标节点不存在, confId={confId}, nodeUuid={backToNodeId}");
            string targetNodeName = targetNode.NodeName ?? "";
            string targetPrimaryKey = targetNode.Id.ToString();

            // 主键 → elementId(taskDefKey)
            List<string> targetElementIds = _bpmVariableService.GetElementIdsdByNodeId(processNumber, targetPrimaryKey);
            if (targetElementIds == null || targetElementIds.Count == 0)
                throw new AFBizException($"条件退回: 未能根据nodeId获取目标节点taskDefKey, processNumber={processNumber}, targetNodeId={targetPrimaryKey}");
            string targetElementId = targetElementIds[0];

            _logger.LogInformation("条件退回: 条件满足, 开始退回, processNumber={PN}, elementId={E}, target={T}, backType={B}",
                processNumber, elementId, targetElementId, backType);

            try
            {
                _backToModifyService.ReturnToTargetNode(delegateTask, procInstId, processNumber,
                    delegateTask.TaskDefKey, targetElementId, targetNodeName,
                    AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id, "条件退回节点自动退回", backType.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "条件退回失败, processNumber={PN}, elementId={E}, target={T}",
                    processNumber, elementId, targetElementId);
                throw new AFBizException($"条件退回失败, processNumber={processNumber}, elementId={elementId}, targetNodeId={targetElementId}", e);
            }
        }
        // conditionResult == false 或 null: 不操作, 留给真实审批人人工处理
    }

    /// <summary>
    /// 条件审批节点处理:复用自动节点条件评估;仅当 conditionResult==true 时才 complete 任务,
    /// 否则留给真实审批人人工处理.不调用 automaticAction.
    /// </summary>
    private void ProcessConditionApproveNode(BpmnNodeLabelVO nodeLabelVO, string processNumber, string elementId,
        string formCode, BusinessDataVo? businessDataVo, bool isOutSide, BpmAfTask delegateTask)
    {
        if (!StringConstants.CONDITION_APPROVE_NODE.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }

        if (businessDataVo == null)
        {
            _logger.LogError("条件审批节点处理失败:businessDataVo 为空,processNumber={}", processNumber);
            return;
        }

        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsOutSideAccessProc = isOutSide;

        var formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
        {
            throw new AFBizException($"未能根据流程formcode找到流程适配器信息! formCode={formCode}");
        }

        if ((businessDataVo.LfConditions == null || businessDataVo.LfConditions.Count == 0)
            && businessDataVo.IsLowCodeFlow == 1)
        {
            businessDataVo.LfConditions = businessDataVo.LfFields;
        }

        bool? conditionResult = null;
        try
        {
            conditionResult = formAdaptor.AutomaticCondition(businessDataVo);
            if (conditionResult == null)
            {
                conditionResult = _conditionEvaluator.Evaluate(businessDataVo);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "条件审批节点条件判断异常, processNumber={}, elementId={}", processNumber, elementId);
        }

        // 仅当条件满足时才自动 complete; 否则留给真实审批人
        if (conditionResult == true)
        {
            string assigneeName = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc;
            delegateTask.Assignee = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id;
            delegateTask.AssigneeName = assigneeName;

            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService?.Complete(delegateTask);

            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = delegateTask.Name,
                TaskId = delegateTask.Id,
                RunInfoId = delegateTask.ProcInstId,
                VerifyUserId = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id,
                VerifyUserName = assigneeName,
                TaskDefKey = delegateTask.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.Format(StringConstants.AF_CONDITION_APPROVE_AUTO_COMMENT, conditionResult),
                ProcessCode = processNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
        }
        // conditionResult == false 或 null: 不 complete, 留给真实审批人
    }

    /// <summary>
    /// 条件抄送节点处理:复用自动节点条件评估;无论条件如何都 complete(assignee=CC_NODE),
    /// 仅条件满足时写 BpmProcessForward 抄送记录.不调用 automaticAction.
    /// </summary>
    private void ProcessConditionCopyNode(BpmnNodeLabelVO nodeLabelVO, string processNumber, string elementId,
        string formCode, BusinessDataVo? businessDataVo, bool isOutSide, string procInstId, BpmAfTask delegateTask)
    {
        if (!StringConstants.CONDITION_COPY_NODE.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }

        if (businessDataVo == null)
        {
            _logger.LogError("条件抄送节点处理失败:businessDataVo 为空,processNumber={}", processNumber);
            return;
        }

        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsOutSideAccessProc = isOutSide;

        var formAdaptor = _formFactory.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
        {
            throw new AFBizException($"未能根据流程formcode找到流程适配器信息! formCode={formCode}");
        }

        if ((businessDataVo.LfConditions == null || businessDataVo.LfConditions.Count == 0)
            && businessDataVo.IsLowCodeFlow == 1)
        {
            businessDataVo.LfConditions = businessDataVo.LfFields;
        }

        bool? conditionResult = null;
        try
        {
            conditionResult = formAdaptor.AutomaticCondition(businessDataVo);
            if (conditionResult == null)
            {
                conditionResult = _conditionEvaluator.Evaluate(businessDataVo);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "条件抄送节点条件判断异常, processNumber={}, elementId={}", processNumber, elementId);
        }

        string assignee = AFSpecialAssigneeEnum.CC_NODE.Id;
        string assigneeName = AFSpecialAssigneeEnum.CC_NODE.Desc;
        string originalAssigneeName = delegateTask.AssigneeName;
        delegateTask.Assignee = assignee;
        string ccAssigneeName = assigneeName + "(" + originalAssigneeName + ")";
        delegateTask.AssigneeName = ccAssigneeName;

        var taskService = ServiceProviderUtils.GetService<ITaskService>();
        taskService?.Complete(delegateTask);

        // 仅条件满足时写抄送记录
        if (conditionResult == true)
        {
            var existingForwards = _bpmProcessForwardService._repository
                .Find(a => a.ProcessInstanceId == procInstId && a.ForwardUserId == assignee);
            if (existingForwards == null || existingForwards.Count == 0)
            {
                BpmProcessForward bpmProcessForward = new BpmProcessForward
                {
                    CreateTime = DateTime.Now,
                    CreateUserId = assignee,
                    ForwardUserId = assignee,
                    ForwardUserName = ccAssigneeName,
                    ProcessInstanceId = procInstId,
                    ProcessNumber = processNumber,
                    IsRead = 0,
                    IsDel = 0,
                    TenantId = MultiTenantUtil.GetCurrentTenantId(),
                };
                _bpmProcessForwardService.AddProcessForward(bpmProcessForward);
            }
        }

        // 写 verifyInfo (文案随条件结果变化)
        string comment = conditionResult == true
            ? StringConstants.AF_CONDITION_COPY_EXECUTE_COMMENT
            : StringConstants.AF_CONDITION_COPY_SKIP_COMMENT;

        BpmVerifyInfo verifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = delegateTask.Name,
            TaskId = delegateTask.Id,
            RunInfoId = procInstId,
            VerifyUserId = assignee,
            VerifyUserName = ccAssigneeName,
            TaskDefKey = delegateTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = comment,
            ProcessCode = processNumber,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmVerifyInfoService.AddVerifyInfo(verifyInfo);
    }

    /// <summary>
    /// 上一节点指定审批人处理:将虚拟审批人 PREV_NODE_APPOINTED("-4") 替换为
    /// 上一节点审批人提交的 nextNodeApprovers 中的实际审批人.简化规则:仅允许1人.
    /// </summary>
    private void ProcessPrevNodeAppointed(BpmnNodeLabelVO nodeLabelVO, BusinessDataVo? businessDataVo,
        BpmAfTask delegateTask, string formCode, string processNumber)
    {
        if (!StringConstants.AF_SYSLABEL_PREV_NODE_APPOINTED.Equals(nodeLabelVO.LabelValue))
        {
            return;
        }

        List<BaseIdTranStruVo>? nextNodeApprovers = businessDataVo?.NextNodeApprovers;
        if (nextNodeApprovers == null || nextNodeApprovers.Count == 0)
        {
            throw new AFBizException("上一节点指定审批人未指定,请在上一节点审批时通过[指定下一节点审批人]按钮选择审批人");
        }
        if (nextNodeApprovers.Count != 1)
        {
            throw new AFBizException("上一节点指定审批人仅允许指定1人,当前指定了" + nextNodeApprovers.Count + "人");
        }
        BaseIdTranStruVo user1 = nextNodeApprovers[0];
        if (user1 == null || string.IsNullOrEmpty(user1.Id))
        {
            throw new AFBizException("上一节点指定审批人信息不完整");
        }

        // 替换虚拟审批人为实际审批人
        string oldUserId = delegateTask.Assignee;
        string oldUserName = AFSpecialAssigneeEnum.PREV_NODE_APPOINTED.Desc;
        delegateTask.Assignee = user1.Id;
        delegateTask.AssigneeName = user1.Name;

        // 必然委托:写 BpmFlowrunEntrust 记录
        _bpmFlowrunEntrustService.AddFlowrunEntrust(
            user1.Id, user1.Name, oldUserId, oldUserName,
            delegateTask.Id, 1, delegateTask.ProcInstId, formCode,
            delegateTask.TaskDefKey, 1);

        _logger.LogInformation("上一节点指定审批人替换: processNumber={}, original={}, actual={}", processNumber, oldUserId, user1.Id);

        // 清空 nextNodeApprovers,供后续节点复用
        businessDataVo!.NextNodeApprovers = null;
    }
}
