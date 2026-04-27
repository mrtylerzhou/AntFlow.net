using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeService : IAntFlowRepositoryMix<BpmnNode, IBpmnNodeRepository>
{
    List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property);
    int? GetCustomizeNodeSignType(long nodeId);
    int UpdateConfExtraFlags(long confId, int? extraFlags);
}
