using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeRoleConfService: AFBaseCurdRepositoryService<BpmnNodeRoleConf>,IBpmnNodeRoleConfService
{
    public BpmnNodeRoleConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}