using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class LFMainService: ILFMainService
{
    public LFMainService(ILFMainRepository repository)
    {
        _repository = repository;
    }

    public ILFMainRepository _repository { get; }
}