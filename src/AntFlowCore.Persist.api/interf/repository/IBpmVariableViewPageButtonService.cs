using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableViewPageButtonService : IBaseRepositoryService<BpmVariableViewPageButton>
{
    List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum);
}
