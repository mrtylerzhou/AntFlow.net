using antflowcore.bpmn.service;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class RemoveFutureAssigneeProcessService: AbstractAddOrRemoveFutureAssigneeSerivce, IProcessOperationAdaptor
{
    public RemoveFutureAssigneeProcessService(BpmBusinessProcessService bpmBusinessProcessService, TaskService taskService, AfTaskInstService afTaskInstService, BpmFlowrunEntrustService bpmFlowrunEntrustService, AFDeploymentService afDeploymentService, BpmvariableBizService bpmvariableBizService) : base(bpmBusinessProcessService, taskService, afTaskInstService, bpmFlowrunEntrustService, afDeploymentService, bpmvariableBizService)
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
        List<BaseIdTranStruVo> userInfos = vo.UserInfos;
        if (nodeElementDto.IsSingle)
        {
            userInfos = new List<BaseIdTranStruVo>()
            {
                new BaseIdTranStruVo
                {
                    Id = "0",
                    Name = "跳过",
                }
            };
        }
       
        base.ModifyFutureAssigneesByProcessInstance(bpmBusinessProcess,vo,nodeElementDto,userInfos,2);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_REMOVE_FUTURE_ASSIGNEE);
    }
}