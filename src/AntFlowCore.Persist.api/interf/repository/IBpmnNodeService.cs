using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeService : IAntFlowRepositoryMix<BpmnNode, IBpmnNodeRepository>
{
    List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property);
    int? GetCustomizeNodeSignType(long nodeId);
    int UpdateConfExtraFlags(long confId, int? extraFlags);
}
