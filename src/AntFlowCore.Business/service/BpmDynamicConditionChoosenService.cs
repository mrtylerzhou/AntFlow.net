using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmDynamicConditionChoosenService : IBpmDynamicConditionChoosenService
{
    public BpmDynamicConditionChoosenService(IBpmDynamicConditionChoosenRepository repository)
    {
        _repository = repository;
    }

    public IBpmDynamicConditionChoosenRepository _repository { get; }
}
