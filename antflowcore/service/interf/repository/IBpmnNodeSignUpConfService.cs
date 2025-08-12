using antflowcore.vo;

namespace antflowcore.service.interf.repository;

public interface IBpmnNodeSignUpConfService
{
    void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}