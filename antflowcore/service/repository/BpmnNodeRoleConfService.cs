using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeRoleConfService: AFBaseCurdRepositoryService<BpmnNodeRoleConf>
{
    public BpmnNodeRoleConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}