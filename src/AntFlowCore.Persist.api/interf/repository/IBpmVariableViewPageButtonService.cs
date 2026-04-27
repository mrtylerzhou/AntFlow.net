using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableViewPageButtonService : IAntFlowRepositoryMix<BpmVariableViewPageButton, IBpmVariableViewPageButtonRepository>
{
    List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum);
}
