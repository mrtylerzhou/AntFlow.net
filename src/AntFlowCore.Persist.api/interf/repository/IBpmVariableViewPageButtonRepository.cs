using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableViewPageButtonRepository : IBaseRepository<BpmVariableViewPageButton>
{
    List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum);
}
