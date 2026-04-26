using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeButtonConfService : IBpmnNodeButtonConfService
{
    public BpmnNodeButtonConfService(IBpmnNodeButtonConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeButtonConfRepository _repository { get; }

    public void EditButtons(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        _repository.EditButtons(bpmnNodeVo, bpmnNodeId);
    }

    public List<BpmnNodeButtonConf>? QueryConfByBpmnConde(string version)
    {
        return _repository.QueryConfByBpmnConde(version);
    }
}
