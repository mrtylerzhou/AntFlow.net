using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class AFExecutionService:IAFExecutionService
{
    public AFExecutionService(IBpmAfExecutionRepository repository)
    {
        _repository = repository;
    }
    public IBpmAfExecutionRepository _repository { get; }
}