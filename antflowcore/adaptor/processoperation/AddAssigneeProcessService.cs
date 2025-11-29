using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class AddAssigneeProcessService: IProcessOperationAdaptor
{
    private readonly AFTaskService _afTaskService;
    private readonly AFDeploymentService _deploymentService;
    private readonly AFExecutionService _executionService;
    private readonly BpmFlowrunEntrustService _flowrunEntrustService;
    private readonly BpmvariableBizService _bpmvariableBizService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;

    public AddAssigneeProcessService(AFTaskService afTaskService,
        AFDeploymentService deploymentService,
        AFExecutionService executionService,
        BpmFlowrunEntrustService flowrunEntrustService,
        BpmvariableBizService bpmvariableBizService,
        BpmBusinessProcessService bpmBusinessProcessService)
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
            .Where(a => a.ProcInstId == procInstId && a.TaskDefKey == taskDefKey)
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
            throw new AFBizException("顺序会签节点禁止加签!");
        }
        List<string> currentList = tasks.Select(a=>a.Assignee).ToList();
        foreach (BaseIdTranStruVo userinfo in userInfos)
        {
            if (currentList.Contains(userinfo.Id))
            {
                throw new AFBizException("不可重复添加已存在的操作人!");
            }

            BpmAfExecution bpmAfExecution = BuildExecution(bpmBusinessProcess, tasks[0], currentElementSignType);
            BpmAfTask bpmAfTask = BuildTask(bpmBusinessProcess,tasks[0],bpmAfExecution,userinfo);
            _executionService.baseRepo.Insert(bpmAfExecution);
            _afTaskService.baseRepo.Insert(bpmAfTask);
            _flowrunEntrustService.AddFlowrunEntrust(userinfo.Id,userinfo.Name,"0","管理员加签",taskDefKey,0,
                bpmBusinessProcess.ProcInstId,bpmBusinessProcess.ProcessinessKey,vo.NodeId,2);
            _bpmvariableBizService.AddNodeAssignees(processNumber,taskDefKey,userInfos);
        }

        
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