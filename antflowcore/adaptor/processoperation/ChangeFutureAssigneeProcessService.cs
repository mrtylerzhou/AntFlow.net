using antflowcore.bpmn.service;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class ChangeFutureAssigneeProcessService :  IProcessOperationAdaptor
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmvariableBizService _bpmvariableBizService;
    private readonly BpmFlowrunEntrustService _flowrunEntrustService;
    private readonly AFDeploymentService _deploymentService;

    public ChangeFutureAssigneeProcessService(BpmBusinessProcessService bpmBusinessProcessService,
        BpmvariableBizService bpmvariableBizService,
        BpmFlowrunEntrustService flowrunEntrustService,
        AFDeploymentService deploymentService
        )
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmvariableBizService = bpmvariableBizService;
        _flowrunEntrustService = flowrunEntrustService;
        _deploymentService = deploymentService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        String processNumber = vo.ProcessNumber;
        String nodeId = vo.NodeId;
        List<BaseIdTranStruVo> userInfos = vo.UserInfos;
        if(string.IsNullOrEmpty(processNumber)){
            throw new AFBizException("流程编号不能为空");
        }
        if(string.IsNullOrEmpty(nodeId)){
            throw new AFBizException("节点id不能为空");
        }
        if(userInfos.IsEmpty()){
            throw new AFBizException("审批人不能为空");
        }
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if(null==bpmBusinessProcess){
            throw new AFBizException("未能根据流程编号找到流程信息:"+processNumber);
        }

        NodeElementDto elementIdByNodeId = _bpmvariableBizService.GetElementIdByNodeId(processNumber,nodeId);
        if (elementIdByNodeId == null)
        {
            throw new AFBizException("当前节点变量信息为空!");
        }
        List<BaseInfoTranStructVo> assignees = elementIdByNodeId.AssigneeInfoList;
        if (assignees.IsEmpty())
        {
            throw new AFBizException("当前节点变更审批人信息为空!");
        }
        IDictionary<BaseInfoTranStructVo,BaseIdTranStruVo> changedAssignees=new Dictionary<BaseInfoTranStructVo, BaseIdTranStruVo>();
        for (int i = 0; i < assignees.Count; i++)
        {
            BaseInfoTranStructVo currentAssignee = assignees[i];
            BaseIdTranStruVo mayChangedAssignee = userInfos[i];
            if (currentAssignee.Id == mayChangedAssignee.Id)//如果二者相同,肯定是没发生变化
            {
                continue;
            }
            List<BaseIdTranStruVo> sameAssignees = userInfos.Where(a => a.Id == currentAssignee.Id).ToList();
            //未能获取到前端传入的和当前审批人相同的人员,说明发生了变更
            if(sameAssignees.IsEmpty())
            {
                changedAssignees[currentAssignee] = mayChangedAssignee;
            }
        }
        if(changedAssignees.IsEmpty()){
            throw  new AFBizException("当前审批人未发生变更!勿需操作!");
        }

        List<BaseInfoTranStructVo> changedInfos = new List<BaseInfoTranStructVo>();
        foreach (var (current, changed) in changedAssignees)
        {
            string currentVariableId = current.VariableId;//对于单人节点,即为VariableSignle的id,多人为multiplayerpersonnel的id
            BaseInfoTranStructVo changeInfo = new BaseInfoTranStructVo
            {
                Id = changed.Id,
                Name = changed.Name,
                VariableId = currentVariableId,
            };
            changedInfos.Add(changeInfo);
        }
        _bpmvariableBizService.ChangeVariableAssignees(changedAssignees,elementIdByNodeId.IsSingle);
        foreach (var (old, changed) in changedAssignees)
        {
            _flowrunEntrustService.AddFlowrunEntrust(changed.Id,changed.Name,old.Id,old.Name,
                elementIdByNodeId.ElementId,1,bpmBusinessProcess.ProcInstId,bpmBusinessProcess.ProcessinessKey, nodeId,1);
        }
        _deploymentService.UpdateNodeAssignee(processNumber,userInfos,nodeId,3);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_CHANGE_FUTURE_ASSIGNEE);
    }
}