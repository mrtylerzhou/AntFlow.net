using AntFlowCore.Entity;

namespace antflowcore.service;

public interface IBpmnEmployeeInfoProviderService
{
    Dictionary<string, string> ProvideEmployeeInfo(IEnumerable<string> empIds);

    public Employee QryLiteEmployeeInfoById(String id);
}