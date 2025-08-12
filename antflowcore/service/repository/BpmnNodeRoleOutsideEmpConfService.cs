using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeRoleOutsideEmpConfService: AFBaseCurdRepositoryService<BpmnNodeRoleOutsideEmpConf>,IBpmnNodeRoleOutsideEmpConfService
{
    public BpmnNodeRoleOutsideEmpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}