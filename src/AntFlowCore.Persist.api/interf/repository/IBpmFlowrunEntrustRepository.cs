using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmFlowrunEntrustRepository : IBaseRepository<BpmFlowrunEntrust>
{
    List<BpmFlowrunEntrust> GetEntrustsByProcessNumber(string processNumber);
}
