using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
/// 未来节点后添加节点实现
/// 核心逻辑：将目标节点变成顺序会签类型，然后增加审批人
/// </summary>
public class InsertNodeAfterCurrentOrFutureService : AbstractAddOrRemoveFutureAssigneeSerivce, IProcessOperationAdaptor
{
    private readonly IAFTaskService _afTaskService;
    private readonly IFreeSql _freeSql;

    public InsertNodeAfterCurrentOrFutureService(
        IBpmBusinessProcessService bpmBusinessProcessService,
        TaskService taskService,
        IAfTaskInstService afTaskInstService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IAFDeploymentService afDeploymentService,
        IBpmvariableBizService bpmvariableBizService,
        IAFTaskService afTaskService,
        IFreeSql freeSql) : base(bpmBusinessProcessService, taskService, afTaskInstService, bpmFlowrunEntrustService, afDeploymentService, bpmvariableBizService)
    {
        _afTaskService = afTaskService;
        _freeSql = freeSql;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        vo.NodeId = "xxx";
        base.checkParam(vo);
        string processNumber = vo.ProcessNumber;
        string taskDefKey = vo.TaskDefKey;
        List<BaseIdTranStruVo> userInfos = vo.UserInfos;

        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException("未能根据流程编号找到流程信息:" + processNumber);
        }

        NodeElementDto nodeElementDto = _bpmvariableBizService.GetNodeIdByElementId(processNumber, taskDefKey);
        if (nodeElementDto == null)
        {
            throw new AFBizException("未能根据元素id获取到节点id" + taskDefKey);
        }
        vo.NodeId = nodeElementDto.NodeId;
        string nodeId = vo.NodeId;
        // 从流程定义中获取节点信息
        List<BpmnConfCommonElementVo> elements = _afDeploymentService.GetDeploymentByProcessNumber(processNumber);
        BpmnConfCommonElementVo element = elements.FirstOrDefault(a => a.NodeId == nodeId);
        if (element == null)
        {
            throw new AFBizException($"流程编号:{processNumber},节点id:{nodeId},未在流程图中找到对应定义");
        }

        // 如果不是多实例节点（SignType == 0），不支持插入节点
        if (element.SignType == 0)
        {
            throw new AFBizException("当前节点不是多实例节点，不支持插入节点");
        }

        List<BpmAfTask> currTasks = _afTaskService.baseRepo.Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId).ToList();
        if(currTasks.IsEmpty()){
            throw new AFBizException("未能根据流程实例id找到任务信息");
        }

        BpmAfTask oneTask  = currTasks[0];
        string oneTaskProcDefId = oneTask.ProcDefId;
        List<string> taskDefKeys=currTasks.Select(a=>a.TaskDefKey).ToList();
        //如果当前正在执行的任务改不是顺序会签改成会签可能导致执行不完,暂不处理当前节点类型的
        // 如果不是顺序会签，需要先将节点改为顺序会签
        if (element.SignType != SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode()&& !taskDefKeys.Contains(element.ElementId))
        {
            element.SignType = SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode();
            UpdateDeploymentSignType(oneTaskProcDefId, elements);
        }


        // 增加审批人
        base.ModifyFutureAssigneesByProcessInstance(bpmBusinessProcess, vo, nodeElementDto, userInfos, 1);
    }

    /// <summary>
    /// 更新流程定义中的节点签收类型
    /// </summary>
    private void UpdateDeploymentSignType(string procDefId, List<BpmnConfCommonElementVo> elements)
    {
        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a=>a.Id==procDefId).First();

        if (bpmAfDeployment == null)
        {
            throw new AFBizException($"未能根据流程定义Id:{procDefId}找到流程定义!");
        }

        bpmAfDeployment.Content = JsonSerializer.Serialize(elements);
        bpmAfDeployment.Rev += 1;
        bpmAfDeployment.UpdateTime = DateTime.Now;
        bpmAfDeployment.UpdateUser = SecurityUtils.GetLogInEmpId();
        _freeSql.Update<BpmAfDeployment>().SetSource(bpmAfDeployment).ExecuteAffrows();
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_INSERT_AFTER_CURRENT_NODE);
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_INSERT_AFTER_FUTURE_NODE);
    }
}
