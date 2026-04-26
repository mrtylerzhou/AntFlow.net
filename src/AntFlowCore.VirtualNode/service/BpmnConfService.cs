using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnConfService : IBpmnConfService
{
    private readonly IBpmProcessNameService _bpmProcessNameService;

    public BpmnConfService(IBpmnConfRepository repository, IBpmProcessNameService bpmProcessNameService)
    {
        _repository = repository;
        _bpmProcessNameService = bpmProcessNameService;
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
        BpmnConf bpmnConf = _repository.Find(a => a.Id == id).FirstOrDefault();
        _bpmProcessNameService.EditProcessName(bpmnConf);
    }
}
