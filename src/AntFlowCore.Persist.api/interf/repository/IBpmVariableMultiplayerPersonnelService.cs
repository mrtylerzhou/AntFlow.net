using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableMultiplayerPersonnelService : IAntFlowRepositoryMix<BpmVariableMultiplayerPersonnel, IBpmVariableMultiplayerPersonnelRepository>
{
    void Undertake(string processNumber, string taskTaskDefKey);
}
