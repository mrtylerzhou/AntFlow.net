using antflowcore.bpmn.service;
using AntFlowCore.Constants;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

 public class TransferAssigneeProcessService : IProcessOperationAdaptor
    {
        private readonly BpmBusinessProcessService _bpmBusinessProcessService;
        private readonly AFTaskService _taskService;
        private readonly TaskMgmtService _taskMgmtService;
        private readonly BpmvariableBizService _bpmvariableBizService;
        private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;

        public TransferAssigneeProcessService(
            BpmBusinessProcessService bpmBusinessProcessService,
            AFTaskService taskService,
            TaskMgmtService taskMgmtService,
            BpmvariableBizService bpmvariableBizService,
            BpmFlowrunEntrustService bpmFlowrunEntrustService)
        {
            _bpmBusinessProcessService = bpmBusinessProcessService;
            _taskService = taskService;
            _taskMgmtService = taskMgmtService;
            _bpmvariableBizService = bpmvariableBizService;
            _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        }

        public void DoProcessButton(BusinessDataVo vo)
        {
            BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
            List<BpmAfTask> tasks = _taskService
                .baseRepo
                .Where(a=>a.ProcInstId==bpmBusinessProcess.ProcInstId)
                .ToList();

            var userInfos = vo.UserInfos;
            if (userInfos.Count != 2)
            {
                throw new AFBizException("转办人员配置错误,无法转办!");
            }

            var originalUserId = userInfos[0].Id;
            var originalUserName = userInfos[0].Name;
            var transferToUserId = userInfos[1].Id;
            var transferToUserName = userInfos[1].Name;

            var assignees = tasks.Select(task => task.Assignee).ToList();
            var originAssigneeIndex = assignees.IndexOf(originalUserId);

            if (originAssigneeIndex < 0)
            {
                throw new AFBizException("流程状态已变更,无当前办理人信息,转办失败!");
            }

            var matched = false;

            foreach (var task in tasks)
            {
                if (task.Assignee == originalUserId)
                {
                    NodeElementDto nodeIdByElementId = _bpmvariableBizService.GetNodeIdByElementId(vo.ProcessNumber,task.TaskDefKey);
                    _bpmFlowrunEntrustService.AddFlowrunEntrust(transferToUserId, transferToUserName, originalUserId, 
                        originalUserName, task.TaskDefKey, 0, bpmBusinessProcess.ProcInstId, vo.ProcessKey,nodeIdByElementId.NodeId,0);

                    var taskMgmtVO = new TaskMgmtVO
                    {
                        ApplyUser = transferToUserId,
                        ApplyUserName = transferToUserName,
                        TaskId = task.Id
                    };
                    
                    _taskMgmtService.UpdateTaskInst(taskMgmtVO);
                    _taskMgmtService.UpdateTask(taskMgmtVO);

                    matched = true;
                }
            }

            if (!matched)
            {
                throw new AFBizException("流程状态已变更,无当前办理人信息,转办失败!");
            }
        }

        public void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_ZB);
            ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker, ProcessOperationEnum.BUTTON_TYPE_ZB);
        }
    }