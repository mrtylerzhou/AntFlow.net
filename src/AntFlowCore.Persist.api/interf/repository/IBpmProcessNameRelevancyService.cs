using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameRelevancyService : IAntFlowRepositoryMix<BpmProcessNameRelevancy, IBpmProcessNameRelevancyRepository>
{
    BpmProcessNameRelevancy? FindProcessNameRelevancy(string formCode);
    bool SelectCount(string formCode);
    void Add(BpmProcessNameRelevancy processNameRelevancy);
    List<string> ProcessKeyList(long id);
}
