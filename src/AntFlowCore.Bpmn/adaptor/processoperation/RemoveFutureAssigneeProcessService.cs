using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class RemoveFutureAssigneeProcessService: AbstractAddOrRemoveFutureAssigneeSerivce, IProcessOperationAdaptor
{
    public RemoveFutureAssigneeProcessService(IBpmBusinessProcessService bpmBusinessProcessService, TaskService taskService,
        IAfTaskInstService afTaskInstService, IBpmFlowrunEntrustService bpmFlowrunEntrustService, 
        IAFDeploymentService afDeploymentService, IBpmvariableBizService bpmvariableBizService) : base(bpmBusinessProcessService, taskService, afTaskInstService, bpmFlowrunEntrustService, afDeploymentService, bpmvariableBizService)
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