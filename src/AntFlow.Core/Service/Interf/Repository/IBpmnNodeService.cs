using AntFlow.Core.Entity;

namespace AntFlow.Core.Service.Interface.Repository;

public interface IBpmnNodeService
{
    List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property);
}