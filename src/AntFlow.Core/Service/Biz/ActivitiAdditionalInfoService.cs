using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json;

namespace AntFlow.Core.Service.Business;

public class ActivitiAdditionalInfoService
{
    private readonly AFDeploymentService _afDeploymentService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly ILogger<ActivitiAdditionalInfoService> _logger;

    public ActivitiAdditionalInfoService(AFDeploymentService afDeploymentService,
        AfTaskInstService afTaskInstService,
        ILogger<ActivitiAdditionalInfoService> logger)
    {
        _afDeploymentService = afDeploymentService;
        _afTaskInstService = afTaskInstService;
        _logger = logger;
    }

    public List<BpmnConfCommonElementVo> GetActivitiList(BpmAfTaskInst historicProcessInstance)
    {
        return GetActivitiList(historicProcessInstance.ProcDefId);
    }

    public List<BpmnConfCommonElementVo> GetActivitiList(string procDefId)
    {
        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a => a.Id == procDefId).First();
        if (bpmAfDeployment == null)
        {
            throw new ApplicationException($"deployment with id {procDefId} not found");
        }

        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        return elements;
    }

    public Dictionary<string, List<BpmAfTaskInst>> GetVariableInstanceMap(string procInstId)
    {
        List<BpmAfTaskInst> bpmAfTaskInsts =
            _afTaskInstService.baseRepo.Where(a => a.ProcInstId == procInstId).ToList();


        Dictionary<string, List<BpmAfTaskInst>>? variableInstanceMap = new();

        foreach (BpmAfTaskInst? variableInstance in bpmAfTaskInsts)
        {
            if (!variableInstanceMap.ContainsKey(variableInstance.Assignee))
            {
                variableInstanceMap[variableInstance.Assignee] = new List<BpmAfTaskInst>();
            }

            variableInstanceMap[variableInstance.Assignee].Add(variableInstance);
        }

        return variableInstanceMap;
    }

    public static List<BpmnConfCommonElementVo> GetNextElementList(string elementId,
        List<BpmnConfCommonElementVo> activitiList)
    {
        List<BpmnConfCommonElementVo> bpmnConfCommonElementVos =
            activitiList.Where(a => a.ElementId == elementId).ToList();
        if (bpmnConfCommonElementVos.Count == 0)
        {
            return null;
        }

        BpmnConfCommonElementVo bpmnConfCommonElementVo =
            BpmnFlowUtil.GetNodeFromCurrentNext(activitiList, bpmnConfCommonElementVos[0].ElementId);
        return bpmnConfCommonElementVo == null ? null : new List<BpmnConfCommonElementVo> { bpmnConfCommonElementVo };
    }
}