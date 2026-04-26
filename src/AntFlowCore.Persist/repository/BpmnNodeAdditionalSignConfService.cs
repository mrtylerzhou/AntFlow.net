using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeAdditionalSignConfService : IBpmnNodeAdditionalSignConfService
{
    public BpmnNodeAdditionalSignConfService(IBpmnNodeAdditionalSignConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeAdditionalSignConfRepository _repository { get; }

    public List<BpmnNodeAdditionalSignConf> GetByBpmnNodeId(long? bpmnNodeId)
    {
        return _repository.Find(conf => conf.BpmnNodeId == bpmnNodeId);
    }

    public void Insert(List<BpmnNodeAdditionalSignConf> entities)
    {
        _repository.AddRange(entities);
    }
}
