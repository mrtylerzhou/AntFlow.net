using AntFlowCore.Base.entity;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Abstraction.service.repository;

public interface IBpmnConfService
{
    IBaseRepository<BpmnConf> baseRepo { get; }
    string GetMaxBpmnCode(string bpmnCodeParts);
    string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode);
    List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo);
    void EffectiveBpmnConf(int id);
}
