using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnConfService : IBpmnConfService
{
    public BpmnConfService(IBpmnConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnConfRepository _repository { get; }

    public string GetMaxBpmnCode(String bpmnCodeParts)
    {
        return _repository.GetMaxBpmnCode(bpmnCodeParts);
    }

    public String ReCheckBpmnCode(String bpmnCodeParts, String bpmnCode)
    {
        return _repository.ReCheckBpmnCode(bpmnCodeParts, bpmnCode);
    }

    public List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo)
    {
        return _repository.SelectPageList(page, vo);
    }

    public void EffectiveBpmnConf(int id)
    {
        _repository.EffectiveBpmnConf(id);
        // Process name edit removed (IBpmProcessNameService deleted)
    }
}
