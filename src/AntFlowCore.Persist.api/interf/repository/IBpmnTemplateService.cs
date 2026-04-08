using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnTemplateService : IBaseRepositoryService<BpmnTemplate>
{
    void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId);
    void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo);
}
