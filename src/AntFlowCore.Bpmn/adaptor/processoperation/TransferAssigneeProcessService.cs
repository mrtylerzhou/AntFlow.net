using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.util;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class TransferAssigneeProcessService : IProcessOperationAdaptor
{
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IAFTaskService _taskService;
    private readonly ITaskMgmtService _taskMgmtService;
    private readonly IBpmvariableBizService _bpmvariableBizService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IBpmVerifyInfoService _verifyInfoService;
    public TransferAssigneeProcessService(
            IBpmBusinessProcessService bpmBusinessProcessService,
            IAFTaskService taskService,
            ITaskMgmtService taskMgmtService,
            IBpmvariableBizService bpmvariableBizService,
            IBpmFlowrunEntrustService bpmFlowrunEntrustService,
            IBpmVerifyInfoService verifyInfoService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
        _taskMgmtService = taskMgmtService;
        _bpmvariableBizService = bpmvariableBizService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _verifyInfoService = verifyInfoService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
        List<BpmAfTask> tasks = _taskService
            .baseRepo
            .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
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
                NodeElementDto nodeIdByElementId = _bpmvariableBizService.GetNodeIdByElementId(vo.ProcessNumber, task.TaskDefKey);
                _bpmFlowrunEntrustService.AddFlowrunEntrust(transferToUserId, transferToUserName, originalUserId,
                    originalUserName, task.TaskDefKey, 0, bpmBusinessProcess.ProcInstId, vo.ProcessKey, nodeIdByElementId.NodeId, 0);
                //将转交信息加入流程记录表中
                _verifyInfoService.AddVerifyInfo(new BpmVerifyInfo
                {
                    BusinessId = bpmBusinessProcess.BusinessId,
                    VerifyUserName = originalUserName,
                    VerifyUserId = originalUserId,
                    OriginalId = transferToUserId,
                    VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_TRANSFER_ASSIGNEE,
                    VerifyDate = DateTime.Now,
                    ProcessCode = bpmBusinessProcess.BusinessNumber,
                    RunInfoId = bpmBusinessProcess.ProcInstId,
                    VerifyDesc = vo.ApprovalComment,
                    TaskName = task.Name,
                    TaskId = task.Id.ToString(),
                    TaskDefKey = task.TaskDefKey
                });

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