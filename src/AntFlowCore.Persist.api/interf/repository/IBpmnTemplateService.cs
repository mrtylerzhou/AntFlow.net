using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnTemplateService : IBaseRepositoryService<BpmnTemplate>
{
    void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId);
    void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo);
}
