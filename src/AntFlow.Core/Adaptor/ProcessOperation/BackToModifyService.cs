using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Factory;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.ProcessOperation;

public class BackToModifyService : IProcessOperationAdaptor
{
    private readonly AFExecutionService _afExecutionService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmVariableService _bpmVariableService;
    private readonly FormFactory _formFactory;
    private readonly ProcessBusinessContansService _processConstants;
    private readonly ProcessNodeJumpService _processNodeJump;
    private readonly BpmProcessNodeSubmitService _processNodeSubmitService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly AFTaskService _taskService;

    private readonly BpmVerifyInfoService _verifyInfoService;

    public BackToModifyService(
        BpmBusinessProcessService bpmBusinessProcessService,
        AFTaskService taskService,
        BpmVerifyInfoService verifyInfoService,
        BpmProcessNodeSubmitService processNodeSubmitService,
        ProcessNodeJumpService processNodeJump,
        FormFactory formFactory,
        TaskMgmtService taskMgmtService,
        BpmVariableService bpmVariableService,
        AFExecutionService afExecutionService,
        ProcessBusinessContansService processConstants)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
        _verifyInfoService = verifyInfoService;
        _processNodeSubmitService = processNodeSubmitService;
        _processNodeJump = processNodeJump;
        _formFactory = formFactory;
        _taskMgmtService = taskMgmtService;
        _bpmVariableService = bpmVariableService;
        _afExecutionService = afExecutionService;
        _processConstants = processConstants;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException("未查询到流程信息!");
        }

        string procInstId = bpmBusinessProcess.ProcInstId;

        // 获取当前流程任务列表
        List<BpmAfTask> taskList = _taskService.baseRepo.Where(t => t.ProcInstId == procInstId).ToList();
        if (!taskList.Any())
        {
            throw new AFBizException($"未获取到当前流程信息!, 流程编号: {bpmBusinessProcess.ProcessinessKey}");
        }

        BpmAfTask taskData = taskList.FirstOrDefault(t => SecurityUtils.GetLogInEmpIdStr() == t.Assignee);
        if (taskData == null)
        {
            throw new AFBizException("当前流程已审批！");
        }

        List<BpmAfTask> otherNodeTasks = taskList.Where(a => a.TaskDefKey != taskData.TaskDefKey).ToList();
        string restoreNodeKey, backToNodeKey;

        int backToModifyType = vo.BackToModifyType ?? ProcessDisagreeTypeEnum.THREE_DISAGREE.Code;
        ProcessDisagreeTypeEnum processDisagreeType = ProcessDisagreeTypeEnum.GetByCode(backToModifyType);


        if (ProcessDisagreeTypeEnum.ONE_DISAGREE == processDisagreeType)
        {
            BpmAfTaskInst prevTask = _processConstants.GetPrevTask(taskData.TaskDefKey, procInstId);
            if (prevTask == null)
            {
                throw new AFBizException("无前置节�? 无法回退上一节点!");
            }

            restoreNodeKey = taskData.TaskDefKey;
            backToNodeKey = prevTask.TaskDefKey;
        }
        else if (ProcessDisagreeTypeEnum.TWO_DISAGREE == processDisagreeType)
        {
            restoreNodeKey = ProcessNodeEnum.TOW_TASK_KEY.Description;
            backToNodeKey = ProcessNodeEnum.START_TASK_KEY.Description;
        }
        else if (ProcessDisagreeTypeEnum.THREE_DISAGREE == processDisagreeType)
        {
            restoreNodeKey = taskData.TaskDefKey;
            backToNodeKey = ProcessNodeEnum.START_TASK_KEY.Description;
        }
        else if (ProcessDisagreeTypeEnum.FOUR_DISAGREE == processDisagreeType)
        {
            string elementId = _bpmVariableService.GetElementIdsdByNodeId(vo.ProcessNumber, vo.BackToNodeId)[0];
            backToNodeKey = elementId;
            List<BpmnConfCommonElementVo> elements = BpmnFlowUtil.GetElementVosByDeployId(taskData.ProcDefId);
            (BpmnConfCommonElementVo? assigneeNode, BpmnConfCommonElementVo? flowNode) =
                BpmnFlowUtil.GetNextNodeAndFlowNode(elements, elementId);
            restoreNodeKey = assigneeNode.ElementId;
        }
        else if (ProcessDisagreeTypeEnum.FIVE_DISAGREE == processDisagreeType)
        {
            restoreNodeKey = taskData.TaskDefKey;
            backToNodeKey = _bpmVariableService.GetElementIdsdByNodeId(vo.ProcessNumber, vo.BackToNodeId)[0];
        }
        else
        {
            throw new AFBizException("未支持的打回类型!");
        }


        // 保存审批信息
        _verifyInfoService.AddVerifyInfo(new BpmVerifyInfo
        {
            BusinessId = bpmBusinessProcess.BusinessId,
            VerifyUserName = SecurityUtils.GetLogInEmpName(),
            VerifyUserId = SecurityUtils.GetLogInEmpIdStr(),
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_UPDATE_TYPE,
            VerifyDate = DateTime.Now,
            ProcessCode = bpmBusinessProcess.BusinessNumber,
            RunInfoId = bpmBusinessProcess.ProcInstId,
            VerifyDesc = vo.ApprovalComment,
            TaskName = taskData.Name,
            TaskId = taskData.Id,
            TaskDefKey = taskData.TaskDefKey,
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        });

        if (!string.IsNullOrEmpty(backToNodeKey))
        {
            // 添加回退节点
            _processNodeSubmitService.AddProcessNode(new BpmProcessNodeSubmit
            {
                State = 1,
                NodeKey = restoreNodeKey,
                ProcessInstanceId = taskData.ProcInstId,
                BackType = backToModifyType,
                CreateUser = SecurityUtils.GetLogInEmpIdStr(),
                CreateTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId()
            });
        }

        List<BpmnConfCommonElementVo> elementList = BpmnFlowUtil.GetElementVosByDeployId(taskData.ProcDefId);
        int backToNodeIndex = elementList.FindIndex(t => t.ElementId == backToNodeKey);
        int currentNodeIndex = elementList.FindIndex(t => t.ElementId == taskData.TaskDefKey);
        bool isBackSpanParallelGateWay = false;
        for (int i = 0; i < elementList.Count; i++)
        {
            BpmnConfCommonElementVo bpmnConfCommonElementVo = elementList[i];
            if (bpmnConfCommonElementVo.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code)
            {
                if (i > backToNodeIndex && i <= currentNodeIndex)
                {
                    isBackSpanParallelGateWay = true;
                    break;
                }
            }
        }

        if (!isBackSpanParallelGateWay)
        {
            Dictionary<string, object> varMap = new()
            {
                { StringConstants.TASK_ASSIGNEE_NAME, taskData.AssigneeName },
                { StringConstants.VERIFY_COMMENT, vo.ApprovalComment }
            };
            _processNodeJump.CommitProcess(taskData, varMap, backToNodeKey);
        }
        else
        {
            taskList = taskList.Distinct(new TaskDataEqualityComparer()).ToList();
            // 并行任务回退
            foreach (BpmAfTask task in taskList)
            {
                Dictionary<string, object> varMap = new()
                {
                    { StringConstants.TASK_ASSIGNEE_NAME, task.AssigneeName },
                    { StringConstants.VERIFY_COMMENT, vo.ApprovalComment }
                };
                _processNodeJump.CommitProcess(task, varMap, backToNodeKey);
            }
        }

        //退回以后的任务
        List<BpmAfTask> currentTasks = _taskService.baseRepo.Where(t => t.ProcInstId == procInstId)
            .OrderByDescending(a => a.CreateTime).ToList();
        if (currentTasks.Count > 0)
        {
            BpmAfTask firstStartNode = currentTasks.First();
            List<BpmAfTask> otherNewTasks = currentTasks.Where(t => t.Id != firstStartNode.Id).ToList();
            if (!isBackSpanParallelGateWay)
            {
                //获取到不在同一个节点上的任�?                    otherNewTasks=otherNewTasks.Where(a => !otherNodeTasks.Select(x=>x.TaskDefKey).Contains(a.TaskDefKey)).ToList();
            }

            if (otherNewTasks.Count > 0)
            {
                List<string> otherNewTaskIds = new(otherNewTasks.Count);
                List<string> otherNewExecutionIds = new(otherNewTasks.Count);
                foreach (BpmAfTask otherNewTask in otherNewTasks)
                {
                    otherNewTaskIds.Add(otherNewTask.Id);
                    otherNewExecutionIds.Add(otherNewTask.ExecutionId);
                }

                _taskService.Frsql.Delete<BpmAfTask>()
                    .Where(a => otherNewTaskIds.Contains(a.Id))
                    .ExecuteAffrows();
                _afExecutionService.Frsql.Delete<BpmAfExecution>()
                    .Where(a => otherNewExecutionIds.Contains(a.Id))
                    .ExecuteAffrows();
            }
        }

        vo.BusinessId = bpmBusinessProcess.BusinessId;

        if (!vo.IsOutSideAccessProc.Value)
        {
            IFormOperationAdaptor<BusinessDataVo>? formAdaptor = _formFactory.GetFormAdaptor(vo);
            formAdaptor.OnBackToModifyData(vo);
        }

        // 回退到指定人�?            if (!string.IsNullOrEmpty(vo.BackToEmployeeId))
        {
            bpmBusinessProcess.BackUserId = vo.BackToEmployeeId;
            _bpmBusinessProcessService.baseRepo.Update(bpmBusinessProcess);

            TaskMgmtVO taskMgmtVo = new()
            {
                TaskIds = [taskList[0].Id], ApplyUser = vo.BackToEmployeeId, ApplyUserName = vo.BackToEmployeeName
            };
            _taskMgmtService.UpdateTask(taskMgmtVo);
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY);
    }
}