using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeAssignLevelConfService: AFBaseCurdRepositoryService<BpmnNodeAssignLevelConf>,IBpmnNodeAssignLevelConfService
{
    public BpmnNodeAssignLevelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}