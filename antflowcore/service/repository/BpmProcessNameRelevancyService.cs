using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmProcessNameRelevancyService : AFBaseCurdRepositoryService<BpmProcessNameRelevancy>
{
    public BpmProcessNameRelevancyService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmProcessNameRelevancy FindProcessNameRelevancy(String formCode)
    {
        BpmProcessNameRelevancy bpmProcessNameRelevancy = baseRepo.Where(a => a.ProcessKey.Equals(formCode) && a.IsDel == 0).ToOne();
        return bpmProcessNameRelevancy;
    }

    public bool SelectCount(string formCode)
    {
        long count = this
            .baseRepo.
            Where(a => a.ProcessKey == formCode && a.IsDel == 0)
            .Count();
        return count > 0;
    }

    public void Add(BpmProcessNameRelevancy processNameRelevancy)
    {
        this.baseRepo.Insert(processNameRelevancy);
    }

    public List<string> ProcessKeyList(long id)
    {
        List<string> processK = this
            .baseRepo
            .Where(a => a.ProcessNameId == id && a.IsDel == 0)
            .ToList(a => a.ProcessKey);
        return processK;
    }
}