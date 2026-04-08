using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.exception;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class AddFutureAssigneeProcessService : AbstractAddOrRemoveFutureAssigneeSerivce, IProcessOperationAdaptor
{
    public AddFutureAssigneeProcessService(IBpmBusinessProcessService bpmBusinessProcessService, TaskService taskService, 
        IAfTaskInstService afTaskInstService, IBpmFlowrunEntrustService bpmFlowrunEntrustService, IAFDeploymentService afDeploymentService, IBpmvariableBizService bpmvariableBizService) : base(bpmBusinessProcessService, taskService, afTaskInstService, bpmFlowrunEntrustService, afDeploymentService, bpmvariableBizService)
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