using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableService : IBaseRepositoryService<BpmVariable>
{
    List<string> GetNodeIdsByeElementId(string processNumber, string elementId);
    List<string> GetElementIdsdByNodeId(string processNumber, string nodeId);
}
