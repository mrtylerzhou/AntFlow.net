using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableMultiplayerService : IBpmVariableMultiplayerService
{
    public BpmVariableMultiplayerService(IBpmVariableMultiplayerRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableMultiplayerRepository _repository { get; }
}
