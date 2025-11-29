using antflowcore.bpmn.service;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class AddFutureAssigneeProcessService : AbstractAddOrRemoveFutureAssigneeSerivce, IProcessOperationAdaptor
{
    public AddFutureAssigneeProcessService(BpmBusinessProcessService bpmBusinessProcessService, TaskService taskService, AfTaskInstService afTaskInstService, BpmFlowrunEntrustService bpmFlowrunEntrustService, AFDeploymentService afDeploymentService, BpmvariableBizService bpmvariableBizService) : base(bpmBusinessProcessService, taskService, afTaskInstService, bpmFlowrunEntrustService, afDeploymentService, bpmvariableBizService)
    {
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        base.checkParam(vo);
        String processNumber=vo.ProcessNumber;
        String nodeId=vo.NodeId;
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if(null==bpmBusinessProcess){
            throw new AFBizException("未能根据流程编号找到流程信息:"+processNumber);
        }
        NodeElementDto nodeElementDto = _bpmvariableBizService.GetElementIdByNodeId(processNumber, nodeId);
        if(nodeElementDto==null){
            throw new AFBizException("未能根据节点id获取元素Id"+nodeId);
        }
        base.ModifyFutureAssigneesByProcessInstance(bpmBusinessProcess,vo,nodeElementDto,vo.UserInfos,1);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_ADD_FUTURE_ASSIGNEE);
    }
}