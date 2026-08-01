using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using System.Text.Json;

namespace AntFlowCore.Business.service;

public class AFTaskService: IAFTaskService
{
    public AFTaskService(IAFTaskRepository repository)
    {
        _repository = repository;
    }


    public List<BpmAfTask> FindTaskByEmpId(String userId)
    {
        List<BpmAfTask> bpmAfTasks = this._repository.Find(a => a.Assignee == userId);
        return bpmAfTasks;
    }

    public void InsertTasks( List<BpmAfTask> tasks)
    {
        UserEntrustService userEntrustService = ServiceProviderUtils.GetService<UserEntrustService>();
        foreach (BpmAfTask bpmAfTask in tasks)
        {
            // 上一节点指定审批人: 当前任务 FormKey 含 af_syslabel_prev_node_appointed 标签时,
            // 将虚拟审批人 -4 替换为上一节点审批人通过[指定下一节点审批人]按钮选择的实际审批人.
            // 替换发生在委托检查之前, 委托检查自然处理实际审批人 (匹配 Java NextNodeLabelsProcessor 设计)
            ProcessPrevNodeAppointed(bpmAfTask);

            string assignee = bpmAfTask.Assignee;
            string assigneeName = bpmAfTask.AssigneeName;
            BaseIdTranStruVo entrustEmployee = userEntrustService.GetEntrustEmployee(assignee, assigneeName,bpmAfTask.FormKey);
            String userId =entrustEmployee.Id;
            if (!string.IsNullOrEmpty(userId)&&!userId.Equals(assignee))
            {
                String userName=entrustEmployee.Name;
                bpmAfTask.Assignee = userId;
                bpmAfTask.AssigneeName = userName;

                BpmFlowrunEntrust entrust = new BpmFlowrunEntrust()
                {
                    Type = 1,
                    RunTaskId = bpmAfTask.Id,
                    Actual = userId,
                    ActualName = userName,
                    Original = assignee,
                    OriginalName = assigneeName,
                    IsRead = 2,
                    ProcDefId = bpmAfTask.ProcDefId,
                    RunInfoId = bpmAfTask.ProcInstId,
                };
                BpmFlowrunEntrustService bpmFlowrunEntrustService = ServiceProviderUtils.GetService<BpmFlowrunEntrustService>();
                bpmFlowrunEntrustService._repository.Add(entrust);
            }
        }

        this._repository.AddRange(tasks);
    }

    /// <summary>
    /// 上一节点指定审批人替换:
    /// 当前任务 FormKey 含 af_syslabel_prev_node_appointed 标签时,
    /// 将虚拟审批人 PREV_NODE_APPOINTED("-4") 替换为 ThreadLocalContainer.NEXT_NODE_APPROVER 中的实际审批人.
    /// 简化规则: nextNodeApprovers 仅允许 1 人.
    /// - 校验 nextNodeApprovers 非空且 size==1, 否则抛 AFBizException
    /// - 替换 bpmAfTask.Assignee/AssigneeName: "-4" → user1
    /// - 写 BpmFlowrunEntrust 委托记录 (original="-4", actual=user1) — 必然委托
    /// - 清空 ThreadLocalContainer.NEXT_NODE_APPROVER (供后续节点复用)
    /// 对应 Java NextNodeLabelsProcessor.processPrevNodeAppointed.
    /// </summary>
    private void ProcessPrevNodeAppointed(BpmAfTask bpmAfTask)
    {
        string formKey = bpmAfTask.FormKey;
        if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
        {
            return;
        }

        NodeExtraInfoDTO? extraInfoDTO = null;
        try
        {
            extraInfoDTO = JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
        }
        catch
        {
            return;
        }

        if (extraInfoDTO?.NodeLabelVOS == null || extraInfoDTO.NodeLabelVOS.Count == 0)
        {
            return;
        }

        bool hasPrevNodeAppointedLabel = false;
        foreach (var nodeLabelVO in extraInfoDTO.NodeLabelVOS)
        {
            if (StringConstants.AF_SYSLABEL_PREV_NODE_APPOINTED.Equals(nodeLabelVO.LabelValue))
            {
                hasPrevNodeAppointedLabel = true;
                break;
            }
        }
        if (!hasPrevNodeAppointedLabel)
        {
            return;
        }

        // 仅当当前审批人是虚拟审批人 -4 时才替换 (避免对其他审批人误处理)
        if (!AFSpecialAssigneeEnum.PREV_NODE_APPOINTED.Id.Equals(bpmAfTask.Assignee))
        {
            return;
        }

        List<BaseIdTranStruVo>? nextNodeApprovers =
            ThreadLocalContainer.Get(StringConstants.NEXT_NODE_APPROVER) as List<BaseIdTranStruVo>;
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
        string oldUserId = bpmAfTask.Assignee;
        string oldUserName = AFSpecialAssigneeEnum.PREV_NODE_APPOINTED.Desc;
        bpmAfTask.Assignee = user1.Id;
        bpmAfTask.AssigneeName = user1.Name;

        // 必然委托: 写 BpmFlowrunEntrust 记录 (original=虚拟用户, actual=实际用户)
        BpmFlowrunEntrust entrust = new BpmFlowrunEntrust()
        {
            Type = 1,
            RunTaskId = bpmAfTask.Id,
            Actual = user1.Id,
            ActualName = user1.Name,
            Original = oldUserId,
            OriginalName = oldUserName,
            IsRead = 2,
            ProcDefId = bpmAfTask.ProcDefId,
            RunInfoId = bpmAfTask.ProcInstId,
        };
        BpmFlowrunEntrustService bpmFlowrunEntrustService = ServiceProviderUtils.GetService<BpmFlowrunEntrustService>();
        bpmFlowrunEntrustService._repository.Add(entrust);

        // 清空 nextNodeApprovers, 供后续节点复用
        ThreadLocalContainer.Remove(StringConstants.NEXT_NODE_APPROVER);
    }

    public IAFTaskRepository _repository { get; }
}