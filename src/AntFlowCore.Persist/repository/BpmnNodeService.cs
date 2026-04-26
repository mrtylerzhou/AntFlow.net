using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeService : IBpmnNodeService
{
    public BpmnNodeService(IBpmnNodeRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeRepository _repository { get; }

    public List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property)
    {
        return _repository.GetNodesByFormCodeAndProperty(formCode, property);
    }

    public int? GetCustomizeNodeSignType(long nodeId)
    {
        return _repository.GetCustomizeNodeSignType(nodeId);
    }

    public int UpdateConfExtraFlags(long confId, int? extraFlags)
    {
        return _repository.UpdateConfExtraFlags(confId, extraFlags);
    }
}
