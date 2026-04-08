using AntFlowCore.Base.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableMultiplayerPersonnelService : IBaseRepositoryService<BpmVariableMultiplayerPersonnel>
{
    void Undertake(string processNumber, string taskTaskDefKey);
}
