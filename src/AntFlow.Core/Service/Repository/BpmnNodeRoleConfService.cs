using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeRoleConfService : AFBaseCurdRepositoryService<BpmnNodeRoleConf>, IBpmnNodeRoleConfService
{
    public BpmnNodeRoleConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}