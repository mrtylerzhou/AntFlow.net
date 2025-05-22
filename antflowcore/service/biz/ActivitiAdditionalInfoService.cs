using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using antflowcore.entity;
using antflowcore.service.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

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
    public List<BpmnConfCommonElementVo> GetActivitiList(BpmAfTaskInst historicProcessInstance) {

        
        return GetActivitiList(historicProcessInstance.ProcDefId);

    }
    public List<BpmnConfCommonElementVo> GetActivitiList(String procDefId){
        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a=>a.Id==procDefId).First();
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
        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService.baseRepo.Where(a => a.ProcInstId == procInstId).ToList();
        

        var variableInstanceMap = new Dictionary<string, List<BpmAfTaskInst>>();

        foreach (var variableInstance in bpmAfTaskInsts)
        {
            if (!variableInstanceMap.ContainsKey(variableInstance.Assignee))
            {
                variableInstanceMap[variableInstance.Assignee] = new List<BpmAfTaskInst>();
            }
            variableInstanceMap[variableInstance.Assignee].Add(variableInstance);
        }

        return variableInstanceMap;
    }

    public static List<BpmnConfCommonElementVo> GetNextElementList(string elementId, List<BpmnConfCommonElementVo> activitiList)
    {
        IEnumerable<BpmnConfCommonElementVo> bpmnConfCommonElementVos = activitiList.Where(a=>a.ElementId==elementId);
        List<BpmnConfCommonElementVo> flowToSequences = activitiList.Where(a=>elementId.Equals(a.FlowFrom)).ToList();
        List<string> flowTos = flowToSequences.Select(a=>a.FlowTo).ToList();
        if (!flowTos.Any())
        {
            return null;
        }

        List<BpmnConfCommonElementVo> nextElementVos = activitiList.Where(a=>flowTos.Contains(a.ElementId)).ToList();
        return nextElementVos;
    }
}