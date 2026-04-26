using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableButtonRepository : IBaseRepository<BpmVariableButton>
{
    List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, List<string> elementIds);
}
