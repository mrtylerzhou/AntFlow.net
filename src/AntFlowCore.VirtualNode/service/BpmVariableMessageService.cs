using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableMessageService : IBpmVariableMessageService
{
    public BpmVariableMessageService(IBpmVariableMessageRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableMessageRepository _repository { get; }
}
