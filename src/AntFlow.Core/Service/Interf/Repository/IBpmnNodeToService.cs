using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Interface.Repository;

public interface IBpmnNodeToService
{
    void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}