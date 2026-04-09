using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
/// 删除未来节点实现
/// 思路：修改流程定义，把节点的审批人全减掉，使其跳过该节点
/// </summary>
public class RemoveFutureNodeProcessService : IProcessOperationAdaptor
{
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmvariableBizService _bpmvariableBizService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IAFDeploymentService _afDeploymentService;
    private readonly IAfTaskInstService _afTaskInstService;

    public RemoveFutureNodeProcessService(
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmvariableBizService bpmvariableBizService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IAFDeploymentService afDeploymentService,
        IAfTaskInstService afTaskInstService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmvariableBizService = bpmvariableBizService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _afDeploymentService = afDeploymentService;
        _afTaskInstService = afTaskInstService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        string processNumber = vo.ProcessNumber;
        string nodeId = vo.NodeId;

        if (string.IsNullOrEmpty(processNumber))
        {
            throw new AFBizException("流程编号不能为空");
        }
        if (string.IsNullOrEmpty(nodeId))
        {
            throw new AFBizException("节点id不能为空");
        }

        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException("未能根据流程编号找到流程信息:" + processNumber);
        }

        // 根据nodeId获取节点信息
        NodeElementDto nodeElementDto = _bpmvariableBizService.GetElementIdByNodeId(processNumber, nodeId);
        if (nodeElementDto == null)
        {
            throw new AFBizException("未能根据节点id获取元素Id:" + nodeId);
        }

        string procInstId = bpmBusinessProcess.ProcInstId;
        string elementId = nodeElementDto.ElementId;
        bool isSingle = nodeElementDto.IsSingle;

        // 检查节点是否已经执行过或正在执行
        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService.baseRepo
            .Where(a => a.ProcInstId == procInstId && a.TaskDefKey == elementId)
            .ToList();
        if (bpmAfTaskInsts.Count > 0)
        {
            throw new AFBizException("当前节点已经执行或正在执行,无法删除!");
        }

        // 获取节点的所有审批人
        List<BaseInfoTranStructVo> assigneeInfoList = nodeElementDto.AssigneeInfoList;
        if (assigneeInfoList == null || assigneeInfoList.Count == 0)
        {
            throw new AFBizException("该节点没有审批人,无需删除!");
        }

        // 获取所有审批人ID
        List<string> assigneeIds = assigneeInfoList.Select(a => a.Id).ToList();

        // 标记删除节点所有审批人
        _bpmvariableBizService.InvalidNodeAssignees(assigneeIds, processNumber, isSingle);

        // 修改流程定义为"跳过"（传入空列表，actionType=2表示删除）
        _afDeploymentService.UpdateNodeAssignee(processNumber, assigneeInfoList.Select(a=>(BaseIdTranStruVo)a).ToList(), nodeId, 2);

        // 记录操作日志
        foreach (BaseInfoTranStructVo assignee in assigneeInfoList)
        {
            _bpmFlowrunEntrustService.AddFlowrunEntrust(
                assignee.Id,
                assignee.Name,
                "0",
                "管理员删除未来节点",
                elementId,
                1,
                procInstId,
                bpmBusinessProcess.ProcessinessKey,
                nodeId,
                3
            );
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_REMOVE_FUTURE_NODE);
    }
}
