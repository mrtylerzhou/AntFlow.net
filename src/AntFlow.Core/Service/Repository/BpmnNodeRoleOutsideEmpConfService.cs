using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeRoleOutsideEmpConfService : AFBaseCurdRepositoryService<BpmnNodeRoleOutsideEmpConf>,
    IBpmnNodeRoleOutsideEmpConfService
{
    public BpmnNodeRoleOutsideEmpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}