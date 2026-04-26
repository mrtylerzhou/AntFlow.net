using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfRepository : IBaseRepository<BpmnConf>
{
    string? GetMaxBpmnCode(string bpmnCodeParts);
    string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode);
    List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo);
    void EffectiveBpmnConf(int id);
    BpmnConf GetBpmnConfByFormCode(string formCode);
    List<BpmnConf> GetBpmnConfByFormCodeBatch(List<string> formCodes);
}
