using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameRelevancyService : IAntFlowRepositoryMix<BpmProcessNameRelevancy, IBpmProcessNameRelevancyRepository>
{
    BpmProcessNameRelevancy? FindProcessNameRelevancy(string formCode);
    bool SelectCount(string formCode);
    void Add(BpmProcessNameRelevancy processNameRelevancy);
    List<string> ProcessKeyList(long id);
}
