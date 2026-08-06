using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
/// Oppose (反对) operation for arbitration sign nodes (signType=4).
///
/// Flow:
/// 1. Look up current task by taskId.
/// 2. Delete the current approver's task (DeletTask) — does NOT touch
///    Activiti's nrOfCompletedInstances, so the multi-instance loop keeps
///    waiting for the remaining approvers.
/// 3. Record a verifyinfo row with verify_status=7 (oppose).
/// 4. Count oppose records (verify_status=7) for this process + taskDefKey.
/// 5. Compute oppose threshold M = ceil(n * (100 - ratio) / 100):
///    - n = number of valid BpmVariableMultiplayerPersonnel records for the node
///    - ratio = ArbitrationRatio retrieved from deployment content
///    - If ratio cannot be read, default to 100 (M=0, any oppose terminates).
/// 6. If oppose count >= M, call EndProcessService.EndProcessWithoutVerify
///    to terminate the process.
///
/// Mirrors Java OpposeProcessImpl.
/// </summary>
public class OpposeProcessService : IProcessOperationAdaptor
{
    /// <summary>verify_status value for oppose actions. 7 = WITHDRAW_DISAGREE_TYPE,
    /// reused for oppose (5 is occupied by 'cancellation/作废').</summary>
    private const int VERIFY_STATUS_OPPOSE = 7;

    private readonly IAFTaskService _taskService;
    private readonly ITaskMgmtService _taskMgmtService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmVerifyInfoService _verifyInfoService;
    private readonly IBpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly IAFDeploymentService _deploymentService;
    private readonly EndProcessService _endProcessService;
    private readonly ILogger<OpposeProcessService> _logger;

    public OpposeProcessService(
        IAFTaskService taskService,
        ITaskMgmtService taskMgmtService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmVerifyInfoService verifyInfoService,
        IBpmVariableMultiplayerService bpmVariableMultiplayerService,
        IBpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
        IAFDeploymentService deploymentService,
        EndProcessService endProcessService,
        ILogger<OpposeProcessService> logger)
    {
        _taskService = taskService;
        _taskMgmtService = taskMgmtService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _verifyInfoService = verifyInfoService;
        _bpmVariableMultiplayerService = bpmVariableMultiplayerService;
        _bpmVariableMultiplayerPersonnelService = bpmVariableMultiplayerPersonnelService;
        _deploymentService = deploymentService;
        _endProcessService = endProcessService;
        _logger = logger;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        if (string.IsNullOrEmpty(vo.TaskId))
        {
            throw new AFBizException("当前任务ID为空,无法执行反对操作");
        }

        BpmAfTask task = _taskService._repository.FirstOrDefault(a => a.Id == vo.TaskId);
        if (task == null)
        {
            throw new AFBizException("当前任务不存在或已被处理");
        }

        string processNumber = vo.ProcessNumber;
        string taskDefKey = task.TaskDefKey;
        string procInstId = task.ProcInstId;

        // Resolve verify user (support outside access proc)
        string verifyUserName;
        string verifyUserId;
        if (vo.IsOutSideAccessProc != null && vo.IsOutSideAccessProc.Value
            && vo.ObjectMap != null && vo.ObjectMap.Any())
        {
            verifyUserName = vo.ObjectMap.ContainsKey("employeeName") ? vo.ObjectMap["employeeName"].ToString() : string.Empty;
            verifyUserId = vo.ObjectMap.ContainsKey("employeeId") ? vo.ObjectMap["employeeId"].ToString() : string.Empty;
        }
        else
        {
            verifyUserName = SecurityUtils.GetLogInEmpName();
            verifyUserId = SecurityUtils.GetLogInEmpIdStr();
        }

        // 1. Delete current approver's task (does NOT affect nrOfCompletedInstances)
        _taskMgmtService.DeletTask(vo.TaskId);
        _logger.LogInformation(
            "Oppose: deleted task {TaskId} (taskDefKey={TaskDefKey}) in process {ProcessNumber}",
            vo.TaskId, taskDefKey, processNumber);

        // 2. Record verifyinfo with verify_status=7 (oppose)
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        string businessId = bpmBusinessProcess?.BusinessId;
        _verifyInfoService.AddVerifyInfo(new BpmVerifyInfo
        {
            BusinessId = businessId,
            VerifyUserId = verifyUserId,
            VerifyUserName = verifyUserName,
            VerifyStatus = VERIFY_STATUS_OPPOSE,
            VerifyDate = DateTime.Now,
            ProcessCode = processNumber,
            VerifyDesc = vo.ApprovalComment,
            TaskName = task.Name,
            TaskId = vo.TaskId,
            TaskDefKey = taskDefKey,
            RunInfoId = procInstId,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        });

        // 3. Count oppose records for this process + taskDefKey
        List<BpmVerifyInfo> opposeRecords = _verifyInfoService._repository.Find(v =>
            v.ProcessCode == processNumber
            && v.TaskDefKey == taskDefKey
            && v.VerifyStatus == VERIFY_STATUS_OPPOSE
            && v.IsDel == 0);
        int opposeCount = opposeRecords.Count;

        // 4. Compute oppose threshold M = ceil(n * (100 - ratio) / 100)
        int n = GetPersonnelCount(processNumber, taskDefKey);
        int ratio = GetArbitrationRatio(processNumber, taskDefKey);
        int M = (int)Math.Ceiling(n * (100 - ratio) / 100.0);

        _logger.LogInformation(
            "Oppose tally: process={ProcessNumber}, node={TaskDefKey}, opposeCount={OpposeCount}, n={N}, ratio={Ratio}, M={M}",
            processNumber, taskDefKey, opposeCount, n, ratio, M);

        // 5. If oppose count >= M, terminate the process
        if (opposeCount >= M)
        {
            _logger.LogInformation(
                "Oppose threshold reached (opposeCount={OpposeCount} >= M={M}), terminating process {ProcessNumber}",
                opposeCount, M, processNumber);
            _endProcessService.EndProcessWithoutVerify(vo);
        }
    }

