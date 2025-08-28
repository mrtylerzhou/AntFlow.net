using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Interface.Repository;

public interface IBpmnNodeSignUpConfService
{
    void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}