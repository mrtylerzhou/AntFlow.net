using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableButtonService : IAntFlowRepositoryMix<BpmVariableButton, IBpmVariableButtonRepository>
{
    List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, List<string> elementIds);
}
