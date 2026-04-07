using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Common.util.Extension;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.Engine.service.biz;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class AddAssigneeProcessService: IProcessOperationAdaptor
{
    private readonly IAFTaskService _afTaskService;
    private readonly IAFDeploymentService _deploymentService;
    private readonly IAFExecutionService _executionService;
    private readonly IBpmFlowrunEntrustService _flowrunEntrustService;
    private readonly IBpmvariableBizService _bpmvariableBizService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;

    public AddAssigneeProcessService(IAFTaskService afTaskService,
        IAFDeploymentService deploymentService,
        IAFExecutionService executionService,
        IBpmFlowrunEntrustService flowrunEntrustService,
        IBpmvariableBizService bpmvariableBizService,
        IBpmBusinessProcessService bpmBusinessProcessService)
    {
        _afTaskService = afTaskService;
        _deploymentService = deploymentService;
        _executionService = executionService;
        _flowrunEntrustService = flowrunEntrustService;
        _bpmvariableBizService = bpmvariableBizService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
    }
    public void DoProcessButton(BusinessDataVo vo)
    {
        String processNumber = vo.ProcessNumber;
        String taskDefKey = vo.TaskDefKey;
        List<BaseIdTranStruVo> userInfos = vo.UserInfos;
        if(userInfos.IsEmpty()){
            throw new AFBizException("请选择要加签的人员");
        }
        if(userInfos.Count>1){
            throw new AFBizException("每次加能加签1人");
        }
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if(bpmBusinessProcess==null){
            throw new AFBizException("未能根据流程编号找到流程实例:"+processNumber);
        }
        String procInstId = bpmBusinessProcess.ProcInstId;
        List<BpmAfTask> tasks = _afTaskService
            .baseRepo
            .Where(a => a.ProcInstId == procInstId)
            .ToList();
        if (tasks.IsEmpty())
        {
            throw new AFBizException($"未能根据流程实例id:{procInstId}和任务节点key:{taskDefKey}找到当前审批任务");
        }

        List<BpmnConfCommonElementVo> elements = _deploymentService.GetDeploymentByProcessNumber(processNumber);
        if (elements.IsEmpty())
        {
            throw new AFBizException($"未能根据流程编号找到流程定义!{processNumber}");
        }
        BpmnConfCommonElementVo currentElement = BpmnFlowUtil.GetCurrentTaskElement(elements,taskDefKey);
        if (currentElement == null)
        {
            throw new AFBizException($"can not get current element by element id: {currentElement}");
        }

        int currentElementSignType = currentElement.SignType;
        if ((int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER == currentElementSignType)
        {
            NodeElementDto nodeIdByElementId = _bpmvariableBizService.GetNodeIdByElementId(processNumber,taskDefKey);
            _bpmvariableBizService.AddNodeAssignees(processNumber,taskDefKey,userInfos);
            _deploymentService.UpdateNodeAssignee(processNumber,userInfos,nodeIdByElementId.NodeId,1);
            return;
        }
        List<string> currentList = tasks.Select(a=>a.Assignee).ToList();
        BpmAfExecution afExecution = _executionService.baseRepo.Where(a=>a.Id==tasks[0].ExecutionId).First();
        if (afExecution == null)
        {
            throw new AFBizException(BusinessError.STATUS_ERROR,$"未能根据流程实例id:{procInstId}和任务节点key:{taskDefKey}找到当前审批任务的执行实例");
        }
        foreach (BaseIdTranStruVo userinfo in userInfos)
        {
            if (currentList.Contains(userinfo.Id))
            {
                throw new AFBizException("不可重复添加已存在的操作人!");
            }

            
            BpmAfTask bpmAfTask = BuildTask(bpmBusinessProcess,tasks[0],afExecution,userinfo);
            _afTaskService.baseRepo.Insert(bpmAfTask);
            _flowrunEntrustService.AddFlowrunEntrust(userinfo.Id,userinfo.Name,"0","管理员加签",taskDefKey,0,
                bpmBusinessProcess.ProcInstId,bpmBusinessProcess.ProcessinessKey,vo.NodeId,2);
            _bpmvariableBizService.AddNodeAssignees(processNumber,taskDefKey,userInfos);
        }
       
        afExecution.TaskCount += userInfos.Count;
        _executionService.baseRepo.Update(afExecution);
        
    }

    private BpmAfExecution BuildExecution(BpmBusinessProcess bpmBusinessProcess,BpmAfTask task,int? signType)
    {
        BpmAfExecution execution = new BpmAfExecution
        {
            Id = StrongUuidGenerator.GetNextId(),
            ProcInstId = bpmBusinessProcess.ProcInstId,
            BusinessKey =bpmBusinessProcess.ProcessinessKey,
            ProcDefId = task.ProcDefId,
            ActId = task.TaskDefKey,
            Name = task.Name,
            StartTime = DateTime.Now,
            StartUserId = SecurityUtils.GetLogInEmpId(),
            TaskCount = 1,
            SignType = signType
        };
        return execution;
    }

    private BpmAfTask BuildTask(BpmBusinessProcess bpmBusinessProcess,BpmAfTask existingTask, BpmAfExecution execution,BaseIdTranStruVo assignee)
    {
        BpmAfTask bpmAfTask = new BpmAfTask()
        {
            Id = StrongUuidGenerator.GetNextId(),
            ProcInstId = bpmBusinessProcess.ProcInstId,
            ProcDefId = existingTask.ProcDefId,
            ExecutionId = execution.Id,
            Name = existingTask.Name,
            TaskDefKey = existingTask.TaskDefKey,
            Owner = bpmBusinessProcess.CreateUser,
            Assignee = assignee.Id,
            AssigneeName = assignee.Name,
            CreateTime = DateTime.Now,
            FormKey = bpmBusinessProcess.ProcessinessKey,
        };
        return bpmAfTask;
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_ADD_ASSIGNEE);
    }
}