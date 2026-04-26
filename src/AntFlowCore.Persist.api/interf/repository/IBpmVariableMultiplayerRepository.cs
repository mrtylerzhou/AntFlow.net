using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableMultiplayerRepository : IBaseRepository<BpmVariableMultiplayer>
{
    List<BpmVariableMultiplayer> QueryMultiplayersByProcessNumAndElementId(string processNum, string elementId);
}
