using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmProcessNameRelevancyService : IBpmProcessNameRelevancyService
{
    public BpmProcessNameRelevancyService(IBpmProcessNameRelevancyRepository repository)
    {
        _repository = repository;
    }

    public IBpmProcessNameRelevancyRepository _repository { get; }

    public BpmProcessNameRelevancy FindProcessNameRelevancy(string formCode)
    {
        BpmProcessNameRelevancy bpmProcessNameRelevancy = _repository.FirstOrDefault(a => a.ProcessKey.Equals(formCode) && a.IsDel == 0);
        return bpmProcessNameRelevancy;
    }

    public bool SelectCount(string formCode)
    {
        long count = _repository.Count(a => a.ProcessKey == formCode && a.IsDel == 0);
        return count > 0;
    }

    public void Add(BpmProcessNameRelevancy processNameRelevancy)
    {
        _repository.Add(processNameRelevancy);
    }

    public List<string> ProcessKeyList(long id)
    {
        return _repository.GetProcessKeysByProcessNameId(id);
    }
}
