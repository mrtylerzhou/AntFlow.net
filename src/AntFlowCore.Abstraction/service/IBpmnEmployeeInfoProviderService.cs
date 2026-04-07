
using AntFlowCore.Core.entity;

namespace AntFlowCore.Abstraction;

public interface IBpmnEmployeeInfoProviderService
{
    Dictionary<string, string> ProvideEmployeeInfo(IEnumerable<string> empIds);
    public DetailedUser QryLiteEmployeeInfoById(String id);
}