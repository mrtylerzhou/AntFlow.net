using antflowcore.vo;

namespace antflowcore.service.interf.repository;

public interface IBpmnNodeToService
{
    void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}