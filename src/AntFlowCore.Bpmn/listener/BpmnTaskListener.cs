using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service.processor;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.interf;

namespace AntFlowCore.Bpmn.listener;

/// <summary>
/// 任务监听器:任务到达时构建 <see cref="BpmNextTaskDto"/> 并按 Order 顺序调用
/// 所有 <see cref="INextNodeTaskProcessor"/> 实现.
/// 对应 Java BpmnTaskListener,仅负责数据收集与分发,不再承载具体业务逻辑.
/// </summary>
public class BpmnTaskListener : ITaskListener
{
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmProcessForwardService _bpmProcessForwardService;
    private readonly ILogger<BpmnTaskListener> _logger;

    public BpmnTaskListener(
        IBpmnConfService bpmnConfService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmProcessForwardService bpmProcessForwardService,
        ILogger<BpmnTaskListener> logger)
    {
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _logger = logger;
    }

    public void Notify(BpmAfTask delegateTask, string eventName)
    {
        // v1 抄送节点:运行时通过 NodeType 判断,非标签驱动,保持原逻辑
        if (delegateTask.NodeType == (int)NodeTypeEnum.NODE_TYPE_COPY)
        {
            BpmProcessForward bpmProcessForward = new BpmProcessForward()
            {
                CreateUserId = SecurityUtils.GetLogInEmpIdStr(),
                ForwardUserId = delegateTask.Assignee,
                ForwardUserName = delegateTask.AssigneeName,
                ProcessNumber = delegateTask.ProcessNumber,
                ProcessInstanceId = delegateTask.ProcInstId,
                IsRead = 0,
                CreateTime = DateTime.Now,
            };
            _bpmProcessForwardService.AddProcessForward(bpmProcessForward);
            delegateTask.Assignee = AFSpecialAssigneeEnum.COPY_NODE.Id;
            delegateTask.AssigneeName = AFSpecialAssigneeEnum.COPY_NODE.Desc;
            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService.Complete(delegateTask);
            return;
        }

        // 构建下一节点任务上下文 DTO,交由后置处理器链按 Order 顺序处理
        BpmNextTaskDto dto = BuildNextTaskDto(delegateTask);
        var processors = ServiceProviderUtils.GetOrderedServices<INextNodeTaskProcessor>();
        foreach (var processor in processors)
        {
            processor.PostProcess(dto);
        }
    }

    /// <summary>
    /// 从 delegateTask 构建下一节点任务上下文 DTO.
    /// 优先从 ThreadLocalContainer 取 BpmnSendMessageAspect 设置的 businessDataVo(包含运行时 lfFields 等),
    /// 取不到则从 DB 构建基础信息(用于流程启动等场景).
    /// </summary>
    private BpmNextTaskDto BuildNextTaskDto(BpmAfTask delegateTask)
    {
        string processNumber = delegateTask.ProcessNumber;

        // 解析 FormKey 中的节点标签
        List<BpmnNodeLabelVO>? nodeLabels = ParseNodeLabels(delegateTask.FormKey);

        // 优先从 ThreadLocal 取 BpmnSendMessageAspect 设置的 businessDataVo (包含运行时 lfFields 等数据)
        BusinessDataVo? businessDataVo = ThreadLocalContainer.Get(StringConstants.AF_RUNTIME_BUISINESS_INFO) as BusinessDataVo;
        string formCode = string.Empty;
        string bpmnCode = string.Empty;
        string bpmnName = string.Empty;
        bool? isOutSide = false;

        if (businessDataVo != null)
        {
            // ThreadLocal 中有完整的 businessDataVo, 补上当前任务的 TaskDefKey
            businessDataVo.TaskDefKey = delegateTask.TaskDefKey;
            formCode = businessDataVo.FormCode ?? string.Empty;
            bpmnCode = businessDataVo.BpmnCode ?? string.Empty;
            bpmnName = businessDataVo.BpmnName ?? string.Empty;
            isOutSide = businessDataVo.IsOutSideAccessProc;
        }
        else
        {
            // ThreadLocal 中没有(例如流程启动场景), 从 DB 构建基础信息
            BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService._repository
                .Find(a => a.BusinessNumber == processNumber)
                .FirstOrDefault();
            if (bpmBusinessProcess != null)
            {
                BpmnConf? bpmnConf = _bpmnConfService._repository
                    .Find(a => a.BpmnCode == bpmBusinessProcess.Version)
                    .FirstOrDefault();
                if (bpmnConf != null)
                {
                    formCode = bpmBusinessProcess.ProcessinessKey;
                    bpmnCode = bpmnConf.BpmnCode;
                    bpmnName = bpmnConf.BpmnName;
                    isOutSide = (bpmnConf.IsOutSideProcess ?? 0) == 1;

                    businessDataVo = new BusinessDataVo
                    {
                        FormCode = formCode,
                        ProcessNumber = processNumber,
                        TaskDefKey = delegateTask.TaskDefKey,
                        BusinessId = bpmBusinessProcess.BusinessId,
                        BpmnCode = bpmBusinessProcess.Version,
                        IsOutSideAccessProc = isOutSide,
                        IsLowCodeFlow = bpmnConf.IsLowCodeFlow,
                        BpmnConfVo = null,
                    };
                }
                else
                {
                    _logger.LogError("构建 BpmNextTaskDto 失败:流程配置不存在,流程号={}", processNumber);
                }
            }
            else
            {
                _logger.LogError("构建 BpmNextTaskDto 失败:流程实例不存在,流程号={}", processNumber);
            }
        }

        return new BpmNextTaskDto
        {
            TaskId = delegateTask.Id,
            TaskName = delegateTask.Name,
            Assignee = delegateTask.Assignee,
            ProcessNumber = processNumber,
            ProcessInstanceId = delegateTask.ProcInstId,
            TaskDefKey = delegateTask.TaskDefKey,
            BpmnCode = bpmnCode,
            BusinessId = businessDataVo?.BusinessId,
            StartUser = businessDataVo?.StartUserId,
            FormCode = formCode,
            BpmnName = bpmnName,
            IsOutSide = isOutSide,
            NodeLabels = nodeLabels,
            BusinessDataVo = businessDataVo,
            DelegateTask = delegateTask,
        };
    }

    /// <summary>
    /// 从 FormKey(JSON) 中解析节点标签列表.
    /// FormKey 可能是纯 formCode(无标签),也可能是 NodeExtraInfoDTO 的 JSON.
    /// </summary>
    private List<BpmnNodeLabelVO>? ParseNodeLabels(string? formKey)
    {
        if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
        {
            return null;
        }

        try
        {
            NodeExtraInfoDTO? extraInfoDTO = JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
            return extraInfoDTO?.NodeLabelVOS;
        }
        catch
        {
            return null;
        }
    }
}
