using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeService : IBaseRepositoryService<BpmnNode>
{
    List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property);
}
