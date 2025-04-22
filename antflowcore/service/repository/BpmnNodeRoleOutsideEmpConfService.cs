using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeRoleOutsideEmpConfService: AFBaseCurdRepositoryService<BpmnNodeRoleOutsideEmpConf>
{
    public BpmnNodeRoleOutsideEmpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}