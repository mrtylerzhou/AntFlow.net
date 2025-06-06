using System.Text;
using System.Text.Json;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class BpmVerifyInfoBizService
{
    private readonly BpmVerifyInfoService _bpmVerifyInfoService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProviderService;
    private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly ActivitiAdditionalInfoService _actitiAdditionalInfoService;
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmVariableSignUpService _bpmVariableSignUpService;
    private readonly BpmVariableSingleService _bpmVariableSingleService;
    private readonly BpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly ILogger<BpmVerifyInfoBizService> _logger;

    public BpmVerifyInfoBizService(BpmVerifyInfoService bpmVerifyInfoService,
        BpmBusinessProcessService bpmBusinessProcessService,
        AfTaskInstService afTaskInstService,
        IBpmnEmployeeInfoProviderService employeeInfoProviderService,
        BpmFlowrunEntrustService bpmFlowrunEntrustService,
        ActivitiAdditionalInfoService actitiAdditionalInfoService,
        BpmVariableService bpmVariableService,
        BpmVariableSignUpService bpmVariableSignUpService,
        BpmVariableSingleService bpmVariableSingleService,
        BpmVariableMultiplayerService bpmVariableMultiplayerService,
        BpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
        ILogger<BpmVerifyInfoBizService> logger)
    {
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _afTaskInstService = afTaskInstService;
        _employeeInfoProviderService = employeeInfoProviderService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _actitiAdditionalInfoService = actitiAdditionalInfoService;
        _bpmVariableService = bpmVariableService;
        _bpmVariableSignUpService = bpmVariableSignUpService;
        _bpmVariableSingleService = bpmVariableSingleService;
        _bpmVariableMultiplayerService = bpmVariableMultiplayerService;
        _bpmVariableMultiplayerPersonnelService = bpmVariableMultiplayerPersonnelService;

        _logger = logger;
    }

    public String FindCurrentNodeIds(String processNumber)
    {
        return _bpmVerifyInfoService.FindCurrentNodeIds(processNumber);
    }

    public List<BpmVerifyInfoVo> GetBpmVerifyInfoVos(string processNumber, bool finishFlag)
    {
        List<BpmVerifyInfoVo> bpmVerifyInfoVos = new List<BpmVerifyInfoVo>();

        // 查询业务流程信息
        BpmBusinessProcess bpmBusinessProcess =
            _bpmBusinessProcessService.baseRepo.Where(a => a.BusinessNumber == processNumber).First();

        finishFlag = (int)ProcessStateEnum.HANDLE_STATE == bpmBusinessProcess.ProcessState;
        if (bpmBusinessProcess == null)
        {
            return bpmVerifyInfoVos;
        }

        // 添加流程开始节点
        bpmVerifyInfoVos.Add(new BpmVerifyInfoVo
        {
            TaskName = "发起",
            VerifyStatus = 1,
            VerifyUserIds = new List<string> { bpmBusinessProcess.CreateUser },
            VerifyUserName = bpmBusinessProcess.UserName,
            VerifyDate = bpmBusinessProcess.CreateTime,
            VerifyStatusName = "提交"
        });

        // 查询并追加流程记录
        var searchBpmVerifyInfoVos = _bpmVerifyInfoService.VerifyInfoList(processNumber, bpmBusinessProcess.ProcInstId)
            .OrderBy(v => v.VerifyDate)
            .ToList();
        bpmVerifyInfoVos.AddRange(searchBpmVerifyInfoVos);

        // 查询流程的历史实例
        BpmAfTaskInst historicProcessInstance = _afTaskInstService
            .baseRepo
            .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
            .First();


        // 获取最后一个审批记录
        BpmAfTaskInst lastHistoricTaskInstance = _afTaskInstService
            .baseRepo
            .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
            .OrderByDescending(a => a.EndTime)
            .First();


        int sort = 0;
        var bpmVerifyInfoSortVos = new List<BpmVerifyInfoVo>();

        foreach (var bpmVerifyInfoVo in bpmVerifyInfoVos)
        {
            if (bpmVerifyInfoVo.VerifyStatus == 3 || bpmVerifyInfoVo.VerifyStatus == 6)
            {
                bpmVerifyInfoVo.TaskName = lastHistoricTaskInstance?.Name;
                bpmVerifyInfoVo.VerifyStatusName = "审批拒绝";
            }

            if (bpmVerifyInfoVo.VerifyStatus == 5)
            {
                var lastAssignee = lastHistoricTaskInstance?.Assignee;
                var lastAssigneeName = lastHistoricTaskInstance?.AssigneeName;
                string json = JsonSerializer.Serialize(bpmVerifyInfoVo);
                BpmVerifyInfoVo vo = JsonSerializer.Deserialize<BpmVerifyInfoVo>(json);
                vo.TaskName = "发起人";
                vo.VerifyUserIds = new List<string> { bpmBusinessProcess.CreateUser };
                vo.VerifyUserName = bpmBusinessProcess.UserName;
                vo.Sort = sort++;
                bpmVerifyInfoSortVos.Add(vo);

                bpmVerifyInfoVo.TaskName = lastHistoricTaskInstance?.Name;
                bpmVerifyInfoVo.VerifyUserId = lastAssignee;

                if (!string.IsNullOrEmpty(lastAssigneeName))
                {
                    bpmVerifyInfoVo.VerifyUserName = lastAssigneeName;
                }
                else
                {
                    var provideEmployeeInfo =
                        _employeeInfoProviderService.ProvideEmployeeInfo(new List<string> { lastAssignee });
                    bpmVerifyInfoVo.VerifyUserName = provideEmployeeInfo.GetValueOrDefault(lastAssignee);
                }

                bpmVerifyInfoVo.VerifyDate = null;
                bpmVerifyInfoVo.VerifyDesc = string.Empty;
                bpmVerifyInfoVo.VerifyStatus = 0;
                bpmVerifyInfoVo.VerifyStatusName = string.Empty;
            }

            bpmVerifyInfoVo.Sort = sort++;
            bpmVerifyInfoSortVos.Add(bpmVerifyInfoVo);
        }

        bpmVerifyInfoVos = bpmVerifyInfoSortVos;

        // 查询待办任务信息
        var taskInfo = _bpmVerifyInfoService.FindTaskInfo(bpmBusinessProcess);
        BpmVerifyInfoVo taskVo;

        if (taskInfo.Any() && !finishFlag)
        {
            taskVo = taskInfo.First();
            taskVo.Sort = sort++;
            taskVo.VerifyStatus = 99;
            taskVo.VerifyStatusName = "处理中";
            bpmVerifyInfoVos.Add(taskVo);

            List<BpmFlowrunEntrust> flowrunEntrustList = _bpmFlowrunEntrustService
                .baseRepo
                .Where(a => a.RunInfoId == taskVo.Id).ToList();


            if (flowrunEntrustList.Any())
            {
                var flowrunEntrust = flowrunEntrustList.First();
                if (taskVo.VerifyUserId == flowrunEntrust.Actual)
                {
                    taskVo.VerifyUserName = $"{taskVo.VerifyUserName} 代 {flowrunEntrust.OriginalName} 审批";
                }
            }

            if (taskVo.ElementId == ProcessNodeEnum.START_TASK_KEY.Description)
            {
                taskVo = new BpmVerifyInfoVo
                {
                    ElementId = lastHistoricTaskInstance?.TaskDefKey,
                    TaskName = lastHistoricTaskInstance?.Name,
                    VerifyStatus = 0,
                    VerifyUserId = lastHistoricTaskInstance?.Assignee,
                    VerifyUserName = _employeeInfoProviderService.ProvideEmployeeInfo(new List<string>
                            { lastHistoricTaskInstance?.Assignee })
                        .GetValueOrDefault(lastHistoricTaskInstance?.Assignee)
                };
                bpmVerifyInfoVos.Add(taskVo);
                sort++;
            }
        }
        else
        {
            taskVo = new BpmVerifyInfoVo
            {
                ElementId = lastHistoricTaskInstance?.TaskDefKey
            };
        }

        int processState = bpmBusinessProcess.ProcessState;
        int endVerifyStatus = 100;

        if (processState != (int)ProcessStateEnum.REJECT_STATE || processState != (int)ProcessStateEnum.END_STATE)
        {
            if (!finishFlag)
            {
                AddBpmVerifyInfoVo(processNumber, sort, bpmVerifyInfoVos, historicProcessInstance, taskVo);
            }

            if (processState == (int)ProcessStateEnum.HANDLING_STATE)
            {
                endVerifyStatus = 0;
            }
        }

        bpmVerifyInfoVos.Add(new BpmVerifyInfoVo
        {
            TaskName = "流程结束",
            VerifyStatus = endVerifyStatus
        });

        return bpmVerifyInfoVos;
    }

    private void AddBpmVerifyInfoVo(string processNumber, int sort, List<BpmVerifyInfoVo> bpmVerifyInfoVos,
        BpmAfTaskInst historicProcessInstance, BpmVerifyInfoVo taskVo)
    {
        // Get all Activiti flow nodes list
        List<BpmnConfCommonElementVo> activitiList = _actitiAdditionalInfoService.GetActivitiList(historicProcessInstance);

        // Query process variable info
        BpmVariable bpmVariable = _bpmVariableService
            .baseRepo
            .Where(a => a.ProcessNum == processNumber)
            .First();

        if (bpmVariable == null)
        {
            return;
        }

        // Get approvers (Node Approveds)
        var nodeApproveds = GetNodeApproveds(bpmVariable.Id);

        // Find the activity by elementId
        var activiti = activitiList.FirstOrDefault(a => a.ElementId == taskVo.ElementId);
        //todo multicharacter

        // Get signup node's element id and collection name
        var signUpNodeCollectionNameMap = GetSignUpNodeCollectionNameMap(bpmVariable.Id);

       
        Dictionary<string, List<BpmAfTaskInst>> variableInstanceMap = _actitiAdditionalInfoService.GetVariableInstanceMap(historicProcessInstance.Id);

        // Perform the final append operation
        DoAddBpmVerifyInfoVo(sort, taskVo.ElementId, activitiList, nodeApproveds,
            bpmVerifyInfoVos, bpmVariable.Id);
    }

    private Dictionary<string, List<BaseIdTranStruVo>> GetNodeApproveds(long variableId)
    {
        var nodeApprovedsMap = new Dictionary<string, List<BaseIdTranStruVo>>();

        // 查询单人审批变量
        List<BpmVariableSingle> variableSingles = _bpmVariableSingleService.baseRepo.Where(a=>a.VariableId == variableId).ToList();
        if (variableSingles.Count >0)
        {
            
            foreach (var bpmVariableSingle in variableSingles)
            {
                nodeApprovedsMap[bpmVariableSingle.ElementId] = new List<BaseIdTranStruVo>
                {
                    new BaseIdTranStruVo
                    {
                        Id = bpmVariableSingle.Assignee,
                        Name = bpmVariableSingle.AssigneeName
                    }
                };
            }
        }

        // 查询多人审批变量
        List<BpmVariableMultiplayer> variableMultiplayers = _bpmVariableMultiplayerService.baseRepo.Where(a=>a.VariableId==variableId).ToList();
        if (variableMultiplayers.Count>0)
        {
            foreach (var bpmVariableMultiplayer in variableMultiplayers)
            {
                List<BpmVariableMultiplayerPersonnel> bpmVariableMultiplayerPersonnels = _bpmVariableMultiplayerPersonnelService
                    .baseRepo.Where(a=>a.VariableMultiplayerId ==bpmVariableMultiplayer.VariableId)
                    .ToList();
                

                if (bpmVariableMultiplayerPersonnels.Any())
                {
                    nodeApprovedsMap[bpmVariableMultiplayer.ElementId] = bpmVariableMultiplayerPersonnels
                        .Select(a => new BaseIdTranStruVo
                        {
                            Id = a.Assignee,
                            Name = a.AssigneeName
                        }).ToList();
                }
            }
        }

        return nodeApprovedsMap;
    }

    private void DoAddBpmVerifyInfoVo(int sort, string elementId, List<BpmnConfCommonElementVo> activitiList,
            Dictionary<string, List<BaseIdTranStruVo>> nodeApproveds,
            List<BpmVerifyInfoVo> bpmVerifyInfoVos, long variableId)
    {
        List<BpmnConfCommonElementVo> nextElements =
            ActivitiAdditionalInfoService.GetNextElementList(elementId, activitiList);

        if (nextElements == null || !nextElements.Any())
        {
            return;
        }

        var empIds = new List<string>();
        var emplNames = new List<string>();

        foreach (var nextElement in nextElements)
        {
            IDictionary<string, string> nextElementAssigneeMap = nextElement.AssigneeMap;
            if (nextElementAssigneeMap == null)
            {
                continue;
            }

            foreach (KeyValuePair<string, string> keyValuePair in nextElementAssigneeMap)
            {
                empIds.Add(keyValuePair.Key);
                emplNames.Add(keyValuePair.Value);
            }
        }

        // 组装审批人信息
        string verifyUserName = emplNames.Any()
            ? string.Join(",", emplNames)
            : StringConstants.DYNAMIC_APPROVER;

        var nameSb = new StringBuilder();
        var elementIdSb = new StringBuilder();

        for (int i = 0; i < nextElements.Count; i++)
        {
            var currElement = nextElements[i];

            if (i != nextElements.Count - 1)
            {
                nameSb.Append(currElement.ElementName).Append("||");
                elementIdSb.Append(currElement.ElementId).Append(",");
            }
            else
            {
                nameSb.Append(currElement.ElementName);
                elementIdSb.Append(currElement.ElementId);
            }
        }

        var bpmVerifyInfoVo = new BpmVerifyInfoVo
        {
            ElementId = elementIdSb.ToString(),
            TaskName = nameSb.ToString(),
            VerifyDesc = string.Empty,
            VerifyStatus = 0,
            VerifyUserIds = empIds,
            VerifyUserName = verifyUserName,
            Sort = sort
        };

        // 添加到审批信息列表
        if (!string.IsNullOrEmpty(bpmVerifyInfoVo.VerifyUserName) && bpmVerifyInfoVo.TaskName != "EndEvent")
        {
            bpmVerifyInfoVos.Add(bpmVerifyInfoVo);
            sort++;
        }

        foreach (var nextElement in nextElements)
        {
            // 递归处理下一个节点
            var nextNextElement = ActivitiAdditionalInfoService.GetNextElementList(nextElement.ElementId, activitiList);
            if (nextNextElement != null)
            {
                DoAddBpmVerifyInfoVo(sort, nextElement.ElementId, activitiList, nodeApproveds, bpmVerifyInfoVos,
                    variableId);
            }
        }
    }

    public Dictionary<string, string> GetSignUpNodeCollectionNameMap(long variableId)
    {
        var signUpNodeCollectionNameMap = new Dictionary<string, string>();

        List<BpmVariableSignUp> bpmVariableSignUps = _bpmVariableSignUpService
            .baseRepo
            .Where(a=>a.VariableId==variableId)
            .ToList();
            
        

        foreach (var variableSignUp in bpmVariableSignUps)
        {
            if (!string.IsNullOrEmpty(variableSignUp.SubElements))
            {
                var bpmnConfCommonElementVos = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(variableSignUp.SubElements);
                if (bpmnConfCommonElementVos != null && bpmnConfCommonElementVos.Any())
                {
                    foreach (var bpmnConfCommonElementVo in bpmnConfCommonElementVos)
                    {
                        signUpNodeCollectionNameMap[bpmnConfCommonElementVo.ElementId] = bpmnConfCommonElementVo.CollectionName;
                    }
                }
            }
        }

        return signUpNodeCollectionNameMap;
    }

}