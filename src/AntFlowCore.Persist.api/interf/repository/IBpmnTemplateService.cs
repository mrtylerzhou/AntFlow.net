using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnTemplateService : IAntFlowRepositoryMix<BpmnTemplate, IBpmnTemplateRepository>
{
    void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId);
    void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo);
}
