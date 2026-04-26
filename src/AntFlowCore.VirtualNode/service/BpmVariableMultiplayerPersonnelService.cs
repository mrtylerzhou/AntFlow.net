using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableMultiplayerPersonnelService : IBpmVariableMultiplayerPersonnelService
{
    public BpmVariableMultiplayerPersonnelService(IBpmVariableMultiplayerPersonnelRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableMultiplayerPersonnelRepository _repository { get; }

    public void Undertake(string processNumber, string taskTaskDefKey)
    {
        _repository.Undertake(processNumber, taskTaskDefKey);
    }
}
