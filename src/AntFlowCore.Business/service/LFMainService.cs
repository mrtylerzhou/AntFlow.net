using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class LFMainService: ILFMainService
{
    public LFMainService(ILFMainRepository repository)
    {
        _repository = repository;
    }

    public ILFMainRepository _repository { get; }
}