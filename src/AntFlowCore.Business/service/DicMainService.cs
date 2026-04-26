using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class DicMainService : IDicMainService
{
    public DicMainService(IDicMainRepository repository)
    {
        _repository = repository;
    }

    public IDicMainRepository _repository { get; }
}
