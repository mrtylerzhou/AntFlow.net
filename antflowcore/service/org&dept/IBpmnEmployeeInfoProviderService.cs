using AntFlowCore.Entity;
using antflowcore.entityj;
using antflowcore.vo;

namespace antflowcore.service;

public interface IBpmnEmployeeInfoProviderService
{
    Dictionary<string, string> ProvideEmployeeInfo(IEnumerable<string> empIds);
    public DetailedUser QryLiteEmployeeInfoById(String id);
}