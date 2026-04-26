using AntFlowCore.Abstraction.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.repository;

public interface IBpmnTemplateRepository : IBaseRepository<BpmnTemplate>
{
    void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId);
    void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo);
}
