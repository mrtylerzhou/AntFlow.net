using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableViewPageButtonService : IAntFlowRepositoryMix<BpmVariableViewPageButton, IBpmVariableViewPageButtonRepository>
{
    List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum);
}
