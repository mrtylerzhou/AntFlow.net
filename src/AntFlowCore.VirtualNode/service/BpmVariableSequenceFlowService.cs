using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableSequenceFlowService : IBpmVariableSequenceFlowService
{
    public BpmVariableSequenceFlowService(IBpmVariableSequenceFlowRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableSequenceFlowRepository _repository { get; }
}
