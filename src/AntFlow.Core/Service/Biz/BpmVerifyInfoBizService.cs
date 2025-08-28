using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;
using System.Text;
using System.Text.Json;

namespace AntFlow.Core.Service.Business;

public class BpmVerifyInfoBizService
{
    private readonly ActivitiAdditionalInfoService _actitiAdditionalInfoService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly BpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmVariableSignUpService _bpmVariableSignUpService;
    private readonly BpmVariableSingleService _bpmVariableSingleService;
    private readonly BpmVerifyInfoService _bpmVerifyInfoService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProviderService;
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

    public string FindCurrentNodeIds(string processNumber)
    {
        return _bpmVerifyInfoService.FindCurrentNodeIds(processNumber);
    }

    public List<BpmVerifyInfoVo> GetBpmVerifyInfoVos(string processNumber, bool finishFlag)
    {
        List<BpmVerifyInfoVo> bpmVerifyInfoVos = new();

        // ?????????????
        BpmBusinessProcess bpmBusinessProcess =
            _bpmBusinessProcessService.baseRepo.Where(a => a.BusinessNumber == processNumber).First();

        finishFlag = (int)ProcessStateEnum.HANDLE_STATE == bpmBusinessProcess.ProcessState;
        if (bpmBusinessProcess == null)
        {
            return bpmVerifyInfoVos;
        }

        // ????????????
        bpmVerifyInfoVos.Add(new BpmVerifyInfoVo
        {
            TaskName = "????",
            VerifyStatus = 1,
            VerifyUserIds = new List<string> { bpmBusinessProcess.CreateUser },
            VerifyUserName = bpmBusinessProcess.UserName,
            VerifyDate = bpmBusinessProcess.CreateTime,
            VerifyStatusName = "??"
        });

        // ??????????????
        List<BpmVerifyInfoVo>? searchBpmVerifyInfoVos = _bpmVerifyInfoService
            .VerifyInfoList(processNumber, bpmBusinessProcess.ProcInstId)
            .OrderBy(v => v.VerifyDate)
            .ToList();
        bpmVerifyInfoVos.AddRange(searchBpmVerifyInfoVos);

        // ??????????????
        BpmAfTaskInst historicProcessInstance = _afTaskInstService
            .baseRepo
            .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
            .First();


        // ????????????????
        BpmAfTaskInst lastHistoricTaskInstance = _afTaskInstService
            .baseRepo
            .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
            .OrderByDescending(a => a.EndTime)
            .First();


        int sort = 0;
        bool noApproval = false;
        List<BpmVerifyInfoVo>? bpmVerifyInfoSortVos = new();

        foreach (BpmVerifyInfoVo? bpmVerifyInfoVo in bpmVerifyInfoVos)
        {
            if (bpmVerifyInfoVo.VerifyStatus == 3 || bpmVerifyInfoVo.VerifyStatus == 6)
            {
                bpmVerifyInfoVo.TaskName = lastHistoricTaskInstance?.Name;
                bpmVerifyInfoVo.VerifyStatusName = "???????";
                noApproval = true; //????????????????????
            }

            if (bpmVerifyInfoVo.VerifyStatus == 5)
            {
                string? lastAssignee = lastHistoricTaskInstance?.Assignee;
                string? lastAssigneeName = lastHistoricTaskInstance?.AssigneeName;
                string json = JsonSerializer.Serialize(bpmVerifyInfoVo);
                BpmVerifyInfoVo vo = JsonSerializer.Deserialize<BpmVerifyInfoVo>(json);
                vo.TaskName = "??????";
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
                    Dictionary<string, string>? provideEmployeeInfo =
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

        // ??????????????
        List<BpmVerifyInfoVo>? taskInfo = _bpmVerifyInfoService.FindTaskInfo(bpmBusinessProcess);
        BpmVerifyInfoVo taskVo;

        if (taskInfo.Any() && !finishFlag)
        {
            taskVo = taskInfo.First();
            taskVo.Sort = sort++;
            taskVo.VerifyStatus = 99;
            taskVo.VerifyStatusName = "??????";
            bpmVerifyInfoVos.Add(taskVo);

            List<BpmFlowrunEntrust> flowrunEntrustList = _bpmFlowrunEntrustService
                .baseRepo
                .Where(a => a.RunInfoId == taskVo.Id).ToList();


            if (flowrunEntrustList.Any())
            {
                BpmFlowrunEntrust? flowrunEntrust = flowrunEntrustList.First();
                if (taskVo.VerifyUserId == flowrunEntrust.Actual)
                {
                    taskVo.VerifyUserName = $"{taskVo.VerifyUserName} ?? {flowrunEntrust.OriginalName} ????";
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
                        {
                            lastHistoricTaskInstance?.Assignee
                        })
                        .GetValueOrDefault(lastHistoricTaskInstance?.Assignee)
                };
                bpmVerifyInfoVos.Add(taskVo);
                sort++;
            }
        }
        else
        {
            taskVo = new BpmVerifyInfoVo { ElementId = lastHistoricTaskInstance?.TaskDefKey };
        }

        int processState = bpmBusinessProcess.ProcessState;
        int endVerifyStatus = 100;

        if (processState != (int)ProcessStateEnum.REJECT_STATE || processState != (int)ProcessStateEnum.END_STATE)
        {
            if (!finishFlag)
            {
                if (!noApproval)
                {
                    AddBpmVerifyInfoVo(processNumber, sort, bpmVerifyInfoVos, historicProcessInstance, taskVo);
                }
            }

            if (processState == (int)ProcessStateEnum.HANDLING_STATE)
            {
                endVerifyStatus = 0;
            }
        }

        bpmVerifyInfoVos.Add(new BpmVerifyInfoVo { TaskName = "???????", VerifyStatus = endVerifyStatus });
        bpmVerifyInfoVos = bpmVerifyInfoVos.Where(a => a.NodeType != (int)NodeTypeEnum.NODE_TYPE_COPY).ToList();
        return bpmVerifyInfoVos;
    }

    private void AddBpmVerifyInfoVo(string processNumber, int sort, List<BpmVerifyInfoVo> bpmVerifyInfoVos,
        BpmAfTaskInst historicProcessInstance, BpmVerifyInfoVo taskVo)
    {
        // Get all Activiti flow nodes list
        List<BpmnConfCommonElementVo> activitiList =
            _actitiAdditionalInfoService.GetActivitiList(historicProcessInstance);

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
        Dictionary<string, List<BaseIdTranStruVo>>? nodeApproveds = GetNodeApproveds(bpmVariable.Id);

        // Find the activity by elementId
        BpmnConfCommonElementVo? activiti = activitiList.FirstOrDefault(a => a.ElementId == taskVo.ElementId);
        //todo multicharacter

        // Get signup node's element id and collection name
        Dictionary<string, string>? signUpNodeCollectionNameMap = GetSignUpNodeCollectionNameMap(bpmVariable.Id);


        Dictionary<string, List<BpmAfTaskInst>> variableInstanceMap =
            _actitiAdditionalInfoService.GetVariableInstanceMap(historicProcessInstance.Id);

        string[] elementIds = taskVo.ElementId.Split(",");
        for (int i = 0; i < elementIds.Length; i++)
        {
            // Perform the final append operation
            DoAddBpmVerifyInfoVo(sort, elementIds[i], activitiList, nodeApproveds,
                bpmVerifyInfoVos, bpmVariable.Id, i == elementIds.Length - 1);
        }
    }

    private Dictionary<string, List<BaseIdTranStruVo>> GetNodeApproveds(long variableId)
    {
        Dictionary<string, List<BaseIdTranStruVo>>? nodeApprovedsMap = new();

        // ???????????????
        List<BpmVariableSingle> variableSingles =
            _bpmVariableSingleService.baseRepo.Where(a => a.VariableId == variableId).ToList();
        if (variableSingles.Count > 0)
        {
            foreach (BpmVariableSingle? bpmVariableSingle in variableSingles)
            {
                nodeApprovedsMap[bpmVariableSingle.ElementId] = new List<BaseIdTranStruVo>
                {
                    new() { Id = bpmVariableSingle.Assignee, Name = bpmVariableSingle.AssigneeName }
                };
            }
        }

        // ???????????????
        List<BpmVariableMultiplayer> variableMultiplayers =
            _bpmVariableMultiplayerService.baseRepo.Where(a => a.VariableId == variableId).ToList();
        if (variableMultiplayers.Count > 0)
        {
            foreach (BpmVariableMultiplayer? bpmVariableMultiplayer in variableMultiplayers)
            {
                List<BpmVariableMultiplayerPersonnel> bpmVariableMultiplayerPersonnels =
                    _bpmVariableMultiplayerPersonnelService
                        .baseRepo.Where(a => a.VariableMultiplayerId == bpmVariableMultiplayer.VariableId)
                        .ToList();


                if (bpmVariableMultiplayerPersonnels.Any())
                {
                    nodeApprovedsMap[bpmVariableMultiplayer.ElementId] = bpmVariableMultiplayerPersonnels
                        .Select(a => new BaseIdTranStruVo { Id = a.Assignee, Name = a.AssigneeName }).ToList();
                }
            }
        }

        return nodeApprovedsMap;
    }

    private void DoAddBpmVerifyInfoVo(int sort, string elementId, List<BpmnConfCommonElementVo> activitiList,
        Dictionary<string, List<BaseIdTranStruVo>> nodeApproveds,
        List<BpmVerifyInfoVo> bpmVerifyInfoVos, long variableId,
        bool includeParallelGateway)
    {
        List<BpmnConfCommonElementVo> nextElements =
            ActivitiAdditionalInfoService.GetNextElementList(elementId, activitiList);

        if (nextElements == null || !nextElements.Any())
        {
            return;
        }

        BpmnConfCommonElementVo bpmnConfCommonElementVo = nextElements[0];
        bool isParallelGateway =
            bpmnConfCommonElementVo.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code;
        if (!includeParallelGateway && isParallelGateway)
        {
            return;
        }

        List<string>? empIds = new();
        List<string>? emplNames = new();

        foreach (BpmnConfCommonElementVo? nextElement in nextElements)
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

        // ????????????
        string verifyUserName = emplNames.Any()
            ? string.Join(",", emplNames)
            : StringConstants.DYNAMIC_APPROVER;

        StringBuilder? nameSb = new();
        StringBuilder? elementIdSb = new();

        for (int i = 0; i < nextElements.Count; i++)
        {
            BpmnConfCommonElementVo? currElement = nextElements[i];

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

        BpmVerifyInfoVo bpmVerifyInfoVo = new()
        {
            ElementId = elementIdSb.ToString(),
            TaskName = nameSb.ToString(),
            VerifyDesc = string.Empty,
            VerifyStatus = 0,
            VerifyUserIds = empIds,
            VerifyUserName = verifyUserName,
            NodeType = bpmnConfCommonElementVo.NodeType,
            Sort = sort
        };

        // ????????????§Ò?
        if (!isParallelGateway && !string.IsNullOrEmpty(bpmVerifyInfoVo.VerifyUserName) &&
            bpmVerifyInfoVo.TaskName != "EndEvent")
        {
            bpmVerifyInfoVos.Add(bpmVerifyInfoVo);
            sort++;
        }

        foreach (BpmnConfCommonElementVo? nextElement in nextElements)
        {
            // ??ø„??????????
            List<BpmnConfCommonElementVo>? nextNextElement =
                ActivitiAdditionalInfoService.GetNextElementList(nextElement.ElementId, activitiList);
            if (nextNextElement != null)
            {
                if (!includeParallelGateway && isParallelGateway)
                {
                    break;
                }

                DoAddBpmVerifyInfoVo(sort, nextElement.ElementId, activitiList, nodeApproveds, bpmVerifyInfoVos,
                    variableId, includeParallelGateway);
            }
        }
    }

    public Dictionary<string, string> GetSignUpNodeCollectionNameMap(long variableId)
    {
        Dictionary<string, string>? signUpNodeCollectionNameMap = new();

        List<BpmVariableSignUp> bpmVariableSignUps = _bpmVariableSignUpService
            .baseRepo
            .Where(a => a.VariableId == variableId)
            .ToList();


        foreach (BpmVariableSignUp? variableSignUp in bpmVariableSignUps)
        {
            if (!string.IsNullOrEmpty(variableSignUp.SubElements))
            {
                List<BpmnConfCommonElementVo>? bpmnConfCommonElementVos =
                    JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(variableSignUp.SubElements);
                if (bpmnConfCommonElementVos != null && bpmnConfCommonElementVos.Any())
                {
                    foreach (BpmnConfCommonElementVo? bpmnConfCommonElementVo in bpmnConfCommonElementVos)
                    {
                        signUpNodeCollectionNameMap[bpmnConfCommonElementVo.ElementId] =
                            bpmnConfCommonElementVo.CollectionName;
                    }
                }
            }
        }

        return signUpNodeCollectionNameMap;
    }
}