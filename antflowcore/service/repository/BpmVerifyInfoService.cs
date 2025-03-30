using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.biz;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class BpmVerifyInfoService: AFBaseCurdRepositoryService<BpmVerifyInfo>
{
    private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly AFTaskService _afTaskService;
    private readonly ProcessConstantsService _processConstantsService;
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmVariableSignUpService _bpmVariableSignUpService;
    private readonly BpmnNodeService _nodeService;

    public BpmVerifyInfoService(
        BpmFlowrunEntrustService bpmFlowrunEntrustService,
        BpmBusinessProcessService bpmBusinessProcessService,
        AFTaskService afTaskService,
        ProcessConstantsService processConstantsService,
        BpmVariableService bpmVariableService,
        BpmVariableSignUpService bpmVariableSignUpService,
        BpmnNodeService nodeService,
        IFreeSql freeSql
    ) : base(freeSql)
    {
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _afTaskService = afTaskService;
        _processConstantsService = processConstantsService;
        _bpmVariableService = bpmVariableService;
        _bpmVariableSignUpService = bpmVariableSignUpService;
        _nodeService = nodeService;
    }

    public void AddVerifyInfo(BpmVerifyInfo verifyInfo)
    {
        BpmFlowrunEntrust entrustByTaskId = _bpmFlowrunEntrustService.GetEntrustByTaskId(SecurityUtils.GetLogInEmpIdStr(), verifyInfo.RunInfoId, verifyInfo.TaskId);
    }

    public string FindCurrentNodeIds(string processNumber)
    {
        // 查询业务流程信息
        BpmBusinessProcess bpmBusinessProcess =
            _bpmBusinessProcessService.baseRepo.Where(a => a.BusinessNumber == processNumber).First();
        
        if (bpmBusinessProcess == null)
        {
            return string.Empty;
        }

        // 获取 act_ru_task 表的 PROC_INST_ID_
        string procInstId = bpmBusinessProcess.ProcInstId;

        var tasks = FindTaskInfo(procInstId) ?? new List<BpmVerifyInfoVo>();
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


        List<BpmnNode> bpmnNodes = _nodeService.baseRepo.Where(a => bpmnNodeIds.Contains(a.Id.ToString())).ToList();
        if (bpmnNodes == null || bpmnNodes.Count == 0)
        {
            return string.Empty;
        }

        var nodeCollect = bpmnNodes.Select(node => node.NodeId).ToList();
        return string.Join(",", nodeCollect);
    }

    public List<BpmVerifyInfoVo> FindTaskInfo(String procInstId)
    {
        List<BpmVerifyInfoVo> bpmVerifyInfos = _afTaskService.baseRepo.Where(a => a.ProcInstId == procInstId).ToList().Select(t => new BpmVerifyInfoVo()
            {
                Id = t.Id,
                TaskName = t.Name,
                VerifyUserId = t.Assignee,
                VerifyUserName = t.AssigneeName,
                VerifyStatusName = "处理中" ,
                ElementId = t.TaskDefKey,
                VerifyDesc = "",
                VerifyDate = null,
            }
        ).ToList();
        return bpmVerifyInfos;
    }
}