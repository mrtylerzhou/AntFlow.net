using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmProcessNameService: AFBaseCurdRepositoryService<BpmProcessName>
{
    private readonly BpmProcessNameRelevancyService _bpmProcessNameRelevancyService;

    public BpmProcessNameService(
        IFreeSql freeSql,
        BpmProcessNameRelevancyService bpmProcessNameRelevancyService
        ) : base(freeSql)
    {
        _bpmProcessNameRelevancyService = bpmProcessNameRelevancyService;
    }
    
    public BpmProcessName GetBpmProcessName(String processKey) {
        BpmProcessNameRelevancy processNameRelevancy = _bpmProcessNameRelevancyService.FindProcessNameRelevancy(processKey);
        if (processNameRelevancy==null) {
            return new BpmProcessName();
        }

        BpmProcessName bpmProcessName = baseRepo.Where(a=>a.Id.Equals(processNameRelevancy.ProcessNameId)).ToOne();
        return bpmProcessName;
    }
}