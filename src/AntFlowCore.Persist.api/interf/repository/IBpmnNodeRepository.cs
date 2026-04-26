using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeRepository : IBaseRepository<BpmnNode>
{
    List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property);
    int? GetCustomizeNodeSignType(long nodeId);
    int UpdateConfExtraFlags(long confId, int? extraFlags);
}
