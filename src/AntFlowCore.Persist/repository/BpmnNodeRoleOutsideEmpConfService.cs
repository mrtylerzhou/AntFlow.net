using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeRoleOutsideEmpConfService: AFBaseCurdRepositoryService<BpmnNodeRoleOutsideEmpConf>,IBpmnNodeRoleOutsideEmpConfService
{
    public BpmnNodeRoleOutsideEmpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}