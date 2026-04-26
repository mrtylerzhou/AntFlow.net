using AntFlowCore.Base.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Abstraction.service.repository;

public interface IBpmnConfService : IAntFlowRepositoryMix<BpmnConf, IBpmnConfRepository>
{
    string GetMaxBpmnCode(string bpmnCodeParts);
    string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode);
    List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo);
    void EffectiveBpmnConf(int id);
}
