using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableButtonService : IBaseRepositoryService<BpmVariableButton>
{
    List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, List<string> elementIds);
}
