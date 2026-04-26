using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeToService : IBpmnNodeToService
{
    public BpmnNodeToService(IBpmnNodeToRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeToRepository _repository { get; }

    public void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        _repository.EditNodeTo(bpmnNodeVo, bpmnNodeId);
    }
}
