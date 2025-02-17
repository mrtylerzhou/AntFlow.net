using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmProcessNameRelevancyService: AFBaseCurdRepositoryService<BpmProcessNameRelevancy>
{
    public BpmProcessNameRelevancyService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public BpmProcessNameRelevancy FindProcessNameRelevancy(String formCode) {
        BpmProcessNameRelevancy bpmProcessNameRelevancy = baseRepo.Where(a=>a.ProcessKey.Equals(formCode)&&a.IsDel==0).ToOne();
        return bpmProcessNameRelevancy;
    }
}