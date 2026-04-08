using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Common.util;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.exception;
using AntFlowCore.Core.extension;
using AntFlowCore.Core.interf;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public abstract class AbstractAddOrRemoveFutureAssigneeSerivce
{
    protected readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    protected readonly ITaskService _taskService;
    protected readonly IAfTaskInstService _afTaskInstService;
    protected readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    protected readonly IAFDeploymentService _afDeploymentService;
    protected readonly IBpmvariableBizService _bpmvariableBizService;


    public AbstractAddOrRemoveFutureAssigneeSerivce(IBpmBusinessProcessService bpmBusinessProcessService,
        TaskService taskService,
        IAfTaskInstService afTaskInstService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IAFDeploymentService afDeploymentService,
        IBpmvariableBizService bpmvariableBizService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
        _afTaskInstService = afTaskInstService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _afDeploymentService = afDeploymentService;
        _bpmvariableBizService = bpmvariableBizService;
    }
    protected void checkParam(BusinessDataVo vo){
        String processNumber = vo.ProcessNumber;
        List<BaseIdTranStruVo> userInfos = vo.UserInfos;
        String nodeId = vo.NodeId;
        if(string.IsNullOrEmpty(processNumber)){
            throw new AFBizException("流程编号不能为空");
        }
        if (userInfos.IsEmpty()){
            throw new AFBizException("要变更的人员信息不能为空");
        }
        if(string.IsNullOrEmpty(nodeId)){
            throw new AFBizException("节点id不能为空");
        }
       
    }

    protected void ModifyFutureAssigneesByProcessInstance(BpmBusinessProcess bpmBusinessProcess,BusinessDataVo vo, NodeElementDto nodeElementDto, List<BaseIdTranStruVo> userInfos, int actionType)
    {
        String procInstId = bpmBusinessProcess.ProcInstId;
        String processNumber = bpmBusinessProcess.BusinessNumber;
        string nodeId = nodeElementDto.NodeId;
        string elementId = nodeElementDto.ElementId;
        bool isSingle = nodeElementDto.IsSingle;
        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService.baseRepo
            .Where(a=>a.ProcInstId==procInstId&&a.TaskDefKey==nodeElementDto.ElementId)
            .ToList();
        if (bpmAfTaskInsts.Count > 0)
        {
            if (actionType == 1)
            {
                AddAssigneeProcessService addAssigneeProcessService = ServiceProviderUtils.GetService<AddAssigneeProcessService>();
                addAssigneeProcessService.DoProcessButton(vo);
                return;
            }else if (actionType == 2)
            {
                RemoveAssigneeProcessService removeAssigneeProcessService = ServiceProviderUtils.GetService<RemoveAssigneeProcessService>();
                removeAssigneeProcessService.DoProcessButton(vo);
                return;
            }
        }

        List<BaseInfoTranStructVo> currentNodeAssignees = nodeElementDto.AssigneeInfoList;
        List<string> assigneeIds = userInfos.Select(a=>a.Id).ToList();
       
        if (actionType == 1)
        {
            foreach (string assignee in assigneeIds)
            {
                if (currentNodeAssignees.Any(a => a.Id == assignee))
                {
                    throw new AFBizException("不可重复添加已存在的操作人!");
                }
            }
            _bpmvariableBizService.AddNodeAssignees(processNumber,elementId,userInfos);
        }else if (actionType == 2)
        {
           
            /*foreach (BaseIdTranStruVo userInfo in userInfos)
            {
                if (!currentNodeAssignees.Select(a => a.Id).Contains(userInfo.Id))
                {
                    throw new AFBizException("要去除的人员非当前节点人员");
                }
            }*/
            _bpmvariableBizService.InvalidNodeAssignees(assigneeIds,processNumber, isSingle);
        }
        _afDeploymentService.UpdateNodeAssignee(processNumber,userInfos,nodeId,actionType);
        foreach (BaseIdTranStruVo userInfo in userInfos)
        {
           String userId = userInfo.Id;
           String userName = userInfo.Name;
          
           String adminName=actionType==1?"管理员加签":"管理员减签";
           int entrustType = actionType == 1 ? 2 : 3;
           _bpmFlowrunEntrustService.AddFlowrunEntrust(userId,userName,"0",adminName,
               elementId,1,procInstId,bpmBusinessProcess.ProcessinessKey,nodeId,entrustType);
        }
        
    }
}