using AntFlowCore.Core.entity;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IActivitiAdditionalInfoService
{
    List<BpmnConfCommonElementVo> GetActivitiList(BpmAfTaskInst historicProcessInstance);
    List<BpmnConfCommonElementVo> GetActivitiList(string procDefId);
    Dictionary<string, List<BpmAfTaskInst>> GetVariableInstanceMap(string procInstId);
}