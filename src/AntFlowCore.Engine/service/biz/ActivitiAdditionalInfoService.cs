using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Bpmn;
using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

public class ActivitiAdditionalInfoService : IActivitiAdditionalInfoService
{
    private readonly IAFDeploymentService _afDeploymentService;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly ILogger<ActivitiAdditionalInfoService> _logger;

    public ActivitiAdditionalInfoService(IAFDeploymentService afDeploymentService,
        IAfTaskInstService afTaskInstService,
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
        List<BpmnConfCommonElementVo> bpmnConfCommonElementVos =
            activitiList.Where(a => a.ElementId == elementId).ToList();
        if(bpmnConfCommonElementVos.Count==0)
        {
            return null;
        }

        BpmnConfCommonElementVo bpmnConfCommonElementVo = BpmnFlowUtil.GetNodeFromCurrentNext(activitiList, bpmnConfCommonElementVos[0].ElementId);
        return bpmnConfCommonElementVo == null ? null : new List<BpmnConfCommonElementVo> { bpmnConfCommonElementVo };
    }
}