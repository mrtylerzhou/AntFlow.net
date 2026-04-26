using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class LFMainFieldService: ILFMainFieldService
{
    public LFMainFieldService(ILFMainFieldRepository repository)
    {
        _repository = repository;
    }

    public ILFMainFieldRepository _repository { get; }
}