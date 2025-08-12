using AntFlowCore.Entity;

namespace antflowcore.service.interf.repository;

public interface IBpmnNodeService
{
    List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property);
}