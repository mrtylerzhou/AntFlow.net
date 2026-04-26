using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmVerifyInfoService : IBpmVerifyInfoService
{
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly AFTaskService _afTaskService;
    private readonly IProcessConstantsService _processConstantsService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmVariableSignUpService _bpmVariableSignUpService;
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
    private readonly IBpmnNodeService _nodeService;

    public BpmVerifyInfoService(
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        AFTaskService afTaskService,
        IProcessConstantsService processConstantsService,
        IBpmVariableService bpmVariableService,
        IBpmVariableSignUpService bpmVariableSignUpService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService,
        IBpmnNodeService nodeService,
        IBpmVerifyInfoRepository repository
    )
    {
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _afTaskService = afTaskService;
        _processConstantsService = processConstantsService;
        _bpmVariableService = bpmVariableService;
        _bpmVariableSignUpService = bpmVariableSignUpService;
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
        _nodeService = nodeService;
        _repository = repository;
    }

    public IBpmVerifyInfoRepository _repository { get; }

    public void AddVerifyInfo(BpmVerifyInfo verifyInfo)
    {
        BpmFlowrunEntrust entrustByTaskId = _bpmFlowrunEntrustService.GetEntrustByTaskId(SecurityUtils.GetLogInEmpIdStr(), verifyInfo.RunInfoId, verifyInfo.TaskDefKey);
        if (entrustByTaskId != null)
        {
            verifyInfo.OriginalId = entrustByTaskId.Original;
        }
        _repository.Add(verifyInfo);
    }

    public string FindCurrentNodeIds(string processNumber)
    {
        // 查询业务流程信息
        BpmBusinessProcess bpmBusinessProcess =
            _bpmBusinessProcessService._repository.FirstOrDefault(a => a.BusinessNumber == processNumber);

        if (bpmBusinessProcess == null)
        {
            return string.Empty;
        }

        // 获取 act_ru_task 表的 PROC_INST_ID_
        string procInstId = bpmBusinessProcess.ProcInstId;

        List<BpmVerifyInfoVo> tasks = FindTaskInfo(procInstId);
        if (tasks.Count == 0)
        {
            return string.Empty;
        }

        string elementId = tasks[0].ElementId;
        var bpmnNodeIds = _bpmVariableService.GetNodeIdsByeElementId(processNumber, elementId);

        if (bpmnNodeIds == null || bpmnNodeIds.Count == 0)
        {
            BpmAfTaskInst prevTask = _processConstantsService.GetPrevTask(elementId, procInstId);
            if (prevTask != null)
            {
                string taskDefinitionKey = prevTask.TaskDefKey;
                bpmnNodeIds = _bpmVariableSignUpService
                    .GetSignUpPrevNodeIdsByeElementId(processNumber, taskDefinitionKey);
            }
        }

        if (bpmnNodeIds == null || bpmnNodeIds.Count == 0)
        {
            return string.Empty;
        }

        List<BpmnNode> bpmnNodes = _nodeService._repository.Find(a => bpmnNodeIds.Contains(a.Id.ToString()));
        if (bpmnNodes == null || bpmnNodes.Count == 0)
        {
            return string.Empty;
        }

        var nodeCollect = bpmnNodes.Select(node => node.NodeId).ToList();
        return string.Join(",", nodeCollect);
    }

    public List<BpmVerifyInfoVo> FindTaskInfo(String procInstId)
    {
        List<BpmVerifyInfoVo> currentTasks = _afTaskService
            ._repository.Find(a => a.ProcInstId == procInstId)
            .OrderByDescending(a => a.CreateTime)
            .Select(t => new BpmVerifyInfoVo()
            {
                Id = t.Id,
                TaskName = t.Name,
                VerifyUserId = t.Assignee,
                VerifyUserName = t.AssigneeName,
                VerifyStatusName = "处理中",
                ElementId = t.TaskDefKey,
                VerifyDesc = "",
                VerifyDate = null,
            }
        ).ToList();
        return currentTasks;
    }

    public List<BpmVerifyInfoVo> FindTaskInfo(BpmBusinessProcess bpmBusinessProcess)
    {
        string procInstId = bpmBusinessProcess.ProcInstId;
        List<BpmVerifyInfoVo> tasks = this.FindTaskInfo(procInstId);

        if (!tasks.Any())
        {
            return new List<BpmVerifyInfoVo>();
        }

        List<string> verifyUserIds = tasks.Select(t => t.VerifyUserId).ToList();
        int? isOutSideProcess = bpmBusinessProcess.IsOutSideProcess;
        Dictionary<string, string> stringStringMap = null;

        if (isOutSideProcess == 1)
        {
            stringStringMap = tasks.ToDictionary(t => t.VerifyUserId, t => t.VerifyUserName, StringComparer.Ordinal);
        }
        else
        {
            stringStringMap = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(verifyUserIds);
        }

        foreach (var task in tasks)
        {
            if (stringStringMap.ContainsKey(task.VerifyUserId))
            {
                task.VerifyUserName = stringStringMap[task.VerifyUserId];
            }
        }

        List<BpmVerifyInfoVo> taskInfors = new List<BpmVerifyInfoVo>();

        if (tasks.Count > 1)
        {
            string verifyUserName = string.Join(",", tasks.Select(t => t.VerifyUserName));
            string taskName = string.Empty;
            List<string> strs = tasks.Select(t => t.TaskName).Where(t => t != null).Distinct().ToList();

            if (strs.Count > 1)
            {
                taskName = string.Join("||", strs);
            }
            else
            {
                taskName = strs.FirstOrDefault();
            }

            string elementId = string.Empty;
            List<string> elementIdList = tasks.Select(t => t.ElementId).Distinct().ToList();
            if (elementIdList.Count > 1)
            {
                elementId = string.Join(",", elementIdList);
            }
            else
            {
                elementId = elementIdList.FirstOrDefault() ?? string.Empty;
            }

            taskInfors.Add(new BpmVerifyInfoVo
            {
                Id = procInstId,
                VerifyUserIds = verifyUserIds,
                VerifyUserName = verifyUserName,
                TaskName = taskName,
                ElementId = elementId
            });
        }
        else
        {
            tasks[0].VerifyUserIds = verifyUserIds;
            taskInfors.Add(tasks[0]);
        }

        return taskInfors;
    }

    public List<BpmVerifyInfoVo> VerifyInfoList(String processNumber, String procInstId)
    {
        BpmVerifyInfoVo vo = new BpmVerifyInfoVo()
        {
            ProcessCode = processNumber
        };
        List<BpmVerifyInfoVo> bpmVerifyInfoVos = _repository.GetVerifyInfo(vo);
        return GetBpmVerifyInfoVoList(bpmVerifyInfoVos, procInstId);
    }

    public List<BpmVerifyInfoVo> GetBpmVerifyInfoVoList(List<BpmVerifyInfoVo> list, string procInstId)
    {
        var infoVoList = new List<BpmVerifyInfoVo>();

        infoVoList.AddRange(list.Select(o =>
        {
            if (!string.IsNullOrEmpty(o.OriginalId))
            {
                if (!string.IsNullOrEmpty(procInstId))
                {
                    List<BpmFlowrunEntrust> bpmFlowrunEntrusts = _bpmFlowrunEntrustService
                        ._repository
                        .Find(a => a.Original == o.OriginalId && a.RunInfoId == o.RunInfoId);

                    if (bpmFlowrunEntrusts != null && bpmFlowrunEntrusts.Any())
                    {
                        o.OriginalName = bpmFlowrunEntrusts[0].OriginalName;
                        o.VerifyUserName = $"{o.VerifyUserName} 代 {o.OriginalName} 审批";
                    }
                }
                else
                {
                    var employeeInfo = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(new List<string> { o.OriginalId });
                    if (employeeInfo.TryGetValue(o.OriginalId, out var originalName))
                    {
                        o.OriginalName = originalName;
                        o.VerifyUserName = $"{o.VerifyUserName} 代 {o.OriginalName} 审批";
                    }
                }
            }
            return o;
        }).ToList());

        return infoVoList;
    }
}
