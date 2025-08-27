using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmProcessNameRelevancyService : AFBaseCurdRepositoryService<BpmProcessNameRelevancy>,
    IBpmProcessNameRelevancyService
{
    public BpmProcessNameRelevancyService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmProcessNameRelevancy FindProcessNameRelevancy(string formCode)
    {
        BpmProcessNameRelevancy bpmProcessNameRelevancy =
            baseRepo.Where(a => a.ProcessKey.Equals(formCode) && a.IsDel == 0).ToOne();
        return bpmProcessNameRelevancy;
    }

    public bool SelectCount(string formCode)
    {
        long count = baseRepo.Where(a => a.ProcessKey == formCode && a.IsDel == 0)
            .Count();
        return count > 0;
    }

    public void Add(BpmProcessNameRelevancy processNameRelevancy)
    {
        baseRepo.Insert(processNameRelevancy);
    }

    public List<string> ProcessKeyList(long id)
    {
        List<string> processK = baseRepo
            .Where(a => a.ProcessNameId == id && a.IsDel == 0)
            .ToList(a => a.ProcessKey);
        return processK;
    }
}