    /// <summary>
    /// Count valid personnel records (IsDel=0) for the arbitration node
    /// identified by (processNumber, elementId=taskDefKey).
    /// </summary>
    private int GetPersonnelCount(string processNumber, string elementId)
    {
        // BpmVariableMultiplayerPersonnel has no IsDel column exposed on the entity,
        // but BpmVariableMultiplayer does. Query multiplayer rows first, then sum personnel.
        List<BpmVariableMultiplayer> multiplayerRows =
            _bpmVariableMultiplayerService._repository.Find(m =>
                m.ElementId == elementId && m.IsDel == 0);

        if (multiplayerRows == null || multiplayerRows.Count == 0)
        {
            // Fallback: try QueryMultiplayersByProcessNumAndElementId which joins BpmVariable
            multiplayerRows = _bpmVariableMultiplayerService
                ._repository
                .QueryMultiplayersByProcessNumAndElementId(processNumber, elementId);
            if (multiplayerRows == null || multiplayerRows.Count == 0)
            {
                _logger.LogWarning(
                    "Oppose: no BpmVariableMultiplayer found for process={ProcessNumber}, elementId={ElementId}. Defaulting n=0.",
                    processNumber, elementId);
                return 0;
            }
        }

        // Sum personnel across all multiplayer rows (typically just one row per element)
        int n = 0;
        foreach (var mp in multiplayerRows)
        {
            List<BpmVariableMultiplayerPersonnel> personnel =
                _bpmVariableMultiplayerPersonnelService._repository.Find(p =>
                    p.VariableMultiplayerId == mp.Id);
            n += personnel?.Count ?? 0;
        }
        return n;
    }

    /// <summary>
    /// Retrieve arbitrationRatio from the deployment content (List<BpmnConfCommonElementVo>)
    /// by matching the elementId (== taskDefKey). Returns 100 if not found or unreadable,
    /// which means M=0 and any single oppose will terminate the process (safest fallback).
    /// </summary>
    private int GetArbitrationRatio(string processNumber, string elementId)
    {
        try
        {
            List<BpmnConfCommonElementVo> elements = _deploymentService.GetDeploymentByProcessNumber(processNumber);
            if (elements == null || elements.Count == 0)
            {
                _logger.LogWarning(
                    "Oppose: deployment content empty for process={ProcessNumber}. Defaulting ratio=100.",
                    processNumber);
                return 100;
            }

            BpmnConfCommonElementVo element = elements.FirstOrDefault(e => e.ElementId == elementId);
            if (element == null)
            {
                _logger.LogWarning(
                    "Oppose: element {ElementId} not found in deployment for process={ProcessNumber}. Defaulting ratio=100.",
                    elementId, processNumber);
                return 100;
            }

            if (element.ArbitrationRatio == null || element.ArbitrationRatio <= 0)
            {
                _logger.LogWarning(
                    "Oppose: ArbitrationRatio missing on element {ElementId}. Defaulting ratio=100.",
                    elementId);
                return 100;
            }

            return element.ArbitrationRatio.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Oppose: failed to read ArbitrationRatio for process={ProcessNumber}, elementId={ElementId}. Defaulting ratio=100.",
                processNumber, elementId);
            return 100;
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_OPPOSE);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker, ProcessOperationEnum.BUTTON_TYPE_OPPOSE);
    }
}