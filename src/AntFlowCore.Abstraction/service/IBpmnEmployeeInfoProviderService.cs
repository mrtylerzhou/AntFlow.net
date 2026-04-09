
using AntFlowCore.Base.entity;

namespace AntFlowCore.Abstraction.service;

public interface IBpmnEmployeeInfoProviderService
{
    Dictionary<string, string> ProvideEmployeeInfo(IEnumerable<string> empIds);
    public DetailedUser QryLiteEmployeeInfoById(String id);
}