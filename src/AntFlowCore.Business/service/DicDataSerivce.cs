using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class DicDataSerivce: IDicDataSerivce
{
    public DicDataSerivce(IDictDataRepository repository)
    {
        _repository = repository;
    }
    public IDictDataRepository _repository { get; }
}