using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

using System;
using System.Collections.Generic;
using System.Linq;



    public class BackToModifyService : IProcessOperationAdaptor
    {
        private readonly BpmBusinessProcessService _bpmBusinessProcessService;
        private readonly AFTaskService _taskService;
       
        private readonly BpmVerifyInfoService _verifyInfoService;
        private readonly BpmProcessNodeSubmitService _processNodeSubmitService;
        private readonly ProcessNodeJumpService _processNodeJump;
        private readonly FormFactory _formFactory;
        private readonly TaskMgmtService _taskMgmtService;
        private readonly BpmVariableService _bpmVariableService;
        private readonly ProcessBusinessContansService _processConstants;

        public BackToModifyService(
            BpmBusinessProcessService bpmBusinessProcessService,
            AFTaskService taskService,
            BpmVerifyInfoService verifyInfoService,
            BpmProcessNodeSubmitService processNodeSubmitService,
            ProcessNodeJumpService processNodeJump,
            FormFactory formFactory,
            TaskMgmtService taskMgmtService,
            BpmVariableService bpmVariableService,
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
            _processConstants = processConstants;
        }

        public void DoProcessButton(BusinessDataVo vo)
        {
            BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
            if (bpmBusinessProcess == null)
                throw new AFBizException("未查询到流程信息!");

            string procInstId = bpmBusinessProcess.ProcInstId;

            // 获取当前流程任务列表
            List<BpmAfTask> taskList = _taskService.baseRepo.Where(t => t.ProcInstId == procInstId).ToList();
            if (!taskList.Any())
                throw new AFBizException($"未获取到当前流程信息!, 流程编号: {bpmBusinessProcess.ProcessinessKey}");

            BpmAfTask taskData = taskList.FirstOrDefault(t => SecurityUtils.GetLogInEmpIdStr() == t.Assignee);
            if (taskData == null)
                throw new AFBizException("当前流程已审批！");

            string restoreNodeKey, backToNodeKey;

            int backToModifyType = vo.BackToModifyType ?? ProcessDisagreeTypeEnum.THREE_DISAGREE.Code;
            ProcessDisagreeTypeEnum processDisagreeType = ProcessDisagreeTypeEnum.GetByCode(backToModifyType);


                if (ProcessDisagreeTypeEnum.ONE_DISAGREE == processDisagreeType)
                {
                    BpmAfTaskInst prevTask = _processConstants.GetPrevTask(taskData.TaskDefKey, procInstId);
                    if (prevTask == null)
                        throw new AFBizException("无前置节点, 无法回退上一节点!");
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
                else if(ProcessDisagreeTypeEnum.FOUR_DISAGREE == processDisagreeType)
                {
                    String elementId = _bpmVariableService.GetElementIdsdByNodeId(vo.ProcessNumber, vo.BackToNodeId)[0];
                    backToNodeKey = elementId;
                    List<BpmnConfCommonElementVo> elements = BpmnFlowUtil.GetElementVosByDeployId(taskData.ProcDefId);
                    var (assigneeNode, flowNode) = BpmnFlowUtil.GetNextNodeAndFlowNode(elements, elementId);
                    restoreNodeKey = assigneeNode.ElementId;
                }else if (ProcessDisagreeTypeEnum.FIVE_DISAGREE == processDisagreeType)
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
                ProcessCode = bpmBusinessProcess.BusinessNumber,
                RunInfoId = bpmBusinessProcess.ProcInstId,
                VerifyDesc = vo.ApprovalComment,
                TaskName = taskData.Name,
                TaskId = taskData.Id,
                TaskDefKey = taskData.TaskDefKey
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
                    CreateUser = SecurityUtils.GetLogInEmpIdStr()
                });
            }

            List<BpmnConfCommonElementVo> elementList = BpmnFlowUtil.GetElementVosByDeployId(taskData.ProcDefId);
            int backToNodeIndex = elementList.FindIndex(t => t.ElementId == backToNodeKey);
            int currentNodeIndex = elementList.FindIndex(t => t.ElementId == taskData.TaskDefKey);
            bool isBackSpanParallelGateWay = false;
            for (var i = 0; i < elementList.Count; i++)
            {
                BpmnConfCommonElementVo bpmnConfCommonElementVo = elementList[i];
                if(bpmnConfCommonElementVo.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code)
                {
                   if(i>backToNodeIndex && i<=currentNodeIndex)
                   {
                       isBackSpanParallelGateWay = true;
                       break;
                   }
                }
            }

            if (isBackSpanParallelGateWay)
            {
                Dictionary<string, object> varMap = new Dictionary<string, object>
                {
                    { StringConstants.TASK_ASSIGNEE_NAME, taskData.AssigneeName },
                    {StringConstants.VERIFY_COMMENT,vo.ApprovalComment},
                };
                _processNodeJump.CommitProcess(taskData, varMap, backToNodeKey);
            }
            else
            {
                taskList = taskList.Distinct(new TaskDataEqualityComparer()).ToList();
                // 并行任务回退
                foreach (BpmAfTask task in taskList)
                {
               
                    Dictionary<string, object> varMap = new Dictionary<string, object>
                    {
                        { StringConstants.TASK_ASSIGNEE_NAME, task.AssigneeName },
                        {StringConstants.VERIFY_COMMENT,vo.ApprovalComment},
                    };
                    _processNodeJump.CommitProcess(task, varMap, backToNodeKey);
                }
            }
            //退回以后的任务
            List<BpmAfTask> currentTasks = _taskService.baseRepo.Where(t => t.ProcInstId == procInstId).ToList();
            if (currentTasks.Count > 0)
            {
                BpmAfTask firstStartNode = currentTasks.First();
                List<String> otherNewTaskIds = currentTasks.Where(t => t.Id != firstStartNode.Id).Select(t => t.Id).ToList();
                _taskService.baseRepo.Delete(t => otherNewTaskIds.Contains(t.Id));
            }
            vo.BusinessId = bpmBusinessProcess.BusinessId;

            if (!vo.IsOutSideAccessProc.Value)
            {
                var formAdaptor = _formFactory.GetFormAdaptor(vo);
                formAdaptor.OnBackToModifyData(vo);
            }

            // 回退到指定人员
            if (!string.IsNullOrEmpty(vo.BackToEmployeeId))
            {
                bpmBusinessProcess.BackUserId = vo.BackToEmployeeId;
                _bpmBusinessProcessService.baseRepo.Update(bpmBusinessProcess);

                TaskMgmtVO taskMgmtVo = new TaskMgmtVO
                {
                    TaskIds = new List<string> { taskList[0].Id },
                    ApplyUser = vo.BackToEmployeeId,
                    ApplyUserName = vo.BackToEmployeeName,
                };
                _taskMgmtService.UpdateTask(taskMgmtVo);
            }
        }

        public void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY);
            ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker, ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY);
        }

      
    }

