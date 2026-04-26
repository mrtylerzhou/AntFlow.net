using AntFlowCore.Base.entity;
using AntFlowCore.Persist;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeBusinessTableConfService : IBpmnNodeBusinessTableConfService
{
    public BpmnNodeBusinessTableConfService(IBpmnNodeBusinessTableConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeBusinessTableConfRepository _repository { get; }

    public BpmnNodeBusinessTableConf? GetByBpmnNodeId(long bpmnNodeId)
    {
        return _repository.Find(conf => conf.BpmnNodeId == bpmnNodeId).FirstOrDefault();
    }

    public void Insert(BpmnNodeBusinessTableConf entity)
    {
        _repository.Add(entity);
    }
}
