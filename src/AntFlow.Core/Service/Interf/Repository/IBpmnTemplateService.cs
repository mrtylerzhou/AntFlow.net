using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Interface.Repository;

public interface IBpmnTemplateService
{
    void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId);
    void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo);
}