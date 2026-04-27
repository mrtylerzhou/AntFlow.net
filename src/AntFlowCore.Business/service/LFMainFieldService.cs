using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class LFMainFieldService: ILFMainFieldService
{
    public LFMainFieldService(ILFMainFieldRepository repository)
    {
        _repository = repository;
    }

    public ILFMainFieldRepository _repository { get; }
}