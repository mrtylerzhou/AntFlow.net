using antflowcore.vo;

namespace antflowcore.service.interf.repository;

public interface IBpmnTemplateService
{
    void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId);
    void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo);
}