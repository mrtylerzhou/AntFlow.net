using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameRelevancyService : IBaseRepositoryService<BpmProcessNameRelevancy>
{
    BpmProcessNameRelevancy? FindProcessNameRelevancy(string formCode);
    bool SelectCount(string formCode);
    void Add(BpmProcessNameRelevancy processNameRelevancy);
    List<string> ProcessKeyList(long id);
}
