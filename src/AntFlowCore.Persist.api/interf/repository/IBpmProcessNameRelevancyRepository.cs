using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameRelevancyRepository : IBaseRepository<BpmProcessNameRelevancy>
{
    List<string> GetProcessKeysByProcessNameId(long id);
}
