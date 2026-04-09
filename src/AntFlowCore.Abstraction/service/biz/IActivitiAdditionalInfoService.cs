using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IActivitiAdditionalInfoService
{
    List<BpmnConfCommonElementVo> GetActivitiList(BpmAfTaskInst historicProcessInstance);
    List<BpmnConfCommonElementVo> GetActivitiList(string procDefId);
    Dictionary<string, List<BpmAfTaskInst>> GetVariableInstanceMap(string procInstId);
}