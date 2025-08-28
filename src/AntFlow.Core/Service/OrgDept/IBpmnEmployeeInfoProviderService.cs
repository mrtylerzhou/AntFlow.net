using AntFlow.Core.Entity;

namespace AntFlow.Core.Service;

public interface IBpmnEmployeeInfoProviderService
{
    Dictionary<string, string> ProvideEmployeeInfo(IEnumerable<string> empIds);
    public Employee QryLiteEmployeeInfoById(string id);
}