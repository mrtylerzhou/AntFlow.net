using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableSingleService : IBpmVariableSingleService
{
    public BpmVariableSingleService(IBpmVariableSingleRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableSingleRepository _repository { get; }
}
