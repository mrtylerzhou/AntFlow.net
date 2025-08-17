using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeRoleConfService: AFBaseCurdRepositoryService<BpmnNodeRoleConf>,IBpmnNodeRoleConfService
{
    public BpmnNodeRoleConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}